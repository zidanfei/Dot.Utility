using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dot.Utility.Security
{
    /// <summary>
    /// 支持插队的队列-符合FIFO
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ThreadSafeJumpQueue<T>
    {
        /// <summary>
        /// 线程安全控制.
        /// </summary>
        protected readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// 内部数据集合
        /// </summary>
        private IList<T> currentList;

        public ThreadSafeJumpQueue()
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.currentList = new List<T>();
        }

        /// <summary>
        /// 返回队列的第一个出队的元素，但是并不从队列中移出
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            _lock.EnterReadLock();
            try
            {
                return currentList.Count > 0 ? currentList[currentList.Count - 1] : default(T);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 入队-追加到队列末尾
        /// </summary>
        /// <param name="instance"></param>
        public void EnQueue(T instance)
        {
            _lock.EnterWriteLock();
            try
            {
                currentList.Add(instance);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 批量入队-追加到队列末尾
        /// </summary>
        /// <param name="instance"></param>
        public void EnQueue(IList<T> instances)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var instance in instances)
                {
                    currentList.Add(instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 批量入队-追加到队列末尾
        /// </summary>
        /// <param name="instance"></param>
        public void EnQueue(T[] instances)
        {
            _lock.EnterWriteLock();
            try
            {
                foreach (var instance in instances)
                {
                    currentList.Add(instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 出队-从队列的开头进行出队-第一个
        /// </summary>
        public T DeQueue()
        {
            _lock.EnterWriteLock();
            try
            {
                if (currentList.Count > 0)
                {
                    var tempT = currentList[0];
                    currentList.Remove(tempT);
                    return tempT;
                }
                return default(T);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 插队-插到列头
        /// </summary>
        /// <param name="instance">插队的元素</param>
        public void JumpQueue(T instance)
        {
            _lock.EnterWriteLock();
            try
            {
                this.currentList.Insert(0, instance);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 批量插队-插到列头-保持原集合的顺序
        /// </summary>
        /// <param name="instance">插队的元素</param>
        public void JumpQueue(T[] instances)
        {
            _lock.EnterWriteLock();
            try
            {
                for (int i = instances.Length - 1; i >= 0; i--)
                {
                    var instance = instances[i];
                    this.currentList.Insert(0, instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 批量插队-插到列头-保持原集合的顺序
        /// </summary>
        /// <param name="instance">插队的元素</param>
        public void JumpQueue(IList<T> instances)
        {
            _lock.EnterWriteLock();
            try
            {
                for (int i = instances.Count - 1; i >= 0; i--)
                {
                    var instance = instances[i];
                    this.currentList.Insert(0, instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 清空队列中的所有元素
        /// </summary>
        public void Clear()
        {
            _lock.EnterWriteLock();
            try
            {
                this.currentList.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 当前队列中的元素总数
        /// </summary>
        public int Count
        {
            get
            {
                _lock.EnterWriteLock();
                try
                {
                    return currentList.Count;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }
    }
}
