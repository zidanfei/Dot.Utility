using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace Dot.Utility.Security
{
    /// <summary>
    /// 线程安全的List
    /// </summary>
    public class ThreadSafeHashtable : IEnumerable
    {
        /// <summary>
        /// 线程安全控制.
        /// </summary>
        protected readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// 内部数据集合
        /// </summary>
        private Hashtable currentList;

        public ThreadSafeHashtable()
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.currentList = new Hashtable();
        }

        /// <summary>
        /// 删除元素-
        /// </summary>
        public void Remove(object key)
        {
            _lock.EnterWriteLock();
            try
            {
                if (currentList.Count > 0)
                {
                    currentList.Remove(key);
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
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(object key, object value)
        {
            _lock.EnterWriteLock();
            try
            {
                this.currentList.Add(key,value);
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

        public object this[object key]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList.Contains(key);
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
                    this.currentList[key]=value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public ICollection Keys
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList.Keys;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public ICollection Values
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList.Values;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public bool ContainKey(object key)
        {
            _lock.EnterReadLock();
            try
            {
                return this.currentList.Contains(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #region 扩展

        public void CopyTo(Array array, int arrayIndex)
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

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
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
    }
}
