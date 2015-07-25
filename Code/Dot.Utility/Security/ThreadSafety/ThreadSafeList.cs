using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dot.Utility.Security
{
    /// <summary>
    /// 线程安全的List
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ThreadSafeList<T> : IList<T>
    {
        /// <summary>
        /// 线程安全控制.
        /// </summary>
        protected readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// 内部数据集合
        /// </summary>
        private IList<T> currentList;

        public ThreadSafeList()
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.currentList = new List<T>();
        }

        /// <summary>
        /// 删除元素-
        /// </summary>
        public void Remove(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                if (currentList.Count > 0)
                {
                    currentList.RemoveAt(index);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 删除元素
        /// </summary>
        public void Remove(T instance)
        {
            _lock.EnterWriteLock();
            try
            {
                if (currentList.Count > 0)
                {
                    currentList.Remove(instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="instance">元素</param>
        public void Add(T instance)
        {
            _lock.EnterWriteLock();
            try
            {
                this.currentList.Add(instance);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素集合
        /// </summary>
        /// <param name="instance">元素集合</param>
        public void Add(T[] instances)
        {
            _lock.EnterWriteLock();
            try
            {
                for (int i = instances.Length - 1; i >= 0; i--)
                {
                    var instance = instances[i];
                    this.currentList.Add(instance);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素集合
        /// </summary>
        /// <param name="instance">元素集合</param>
        public void Add(IList<T> instances)
        {
            _lock.EnterWriteLock();
            try
            {
                for (int i = instances.Count - 1; i >= 0; i--)
                {
                    var instance = instances[i];
                    this.currentList.Add(instance);
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

        #region IList<T> 成员

        public int IndexOf(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return this.currentList.IndexOf(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Insert(int index, T item)
        {
            _lock.EnterWriteLock();
            try
            {
                currentList.Insert(index, item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index)
        {
            _lock.EnterWriteLock();
            try
            {
                currentList.RemoveAt(index);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T this[int index]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList[index];
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    currentList[index] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        #endregion

        #region ICollection<T> 成员


        public bool Contains(T item)
        {
            _lock.EnterReadLock();
            try
            {
                return this.currentList.Contains(item);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _lock.EnterWriteLock();
            try
            {
                currentList.CopyTo(array, arrayIndex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList.IsReadOnly;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        bool ICollection<T>.Remove(T item)
        {
            _lock.EnterWriteLock();
            try
            {
                return currentList.Remove(item);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #endregion

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return this.currentList.GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return this.GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion
    }
}
