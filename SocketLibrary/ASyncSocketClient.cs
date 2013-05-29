namespace IcerDesign.SocketLibrary
{
    using System;
    using System.Net.Sockets;

    /// <summary>
    /// Socket class for asynchronous connection.
    /// </summary>
    public class AsyncSocketClient : IDisposable
    {
        #region delegates

        /// <summary>
        /// Called when a message is received.
        /// </summary>
        /// <param name="asyncSocketClient">
        /// The AsyncSocketClient to receive messages from.
        /// </param>
        public delegate void MessageHandler(AsyncSocketClient asyncSocketClient);

        /// <summary>
        /// Called when a connection is closed.
        /// </summary>
        /// <param name="asyncSocketClient">
        /// The AsyncSocketClient to receive messages from.
        /// </param>
        public delegate void SocketClosingHandler(
            AsyncSocketClient asyncSocketClient);

        /// <summary>
        /// Called when a socket error occurs.
        /// </summary>
        /// <param name="asyncSocketClient">
        /// The AsyncSocketClient to receive messages from.
        /// </param>
        /// <param name="exception">
        /// The exception that generated the error.
        /// </param>
        public delegate void ErrorHandler(
            AsyncSocketClient asyncSocketClient, Exception exception);

        #endregion delegates
        //one MB
        private const int BUFFER_SIZE = 1048576;

        #region private fields

        private NetworkStream _NetworkStream;
        private TcpClient _TcpClient;
        private AsyncCallback _CallbackReadMethod;
        private AsyncCallback _CallbackWriteMethod;
        private MessageHandler _MessageHandler;
        private SocketClosingHandler _SocketCloseHandler;
        private ErrorHandler _ErrorHandler;
        private bool _IsDisposed;
        private string _ServerIPAddress;
        private Int16 _ServerPort;
        private object _StateObject;
        private byte[] _Buffer;
        private int _BufferSize;// add by xuelp
        //private int clientBufferSize;

        #endregion private fields

        #region properties

        /// <summary>
        /// The server IP address to connect to.
        /// </summary>
        public string ServerIPAddress
        {
            get
            {
                return this._ServerIPAddress;
            }
        }

        public bool Connected
        {
            get
            {
                return this._TcpClient == null ? false : this._TcpClient.Connected;
            }
        }

        /// <summary>
        /// The server port to connect to.
        /// </summary>
        public Int16 ServerPort
        {
            get
            {
                return this._ServerPort;
            }
        }

        /// <summary>
        /// A user set state object to associate some state with a connection.
        /// </summary>
        public object StateObject
        {
            get
            {
                return this._StateObject;
            }
            set
            {
                this._StateObject = value;
            }
        }

        /// <summary>
        /// Buffer to hold data coming in from the socket.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this._Buffer;
            }
        }

        public int BufferSize
        {
            get
            {
                return this._BufferSize;
            }
            set
            {
                this._BufferSize = value;
            }
        }

        #endregion properties

        /// <summary>
        /// Constructs an AsyncSocketClient.
        /// </summary>
        /// <param name="bufferSize">The size of the receive buffer.</param>
        /// <param name="stateObject">The object to use for sending state
        /// information.</param>
        /// <param name="msgHandler">The user-defined message handling method.
        /// </param>
        /// <param name="closingHandler">The user-defined socket closing
        /// handling method.</param>
        /// <param name="errHandler">The user defined error handling method.
        /// </param>
        public AsyncSocketClient(Int32 bufferSize, object stateObject,
                                 MessageHandler msgHandler, SocketClosingHandler closingHandler,
                                 ErrorHandler errHandler)
        {
            //allocate buffer
            //			clientBufferSize = bufferSize;
            //			_Buffer = new byte[clientBufferSize];

            this._Buffer = new byte[BUFFER_SIZE];
            this._BufferSize = 0;
            this._StateObject = stateObject;

            //set handlers
            this._MessageHandler = msgHandler;
            this._SocketCloseHandler = closingHandler;
            this._ErrorHandler = errHandler;

            //set the asynchronous method handlers
            this._CallbackReadMethod = new AsyncCallback(this.ReceiveComplete);
            this._CallbackWriteMethod = new AsyncCallback(this.SendComplete);

            //haven't been disposed yet
            this._IsDisposed = false;
        }

        /// <summary>
        /// Finalizer method.  If Dispose() is called correctly, there is nothing
        /// for this to do.
        /// </summary>
        ~AsyncSocketClient()
        {
            if (!this._IsDisposed)
            {
                this.Dispose();
            }
        }

        #region public methods

        /// <summary>
        /// Sets the disposed flag to true and disconnects the socket.
        /// </summary>
        public void Dispose()
        {
            try
            {
                this._IsDisposed = true;
                this.Disconnect();
            }
            catch
            { }
        }

        /// <summary>
        /// Connects the socket to the given IP address and port.
        /// This also calls Receive().
        /// </summary>
        /// <param name="IPAddress">The IP address of the server.</param>
        /// <param name="port">The port to connect to.</param>
        public void Connect(String IPAddress, Int16 port)
        {
            try
            {
                //do we already have an open connection?
                if (this._NetworkStream == null)
                {
                    this._ServerIPAddress = IPAddress;
                    this._ServerPort = port;

                    //attempt to establish the connection
                    this._TcpClient = new TcpClient(this._ServerIPAddress, this._ServerPort);
                    this._NetworkStream = this._TcpClient.GetStream();

                    //set some socket options
                    this._TcpClient.ReceiveBufferSize = BUFFER_SIZE;
                    this._TcpClient.SendBufferSize = BUFFER_SIZE;
                    this._TcpClient.NoDelay = true;
                    //if the connection is dropped, drop all associated data
                    this._TcpClient.LingerState = new LingerOption(false, 0);

                    //start receiving messages
                    this.Receive();
                }
            }
            catch (SocketException se)
            {
                //the connect failed..pass the word on
                throw new SocketException(se.ErrorCode);
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message, exc.InnerException);
            }
        }

        ///<summary>
        /// Disconnects from the server.
        /// </summary>
        public void Disconnect()
        {
            //close down the connection, making sure it exists first
            if (this._NetworkStream != null)
            {
                this._NetworkStream.Close();
            }
            if (this._TcpClient != null)
            {
                this._TcpClient.Close();
            }

            //prep for garbage collection-we may want to use this instance again
            this._NetworkStream = null;
            this._TcpClient = null;

        }

        ///<summary>
        /// Asynchronously sends data across the socket.
        /// </summary>
        /// <param name="buffer">
        /// The buffer of data to send.
        /// </param>
        public void Send(byte[] buffer)
        {
            //send the data; don't worry about receiving any state information
            //back;
            try
            {
                if (this._NetworkStream != null && this._NetworkStream.CanWrite)
                {
                    this._NetworkStream.BeginWrite(
                        buffer, 0, buffer.Length, this._CallbackWriteMethod, null);
                }
                else
                {
                    throw new Exception("Socket is closed, cannot Send().");
                }
            }
            catch 
            {
                //DebugHelper.WriteLog("Send() failed, exception: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Asynchronously receives data from the socket.
        /// </summary>
        public void Receive()
        {
            try
            {
                if (this._NetworkStream != null && this._NetworkStream.CanRead)
                {
                    //_Buffer = new byte[clientBufferSize];
                    //_Buffer = new byte[BUFFER_SIZE];

                    this._NetworkStream.BeginRead(
                        this._Buffer, this._BufferSize, this._Buffer.Length - this._BufferSize, this._CallbackReadMethod, null);
                }
                else
                {
                    throw new Exception("Socket is closed, cannot Receive().");
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion public methods

        #region private methods

        /// <summary>
        /// Callback method called by the NetworkStream's thread when a message
        /// is sent.
        /// </summary>
        /// <param name="state">The state object holding information about
        /// the connection.</param>
        private void SendComplete(IAsyncResult state)
        {
            try
            {
                //check to be sure the network stream is valid before writing
                if (this._NetworkStream.CanWrite)
                {
                    this._NetworkStream.EndWrite(state);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Callback method called by the NetworkStream's thread when a message
        /// arrives.
        /// </summary>
        /// <param name="state">The state object holding information about
        /// the connection.</param>
        private void ReceiveComplete(IAsyncResult state)
        {
            try
            {
                //check the stream to be sure it is valid
                if (this._NetworkStream.CanRead)
                {
                    this._BufferSize += this._NetworkStream.EndRead(state);

                    //if there are bytes to process, do so.  Otherwise, the
                    //connection has been lost, so clean it up
                    if (this._BufferSize > 0)
                    {
                        try
                        {
                            //send the incoming message to the message handler
                            this._MessageHandler(this);
                        }
                        finally
                        {
                            //start listening again
                            this.Receive();
                        }
                    }
                }
            }
            catch
            {
                //the connection has been dropped so call the CloseHandler
                try
                {
                    if (this._SocketCloseHandler != null)
                    {
                        this._SocketCloseHandler(this);
                    }
                    else
                    {
                        this.Disconnect();
                    }
                }
                finally
                {
                    this.Dispose();
                }
            }
        }
        #endregion private methods
    }
}