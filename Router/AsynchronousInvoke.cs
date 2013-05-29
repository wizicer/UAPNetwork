namespace IcerDesign.Router
{
    using System;
    using System.Threading;

    using Amib.Threading;

    using log4net;

    public delegate void LevelArrivedDelegate<T>(object sender, LevelArrivedEventArgs<T> e);

    /// <summary>
    /// 异步调用类（使用人工进程池排队管理）
    /// </summary>
    public class AsynchronousInvoke<T> : IDisposable
    {
        private static ILog Log = log4net.LogManager.GetLogger("Invoker");
        #region varibles
        private SmartThreadPool _threadPool;
        private Thread thWork;
        private SafeQueue<WAITQUEUESTRUCTURE> _queue = new SafeQueue<WAITQUEUESTRUCTURE>();
        private int[] _list;
        private bool _isRunning = false;

        private readonly int Interval;
        private readonly int MaxThreadsNumber;
        private readonly string _name;

#if DEBUG
        private DateTime _lastWriteTime = DateTime.Now;
#endif

        #endregion

        /// <summary>
        /// 异步调用类及相关线程的名字
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>
        /// 当达到一个警告等级时触发
        /// </summary>
        public event LevelArrivedDelegate<T> OnLevelArrived;

        /// <summary>
        /// 等待队列结构
        /// </summary>
        struct WAITQUEUESTRUCTURE
        {
            /// <summary>
            /// 队列中的事件处理句柄
            /// </summary>
            public WorkItemCallback Callback;
            /// <summary>
            /// 需要处理的数据包
            /// </summary>
            public T Packet;
        }

        #region Constructor

        /// <summary>
        /// 初始化异步调用类
        /// </summary>
        public AsynchronousInvoke()
            : this("", 10000, 4, 4, 5000, 10, new int[] { 0 })
        {
        }

        /// <summary>
        /// 初始化异步调用类
        /// </summary>
        /// <param name="maxThreadsNumber">最大线程数</param>
        /// <param name="levelNumbers">警告事件激活线程数量</param>
        public AsynchronousInvoke(int maxThreadsNumber, int[] levelNumbers)
            : this("", 10000, 4, 4, maxThreadsNumber, 10, levelNumbers)
        {
        }

        /// <summary>
        /// 初始化异步调用类
        /// </summary>
        /// <param name="name">异步调用类的名字</param>
        /// <param name="maxThreadsNumber">最大线程数</param>
        /// <param name="levelNumbers">警告事件激活线程数量</param>
        public AsynchronousInvoke(string name, int maxThreadsNumber, int[] levelNumbers)
            : this(name, 10000, 4, 4, maxThreadsNumber, 10, levelNumbers)
        {
        }

        /// <summary>
        /// 初始化异步调用类
        /// </summary>
        /// <param name="name">异步调用类的名字</param>
        /// <param name="idleTimeout">空闲超时时间</param>
        /// <param name="maxWorkerThreads">最大工作线程数</param>
        /// <param name="minWorkerThreads">最小工作线程数</param>
        /// <param name="maxThreadsNumber">最大线程数</param>
        /// <param name="interval">循环等待间隔</param>
        /// <param name="levelNumbers">警告事件激活线程数量</param>
        public AsynchronousInvoke(string name, int idleTimeout, int maxWorkerThreads, int minWorkerThreads, int maxThreadsNumber, int interval, int[] levelNumbers)
        {
            this.MaxThreadsNumber = maxThreadsNumber;
            this.Interval = interval;
            this._name = name;
            for (int i = 1; i < levelNumbers.Length; i++)
            {
                if (levelNumbers[i] <= levelNumbers[i - 1])
                {
                    throw new Exception("阶段数量需按从小到大排列。");
                }
            }
            // mark ,problem maybe here
            this._list = levelNumbers;
            this._threadPool = new SmartThreadPool(idleTimeout, maxWorkerThreads, minWorkerThreads);
            this._threadPool.Name = name + "的线程池";
        }
        #endregion

        /// <summary>
        /// 加入工作项
        /// </summary>
        /// <param name="callback">执行的方法</param>
        /// <param name="packet">方法的入参</param>
        public void Enqueue(WorkItemCallback callback, T packet)
        {
            WorkItemPriority priority = WorkItemPriority.Normal;
            this.Enqueue(callback, packet, priority);
        }

        /// <summary>
        /// 加入工作项
        /// </summary>
        /// <param name="callback">执行的方法</param>
        /// <param name="packet">方法的入参</param>
        /// <param name="priority">执行的优先级</param>
        public void Enqueue(WorkItemCallback callback, T packet, WorkItemPriority priority)
        {
            if (callback == null) return;
            int level = 0;
            int count = this._queue.Count;
            if (this._list[this._list.Length - 1] < count)
            {
                level = this._list.Length - 1;
            }
            else
            {
                for (int i = 0; i < this._list.Length - 1; i++)
                {
                    if (count > this._list[i] && count < this._list[i + 1])
                    {
                        level = i;
                        break;
                    }
                }
            }
            LevelArrivedEventArgs<T> eventArgs = new LevelArrivedEventArgs<T>() { CancelEnqueue = false, LevelNumber = level, Packet = packet };
            if (level > 0 && this.OnLevelArrived != null)
            {
                this.OnLevelArrived(this, eventArgs);
            }
            if (!eventArgs.CancelEnqueue)
            {
                if (this.MaxThreadsNumber == 0)
                {
                    this._threadPool.QueueWorkItem(callback, packet, priority);
#if DEBUG
                    bool flag = false;
                    if ((DateTime.Now - this._lastWriteTime).TotalSeconds > 1)
                    {
                        flag = true;
                        this._lastWriteTime = DateTime.Now;
                    }
                    if (flag)
                    {
                        Console.WriteLine(String.Format("[{2}]完成:{0,5:#####}|{1,-5:#####}", this._queue.Count, this._threadPool.WaitingCallbacks, this.Name));
                    }
#endif
                }
                else
                {
                    this._queue.Enqueue(new WAITQUEUESTRUCTURE() { Callback = callback, Packet = packet });
                }
            }
        }

        /// <summary>
        /// 启动异步调用类的排队功能
        /// </summary>
        public void Start()
        {
            if (this.MaxThreadsNumber > 0)
            {
                this._isRunning = true;
                this.thWork = new Thread(new ThreadStart(this.MessageLoopProcess));
                this.thWork.IsBackground = true;
                this.thWork.Start();
            }
        }

        /// <summary>
        /// 停止异步调用类
        /// </summary>
        public void Stop()
        {
            if (this.MaxThreadsNumber > 0)
            {
                this._isRunning = false;
                this.thWork.Abort();
            }
        }

        /// <summary>
        /// 循环处理信息
        /// </summary>
        private void MessageLoopProcess()
        {
            while (this._isRunning)
            {
                try
                {
#if DEBUG
                    bool flag = false;
                    if ((DateTime.Now - this._lastWriteTime).TotalSeconds > 1)
                    {
                        flag = true;
                        this._lastWriteTime = DateTime.Now;
                    }
#endif
                    while (this._queue.Count > 0 && this._threadPool.WaitingCallbacks + this._threadPool.InUseThreads < this.MaxThreadsNumber)
                    {
                        WAITQUEUESTRUCTURE response = this._queue.Dequeue();
                        this._threadPool.QueueWorkItem(response.Callback, response.Packet);
                    }
                    Thread.Sleep(this.Interval);
#if DEBUG
                    if (flag) Console.WriteLine(String.Format("[{2}]完成:{0,5:#####}|{1,-5:#####}", this._queue.Count, this._threadPool.WaitingCallbacks, this.Name));
#endif
                }
                catch (Exception ex)
                {
                    Log.Warn("人工缓冲区消息处理出现未知错误", ex);
                }
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            this.thWork = null;
        }
    }

    /// <summary>
    /// 警告等级达到的事件源参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LevelArrivedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 通过设置true来避免该项进入队列
        /// </summary>
        public bool CancelEnqueue { get; set; }
        /// <summary>
        /// 入参数据
        /// </summary>
        public T Packet { get; set; }
        /// <summary>
        /// 警告等级数字
        /// </summary>
        public int LevelNumber { get; set; }
    }
}
