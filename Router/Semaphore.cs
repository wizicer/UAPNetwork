namespace IcerDesign.Router
{
    using System.Diagnostics;
    using System.Threading;

    /// <summary>
    /// 信号类
    /// </summary>
    public class Semaphore
    {
        private int _currentCount; //当前资源数
        private int _maxCount = 1; //最大资源数
        private object _syncObjWait = new object();
        private ManualResetEvent _waitEvent = new ManualResetEvent(false);

        /// <summary>
        /// 初始化信号类
        /// </summary>
        /// <param name="maxCount">最大资源数</param>
        public Semaphore(int maxCount)
        {
            this._maxCount = maxCount;
        }

        /// <summary>
        /// 等待资源
        /// </summary>
        /// <returns></returns>
        public bool Wait()
        {
            lock (this._syncObjWait) //只能一个线程进入下面代码
            {
                bool waitResult = this._waitEvent.WaitOne(); //在此等待资源数大于零
                if (waitResult)
                {
                    lock (this)
                    {
                        if (this._currentCount > 0)
                        {
                            this._currentCount--;
                            if (this._currentCount == 0)
                            {
                                this._waitEvent.Reset();
                            }
                        }
                        else
                        {
                            Debug.Assert(false, "Semaphore is not allow current count < 0");
                        }
                    }
                }
                return waitResult;
            }
        }

        /// <summary>
        /// 允许超时返回的 Wait 操作
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        /// <returns></returns>
        public bool Wait(int millisecondsTimeout)
        {
            lock (this._syncObjWait) // Monitor 确保该范围类代码在临界区内
            {
                bool waitResult = this._waitEvent.WaitOne(millisecondsTimeout, false);
                if (waitResult)
                {
                    lock (this)
                    {
                        if (this._currentCount > 0)
                        {
                            this._currentCount--;
                            if (this._currentCount == 0)
                            {
                                this._waitEvent.Reset();
                            }
                        }
                        else
                        {
                            Debug.Assert(false, "Semaphore is not allow current count < 0");
                        }
                    }
                }
                return waitResult;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <returns></returns>
        public bool Release()
        {
            lock (this) // Monitor 确保该范围类代码在临界区内
            {
                this._currentCount++;
                if (this._currentCount > this._maxCount)
                {
                    this._currentCount = this._maxCount;
                    return false;
                }
                this._waitEvent.Set(); //允许调用Wait的线程进入
            }
            return true;
        }
    }
}