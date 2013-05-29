namespace IcerDesign.Router
{
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// 线程安全的队列
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    public class SafeQueue<T>
    {
        #region 私有变量

        private Queue<T> _queue;
        private ReaderWriterLock _lock;

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SafeQueue()
        {
            this._queue = new Queue<T>();
            this._lock = new ReaderWriterLock();
        }

        /// <summary>
        /// 提供初始内容的构造函数
        /// </summary>
        /// <param name="collection">队列初始内容</param>
        public SafeQueue(IEnumerable<T> collection)
        {
            this._queue = new Queue<T>(collection);
            this._lock = new ReaderWriterLock();
        }

        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            try
            {
                this._lock.AcquireWriterLock(Timeout.Infinite);
                this._queue.Enqueue(item);
            }
            finally
            {
                this._lock.ReleaseLock();
            }
        }

        /// <summary>
        /// 出列
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            try
            {
                this._lock.AcquireWriterLock(Timeout.Infinite);
                if (this._queue.Count > 0)
                {
                    T item = this._queue.Dequeue();
                    return item;
                }

                return default(T);
            }
            finally
            {
                this._lock.ReleaseLock();
            }
        }

        /// <summary>
        /// 非安全出列
        /// </summary>
        /// <returns></returns>
        public T UnsafeDequeue()
        {
            if (this._queue.Count > 0)
            {
                T item = this._queue.Dequeue();
                return item;
            }

            return default(T);
        }

        /// <summary>
        /// 释放队列多余空间
        /// </summary>
        public void TrimExcess()
        {
            this._queue.TrimExcess();
        }

        /// <summary>
        /// 队列长度
        /// </summary>
        public int Count
        {
            get { return this._queue.Count; }
        }

        /// <summary>
        /// 获取队列头数据，而不移除他
        /// </summary>
        public T Peek()
        {
            return this._queue.Peek();
        }

        /// <summary>
        /// 锁定
        /// </summary>
        public void Lock()
        {
            this._lock.AcquireWriterLock(Timeout.Infinite);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void Unlock()
        {
            this._lock.ReleaseLock();
        }
    }
}
