namespace IcerDesign.Router
{
    using System;
    using System.Collections.Generic;

    using Amib.Threading;

    using log4net;

    /// <summary>
    /// 转发路由管理器
    /// </summary>
    /// <typeparam name="T">转发数据包类型</typeparam>
    public class RouteManager<T> : IDisposable
    {
        internal class DebugHelper
        {
            private static ILog log = log4net.LogManager.GetLogger("Route");
            public static void WriteLog(string content)
            {
                log.Debug(content);
            }
        }
        private Dictionary<string, ICommunicator<T>> _communicators = new Dictionary<string, ICommunicator<T>>();
        private AsynchronousInvoke<StateObject<T>> _asynInvoke;

        private class StateObject<V>
        {
            public ICommunicator<V> Communicator { get; set; }
            public RoutePacket<V> Packet { get; set; }
        }

        /// <summary>
        /// 初始化转发路由管理器
        /// </summary>
        /// <param name="communicator">转发路由管理器需管理的接口设备</param>
        public RouteManager(ICommunicator<T>[] communicator)
        {
            this._asynInvoke = new AsynchronousInvoke<StateObject<T>>("RtPr", 10000, 4, 4, 0, 1, new int[] { 0 });
            if (communicator != null)
            {
                for (int i = 0; i < communicator.Length; i++)
                {
                    this._communicators.Add(communicator[i].IPAddress, communicator[i]);
                    this._communicators[communicator[i].IPAddress].OnSendMessageToInner += new SendMessageDelegate<T>(this.RouteManager_OnSendMessageToInner);
                }
            }
        }

        private void RouteManager_OnSendMessageToInner(RoutePacket<T> packet)
        {
            if (packet != null)
            {
                for (int i = 0; i < packet.Destination.Length; i++)
                {
                    if (this._communicators.ContainsKey(packet.Destination[i]))
                    {
                        if (this._communicators[packet.Destination[i]].IsConnected)
                        {
                            if (packet.Destination[i] == "Log")
                            {
                                this._asynInvoke.Enqueue(new WorkItemCallback(this.SendMessage),
                                    new StateObject<T>()
                                    {
                                        Communicator = this._communicators[packet.Destination[i]],
                                        Packet = packet
                                    }, WorkItemPriority.Lowest);
                            }
                            else
                            {
                                this._asynInvoke.Enqueue(new WorkItemCallback(this.SendMessage),
                                    new StateObject<T>()
                                    {
                                        Communicator = this._communicators[packet.Destination[i]],
                                        Packet = packet
                                    }, WorkItemPriority.Normal);
                            }
                        }
                    }
                }
            }
        }

        private object SendMessage(object obj)
        {
            StateObject<T> sobj = obj as StateObject<T>;
            if (sobj != null)
            {
                try
                {
                    RoutePacket<T> packet = sobj.Packet;
                    return sobj.Communicator.SendMessageToClient(packet);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLog(String.Format("发送至通信器{{{0}}}时出现异常，异常信息：{1}", sobj.Communicator.IPAddress, ex.Message));
                }
            }
            return false;
        }

        /// <summary>
        /// 添加接口设备供转发路由管理器进行统一管理
        /// </summary>
        /// <param name="communicator">需由转发路由管理器管理的接口设备</param>
        public void AddCommunicator(ICommunicator<T> communicator)
        {
            try
            {
                communicator.OnSendMessageToInner += new SendMessageDelegate<T>(this.RouteManager_OnSendMessageToInner);
                this._communicators.Add(communicator.IPAddress, communicator);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLog("增加通讯器时出现异常，异常信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 启动转发路由管理器
        /// </summary>
        public void Start()
        {
            this._asynInvoke.Start();
            if (this._communicators.Count > 0)
            {
                foreach (KeyValuePair<string, ICommunicator<T>> pair in this._communicators)
                {
                    try
                    {
                        bool flag = pair.Value.Start();
                        if (!flag)
                        {
                            DebugHelper.WriteLog(String.Format("启动{0}失败", pair.Value.GetType().FullName));
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLog(String.Format("启动{0}发生异常，异常信息：{1}", pair.Value.GetType().FullName, ex.Message));
                    }
                }
            }
        }

        /// <summary>
        /// 停止转发路由管理器
        /// </summary>
        public void Stop()
        {
            this._asynInvoke.Stop();
            if (this._communicators.Count > 0)
            {
                foreach (KeyValuePair<string, ICommunicator<T>> pair in this._communicators)
                {
                    try
                    {
                        bool flag = pair.Value.Stop();
                        if (!flag)
                        {
                            DebugHelper.WriteLog(String.Format("停止{0}失败", pair.Value.GetType().FullName));
                        }
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteLog(String.Format("停止{0}发生异常，异常信息：{1}", pair.Value.GetType().FullName, ex.Message));
                    }
                }
            }
        }

        public void Dispose()
        {
            this.Stop();
        }
    }
}
