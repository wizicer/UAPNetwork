namespace IcerDesign
{
    using System;
    using System.Collections;
    using System.Net.Sockets;
    using System.Threading;
    using System.Timers;

    using Amib.Threading;

    using IcerDesign.Router;
    using IcerDesign.SocketLibrary;

    using log4net;

    using UAPLibrary.Packet;
    using UAPLibrary.Utility;

    public delegate void SendToLogServerDelegate(UapBase packet);

    public class DebugHelper
    {
        private static ILog log = log4net.LogManager.GetLogger("Main");
        public static void WriteLog(string content)
        {
            log.Debug(content);
        }
    }

    public class UapCommunicator : IDisposable
    {
        #region variables
        private AsyncSocketClient _asClient;
        private int _enquireLinkInterval;
        private int _sleepTimeAfterSocketFailure;
        private System.Timers.Timer _ussdTimer;
        private bool _isRebinding = false;
        private AsynchronousInvoke<UapBase> _queuePool;

        #endregion

        #region properties
        public event SendToLogServerDelegate OnSendToLogServer;

        public int NoReplyQueueLength { get; set; }
        public int MaxQueueLength { get; set; }
        public int MinQueueLength { get; set; }
        public int MaxThreadsNumber { get; set; }

        public int QueuePoolInterval { get; set; }
        public string SendBackServiceType { get; set; }
        public string SendBackShortMessage { get; set; }
        public int ListenBufferSize { private get; set; }

        public int MaxRetry { private get; set; }

        /// <summary>
        /// The port on the SMSC to connect to.
        /// </summary>
        public short Port { get; set; }

        /// <summary>
        /// The system type to use when connecting to the SMSC.
        /// </summary>
        public string SystemType { get; set; }

        /// <summary>
        /// The system ID to use when connecting to the SMSC.  This is, 
        /// in essence, a user name.
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// The password to use when connecting to an SMSC.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The host to bind this SMPPCommunicator to.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The SMPP specification version to use.
        /// </summary>
        public uint InterfaceVersion { get; set; }

        /// <summary>
        /// Set to the number of seconds that should elapse in between enquire_link 
        /// packets.  Setting this to anything other than 0 will enable the timer, setting 
        /// it to 0 will disable the timer.  Note that the timer is only started/stopped 
        /// during a bind/unbind.  Negative values are ignored.
        /// </summary>
        public int EnquireLinkInterval
        {
            get
            {
                return this._enquireLinkInterval;
            }

            set
            {
                if (value >= 0)
                    this._enquireLinkInterval = value;
            }
        }

        /// <summary>
        /// Sets the number of seconds that the system will wait before trying to rebind 
        /// after a total network failure(due to cable problems, etc).  Negative values are 
        /// ignored.
        /// </summary>
        public int SleepTimeAfterSocketFailure
        {
            get
            {
                return this._sleepTimeAfterSocketFailure;
            }

            set
            {
                if (value >= 0)
                    this._sleepTimeAfterSocketFailure = value;
            }
        }
        #endregion properties

        #region events
        /// <summary>
        /// 当从通讯接口收到一个UapBind包时触发
        /// </summary>
        public event UapBindEventHandler OnBindEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapBindResp包时触发
        /// </summary>
        public event UapBindRespEventHandler OnBindRespEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapUnbind包时触发
        /// </summary>
        public event UapUnbindEventHandler OnUnbindEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapUnbindResp包时触发
        /// </summary>
        public event UapUnbindRespEventHandler OnUnbindRespEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapEnquireLink包时触发
        /// </summary>
        public event UapEnquireLinkEventHandler OnEnquireLinkEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapEnquireLinkResp包时触发
        /// </summary>
        public event UapEnquireLinkRespEventHandler OnEnquireLinkRespEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapBegin包时触发
        /// </summary>
        public event UapBeginEventHandler OnBeginEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapContinue包时触发
        /// </summary>
        public event UapContinueEventHandler OnContinueEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapEnd包时触发
        /// </summary>
        public event UapEndEventHandler OnEndEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapAbort包时触发
        /// </summary>
        public event UapAbortEventHandler OnAbortEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapSwitch包时触发
        /// </summary>
        public event UapSwitchEventHandler OnSwitchEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapChargeind包时触发
        /// </summary>
        public event UapChargeindEventHandler OnChargeindEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapChargeindResp包时触发
        /// </summary>
        public event UapChargeindRespEventHandler OnChargeindRespEvent;
        /// <summary>
        /// 当从通讯接口收到一个UapSwitchBegin包时触发
        /// </summary>
        public event UapSwitchBeginEventHandler OnSwitchBeginEvent;

        #endregion events

        #region Constructors

        public UapCommunicator()
        {
        }

        public void Init()
        {
            this._queuePool = new AsynchronousInvoke<UapBase>("SoIn", this.MaxThreadsNumber, new int[] { this.MinQueueLength, this.MaxQueueLength, this.NoReplyQueueLength });
            this._queuePool.OnLevelArrived += new LevelArrivedDelegate<UapBase>(this.QueuePool_OnLevelArrived);
            this._queuePool.Start();

        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// -------0-------+-------1-------+------2------+-------3---------
        ///               Min             Max          NoRep
        ///    处理全部        处理PSSRR      回发所有       所有均舍弃
        ///                    回发其他
        /// Min = MinQueueLength
        /// Max = MaxQueueLength
        /// NoRep = NoReplyQueueLength
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueuePool_OnLevelArrived(object sender, LevelArrivedEventArgs<UapBase> e)
        {
            UapMessageBase dpacket = (e.Packet as UapMessageBase);
            if (e.Packet != null)
            {
                switch (e.LevelNumber)
                {
                    case 1:
                        if (dpacket is UapBegin)
                        {
                            e.CancelEnqueue = true;
                            this.SendErrorBackToUser(dpacket);
                        }
                        break;
                    case 2:
                        e.CancelEnqueue = true;
                        this.SendErrorBackToUser(dpacket);
                        break;
                    case 3:
                        e.CancelEnqueue = true;
                        break;
                    default:
                        break;
                }
            }

        }

        private void SendErrorBackToUser(UapMessageBase dpacket)
        {
            // 发送错误提示信息
            UapMessageBase uap;
            if (this.SendBackServiceType == "USSRR")
            {
                uap = new UapContinue();
            }
            else if (this.SendBackServiceType.StartsWith("REL"))
            {
                uap = new UapEnd();
            }
            else
            {
                return;
            }
            uap.UssdVersion = dpacket.UssdVersion;
            uap.UssdOpType = UapBase.UssdOpTypeEnum.Request;
            uap.MsIsdn = dpacket.MsIsdn;
            uap.ServiceCode = dpacket.ServiceCode;
            uap.CodeScheme = 0x44;
            uap.UssdContent = this.SendBackShortMessage;

            // 发送至日志服务器
            this.OnSendToLogServer(uap);

            this.SendUap(uap);
        }

        ~UapCommunicator()
        {
            if (this._queuePool != null)
            {
                this._queuePool.Stop();
            }
            this.Unbind();
        }

        #endregion

        #region Communicate with USSDG

        /// <summary>
        /// 绑定至USSD网关
        /// </summary>
        public void Bind()
        {
            try
            {
                this.ConnectToUSSDG();
            }
            catch (SocketException se)
            {
                Thread.Sleep(this._sleepTimeAfterSocketFailure * 1000);
                this.Bind();
            }
            catch (Exception exc)
            {
                //Bind again
                Thread.Sleep(this._sleepTimeAfterSocketFailure * 1000);
                this.Bind();
            }
        }

        /// <summary>
        /// 连接USSD网关
        /// </summary>
        private void ConnectToUSSDG()
        {
            if (this._asClient != null)
            {
                this._asClient.Disconnect();
            }

            this.StopUssdTimer();


            //connect USSDG
            this._asClient = new AsyncSocketClient(this.ListenBufferSize, null,
                                              new AsyncSocketClient.MessageHandler(this.ClientMessageHandler), null,
                                              new AsyncSocketClient.ErrorHandler(this.ClientErrorHandler));

            this._asClient.Connect(this.Host, this.Port);

            UapBind request = new UapBind();
            request.SystemId = this.SystemId;
            request.Password = this.Password;
            request.SystemType = this.SystemType;
            request.InterfaceVersion = this.InterfaceVersion;

            this.SendUap(request);

            this.StartUssdTimer();
        }

        /// <summary>
        /// 进行重新绑定，当前策略如下：
        /// 重连的次数限定为三次，分别按照如下进行操作：
        /// 1、失败后立即重连；
        /// 2、失败3秒后重连；
        /// 3、失败5秒后重连；
        /// 4、仍旧失败就立即结束自己，等待ServiceWatcher来重新启动自己。
        /// </summary>
        /// <param name="level">绑定的次数</param>
        public void Rebind(int level)
        {
            // 防止多次触发重新绑定
            if (this._isRebinding && level == 0) return;
            this._isRebinding = true;
            switch (level)
            {
                case 0:
                    Console.WriteLine("第一次进行网关重新绑定！");
                    DebugHelper.WriteLog("第一次进行网关重新绑定！");
                    break;
                case 1:
                    Console.WriteLine("第二次进行网关重新绑定！");
                    DebugHelper.WriteLog("第二次进行网关重新绑定！");
                    Thread.Sleep(3000);
                    break;
                case 2:
                    Console.WriteLine("第三次进行网关重新绑定！");
                    DebugHelper.WriteLog("第三次进行网关重新绑定！");
                    Thread.Sleep(5000);
                    break;
                case 3:
                    Console.WriteLine("绑定失败，退出程序！");
                    DebugHelper.WriteLog("绑定失败，退出程序！");
                    System.Environment.Exit(0);
                    break;
            }
            try
            {
                this.ConnectToUSSDG();
                Console.WriteLine("重连网关成功！");
                DebugHelper.WriteLog("重连网关成功！");
            }
            catch (Exception exc)
            {
                //OnError(this, new CommonErrorEventArgs(exc));
                this.Rebind(level + 1);
            }
            this._isRebinding = false;
        }

        /// <summary>
        /// 和USSD网关解除绑定
        /// </summary>
        public void Unbind()
        {
            try
            {
                if (this._asClient != null)
                {
                    this._asClient.Disconnect();
                }
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLog("解除绑定时出现错误，错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// Sends a user-specified Pdu(see the RoaminSMPP base library for
        /// Pdu types).  This allows complete flexibility for sending Pdus.
        /// </summary>
        /// <param name="packet">The Pdu to send.</param>
        public void SendUap(UapBase packet)
        {
#if DEBUG
            if (!(packet is UapEnquireLink))
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(String.Format("Send: {0}", packet));
                Console.ForegroundColor = color;
            }
#endif
            bool sendFailed = true;
            int retryCount = 0;

            while (sendFailed && retryCount < this.MaxRetry)
            {
                try
                {
                    this._asClient.Send(packet.GetPacket());

                    sendFailed = false;
                }
                catch (Exception e)
                {
                    //OnError(this, new CommonErrorEventArgs(e));

                    Thread.Sleep(this._sleepTimeAfterSocketFailure * 1000);
                    retryCount++;
                }
            }
        }

        #endregion

        #region handlers

        /// <summary>
        /// Callback method to handle received messages.  The AsyncSocketClient
        /// library calls this; don't call it yourself.
        /// </summary>
        /// <param name="client">The client to receive messages from.</param>
        internal void ClientMessageHandler(AsyncSocketClient client)
        {
            Queue responseQueue = null;
            try
            {
                byte[] buffer = client.Buffer;
                int bufferSize = client.BufferSize;
                responseQueue = UapFactory.GetUapQueue(ref buffer, ref bufferSize);
                Array.Copy(buffer, client.Buffer, bufferSize);
                client.BufferSize = bufferSize;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLog("解析Pdu失败，错误信息：" + ex.Message);
            }
            try
            {
                if (responseQueue != null)
                {
                    foreach (UapBase response in responseQueue)
                    {
                        if (response != null)
                        {
                            WorkItemCallback callback = new WorkItemCallback(this.ProcessPdu);
                            this._queuePool.Enqueue(callback, response);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                //OnError(this, new CommonErrorEventArgs(exception));
            }
        }

        /// <summary>
        /// Callback method to handle socket closing.
        /// </summary>
        /// <param name="client">The client to receive messages from.</param>
        internal void ClientCloseHandler(AsyncSocketClient client)
        {
            //fire off a closing event
            System.EventArgs e = new System.EventArgs();
            //OnClose(this, e);
        }

        /// <summary>
        /// Callback method to handle errors.
        /// </summary>
        /// <param name="client">The client to receive messages from.</param>
        /// <param name="exception">The generated exception.</param>
        internal void ClientErrorHandler(AsyncSocketClient client,
                                         Exception exception)
        {
            //fire off an error handler
            //OnError(this, new CommonErrorEventArgs(exception));
        }

        #endregion

        #region Message Processor

        /// <summary>
        /// 楼上的单Pdu版
        /// </summary>
        /// <param name="stateObj">byte packets.</param>
        private object ProcessPdu(object stateObj)
        {
            UapBase response = stateObj as UapBase;
            if (response != null) this.FireEvents(response);
            //QueuePool.DecreaseThreads();
            return null;
        }

        /// <summary>
        /// Fires an event off based on what Pdu is sent in.
        /// </summary>
        /// <param name="response">The response to fire an event for.</param>
        private void FireEvents(UapBase response)
        {
#if DEBUG
            if (!(response is UapEnquireLinkResp))
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(String.Format("Recv: {0}", response));
                Console.ForegroundColor = color;
            }
#endif
            if (response is UapBind)
            {
                if (this.OnBindEvent != null)
                {
                    this.OnBindEvent(this, new UapBindEventArgs((UapBind)response));
                }
            }
            else if (response is UapBindResp)
            {
                if (this.OnBindRespEvent != null)
                {
                    this.OnBindRespEvent(this, new UapBindRespEventArgs((UapBindResp)response));
                }
            }
            else if (response is UapUnbind)
            {
                if (this.OnUnbindEvent != null)
                {
                    this.OnUnbindEvent(this, new UapUnbindEventArgs((UapUnbind)response));
                }
            }
            else if (response is UapUnbindResp)
            {
                if (this.OnUnbindRespEvent != null)
                {
                    this.OnUnbindRespEvent(this, new UapUnbindRespEventArgs((UapUnbindResp)response));
                }
            }
            else if (response is UapEnquireLink)
            {
                if (this.OnEnquireLinkEvent != null)
                {
                    this.OnEnquireLinkEvent(this, new UapEnquireLinkEventArgs((UapEnquireLink)response));
                }
            }
            else if (response is UapEnquireLinkResp)
            {
                if (this.OnEnquireLinkRespEvent != null)
                {
                    this.OnEnquireLinkRespEvent(this, new UapEnquireLinkRespEventArgs((UapEnquireLinkResp)response));
                }
            }
            else if (response is UapBegin)
            {
                if (this.OnBeginEvent != null)
                {
                    this.OnBeginEvent(this, new UapBeginEventArgs((UapBegin)response));
                }
            }
            else if (response is UapContinue)
            {
                if (this.OnContinueEvent != null)
                {
                    this.OnContinueEvent(this, new UapContinueEventArgs((UapContinue)response));
                }
            }
            else if (response is UapEnd)
            {
                if (this.OnEndEvent != null)
                {
                    this.OnEndEvent(this, new UapEndEventArgs((UapEnd)response));
                }
            }
            else if (response is UapAbort)
            {
                if (this.OnAbortEvent != null)
                {
                    this.OnAbortEvent(this, new UapAbortEventArgs((UapAbort)response));
                }
            }
            else if (response is UapSwitch)
            {
                if (this.OnSwitchEvent != null)
                {
                    this.OnSwitchEvent(this, new UapSwitchEventArgs((UapSwitch)response));
                }
            }
            else if (response is UapChargeind)
            {
                if (this.OnChargeindEvent != null)
                {
                    this.OnChargeindEvent(this, new UapChargeindEventArgs((UapChargeind)response));
                }
            }
            else if (response is UapChargeindResp)
            {
                if (this.OnChargeindRespEvent != null)
                {
                    this.OnChargeindRespEvent(this, new UapChargeindRespEventArgs((UapChargeindResp)response));
                }
            }
            else if (response is UapSwitchBegin)
            {
                if (this.OnSwitchBeginEvent != null)
                {
                    this.OnSwitchBeginEvent(this, new UapSwitchBeginEventArgs((UapSwitchBegin)response));
                }
            }

        }

        #endregion

        #region System Timer

        /// <summary>
        /// 启动发送心跳包至USSD网关
        /// </summary>
        private void StartUssdTimer()
        {
            if (this._enquireLinkInterval > 0)
            {
                // Start USSD Timer.
                if (this._ussdTimer != null)
                {
                    this._ussdTimer.Dispose();
                }
                this._ussdTimer = new System.Timers.Timer();

                this._ussdTimer.Interval = this._enquireLinkInterval * 1000;
                this._ussdTimer.Elapsed += new ElapsedEventHandler(this.USSDTimerElapsed);
                this._ussdTimer.Start();
            }
        }

        /// <summary>
        /// 停止发送心跳包至USSD网关
        /// </summary>
        private void StopUssdTimer()
        {
            if (this._ussdTimer != null)
            {
                this._ussdTimer.Stop();
            }
        }

        /// <summary>
        /// Sends out an enquire_link packet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        private void USSDTimerElapsed(object sender, ElapsedEventArgs ea)
        {
            this.SendKeepAliveToUSSDG();
        }

        /// <summary>
        /// 与USSDG保持连接
        /// </summary>
        public void SendKeepAliveToUSSDG()
        {
            if (this._asClient != null)
            {
                if (this._asClient.Connected)
                {
                    UapEnquireLink link = new UapEnquireLink();
                    try
                    {
                        this.SendUap(link);
                        //_asClient.Send(link.GetPacket());
                    }
                    catch
                    {
                        this.Rebind(0);
                    }
                }
                else
                {
                    this.Rebind(0);
                }
            }
        }

        #endregion

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            this._queuePool.Dispose();
            this.StopUssdTimer();
        }
    }
}
