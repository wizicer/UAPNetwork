namespace IcerDesign.SocketLibrary
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    using log4net;

    public class StateObject
    {
        // Client socket. 
        public Socket workSocket = null;
        // Receive buffer. 
        public byte[] buffer = null;
        // Received data string. 
        public StringBuilder sb = new StringBuilder();
    }

    public class AsynchronousSocketListener : IDisposable
    {
        internal class DebugHelper
        {
            private static ILog log = log4net.LogManager.GetLogger("Socket");
            public static void WriteLog(string content)
            {
                log.Debug(content);
            }
        }
        Socket listener = null;

        private ArrayList Connections = null;

        /// <summary>
        /// 最大连接数
        /// </summary>
        private readonly int MaxConnection = 300;
        public ClientSocketDropDelegate ClientDropHandler { get; set; }
        public ClientSocketAcceptDelegate CLientAcceptHandler { get; set; }
        public int ReceiveBufferSize { private get; set; }
        public int Port { private get; set; }
        public int PendingConnectionCount { private get; set; }
        public int ReadBufferSize { private get; set; }
        public IMessageProcessor Processor { get; set; }
        // Thread signal. 
        private ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
            this.Connections = new ArrayList(this.MaxConnection);
        }

        public AsynchronousSocketListener(IMessageProcessor processor)
        {
            this.Connections = new ArrayList(this.MaxConnection);
            this.Processor = processor;
        }

        public void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, this.Port);
            // Create a TCP/IP socket. 
            this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.listener.ReceiveBufferSize = this.ReceiveBufferSize;
            // Bind the socket to the local endpoint and listen for incoming connections. 
            try
            {
                this.listener.Bind(localEndPoint);
                this.listener.Listen(this.PendingConnectionCount);
                while (true)
                {
                    // Set the event to nonsignaled state. 
                    this.allDone.Reset();
                    // Start an asynchronous socket to listen for connections. 
                    this.listener.BeginAccept(new AsyncCallback(this.AcceptCallback), this.listener);
                    // Wait until a connection is made before continuing. 
                    this.allDone.WaitOne();
                }
            }
            catch (SocketException se)
            {
                throw new SocketException(se.ErrorCode);
            }
            catch (Exception e)
            {
                DebugHelper.WriteLog("侦听过程中发生未知错误，错误信息：" + e.Message);
                DebugHelper.WriteLog(e.StackTrace);
            }
        }

        /// <summary>
        /// 服务端关闭
        /// 首先关闭服务端
        /// 然后关闭Connections中的客户端列表，避免内存泄露
        /// </summary>
        public void StopListener()
        {
            lock (this)
            {

                if (this.listener != null)
                {

                    try
                    {

                        //首先关闭服务端
                        this.listener.Close();

                        foreach (Socket client in this.Connections)
                        {
                            try
                            {
                                if (this.ClientDropHandler != null)
                                {
                                    this.ClientDropHandler(client);
                                }
                            }
                            catch (Exception ex)
                            {
                                DebugHelper.WriteLog(ex.Message);
                                DebugHelper.WriteLog(ex.StackTrace);
                            }

                            client.Shutdown(SocketShutdown.Both);
                            client.Close();
                        }

                        this.Connections.Clear();
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteLog(e.Message);
                        DebugHelper.WriteLog(e.StackTrace);
                    }
                }
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue. 
            this.allDone.Set();
            try
            {
                // Get the socket that handles the client request. 
                Socket listener = ar.AsyncState as Socket;
                Socket clientSocket = listener.EndAccept(ar);
                // Create the state object. 
                StateObject state = new StateObject();
                state.workSocket = clientSocket;
                state.buffer = new byte[this.ReadBufferSize];

                //Add this Socket to Connections Arraylist
                this.Connections.Add(clientSocket);

                if (this.CLientAcceptHandler != null)
                {
                    this.CLientAcceptHandler(clientSocket);
                }
                //if (clientSocket.Connected)
                //{
                //    clientSocket.BeginReceive(state.buffer, 0, ReadBufferSize, 0, new AsyncCallback(ReadCallback), state);
                //}
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLog("接受连接时出现错误，错误信息：" + ex.Message);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String str = String.Empty;
            // Retrieve the state object and the handler socket 
            // from the asynchronous state object. 
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            try
            {
                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    this.Processor.HandleMessage(state, bytesRead);

                    // Not all data received. Get more. 
                    handler.BeginReceive(state.buffer, 0, this.ReadBufferSize, 0, new AsyncCallback(this.ReadCallback), state);
                }
            }
            catch (Exception ex)
            {
                if (handler.Connected)
                {
                    DebugHelper.WriteLog("读取Socket信息出错，正在关闭连接，错误信息：" + ex.Message);
                    //handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                else
                {
                    DebugHelper.WriteLog("读取Socket信息出错，连接已被关闭，错误信息：" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 关闭和客户端的连接
        /// 首先调用业务层的逻辑
        /// 再执行底层的Socket关闭动作
        /// </summary>
        /// <param name="client"></param>
        public void DropClientSocket(Socket client)
        {
            //首先调用业务层的逻辑
            lock (this)
            {
                if (this.ClientDropHandler != null)
                {
                    this.ClientDropHandler(client);
                }

                //在执行底层的Socket关闭动作
                try
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();

                    if (this.Connections.Contains(client))
                        this.Connections.Remove(client);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteLog(ex.Message);
                    DebugHelper.WriteLog(ex.StackTrace);
                }
            }
        }

        public void Dispose()
        {
            this.StopListener();
        }

    }
}