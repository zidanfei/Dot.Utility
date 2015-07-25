using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Dot.Utility.Security
{
    /// <summary>
    /// Key,Value1,Value2 存储集合-存在映射关系
    /// </summary>
    /// <typeparam name="TK">键</typeparam>
    /// <typeparam name="TV1">值1</typeparam>
    /// <typeparam name="TV2">值2</typeparam>
    public class ThreadSafeThreeDictionary<TK, TV1, TV2>
    {
        /// <summary>
        /// 线程安全控制.
        /// </summary>
        protected readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// 内部数据集合-TV1
        /// </summary>
        private IDictionary<TK, TV1> currentList1;
        /// <summary>
        /// 内部数据集合-TV2
        /// </summary>
        private IDictionary<TK, TV2> currentList2;

        public ThreadSafeThreeDictionary()
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.currentList1 = new Dictionary<TK, TV1>();
            this.currentList2 = new Dictionary<TK, TV2>();
        }

        /// <summary>
        /// 删除元素-根据指定的键删除键映射的所有值信息
        /// </summary>
        public void Remove(TK key)
        {
            _lock.EnterWriteLock();
            try
            {
                if (currentList1.Count > 0)
                {
                    currentList1.Remove(key);
                }

                if (currentList2.Count > 0)
                {
                    currentList2.Remove(key);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素-KV1-如果存在,则将指定的键映射的值替换为新值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TK key, TV1 value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!this.currentList1.ContainsKey(key))
                    this.currentList1.Add(key, value);
                else
                    this.currentList1[key] = value;

                if (!this.currentList2.ContainsKey(key))
                    this.currentList2.Add(key, default(TV2));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素-KV2-如果存在,则将指定的键映射的值替换为新值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(TK key, TV2 value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!this.currentList2.ContainsKey(key))
                    this.currentList2.Add(key, value);
                else
                    this.currentList2[key] = value;

                if (!this.currentList1.ContainsKey(key))
                    this.currentList1.Add(key, default(TV1));
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 添加元素集合
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value1">值</param>
        /// <param name="value2">值</param>
        public void Add(TK key, TV1 value1, TV2 value2)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!this.currentList1.ContainsKey(key))
                    this.currentList1.Add(key, value1);
                else
                    this.currentList1[key] = value1;

                if (!this.currentList2.ContainsKey(key))
                    this.currentList2.Add(key, value2);
                else
                    this.currentList2[key] = value2;
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
                this.currentList1.Clear();
                this.currentList2.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool ContainsKey(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                return this.currentList1.ContainsKey(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 获取前一个映射集合
        /// </summary>
        public IDictionary<TK, TV1> Values1
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return currentList1;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 获取后一个映射集合
        /// </summary>
        public IDictionary<TK, TV2> Values2
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return currentList2;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 设置映射1的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetValue(TK key, TV1 value)
        {
            _lock.EnterReadLock();
            try
            {
                if (!this.currentList1.ContainsKey(key))
                    this.currentList1.Add(key, value);
                else
                    this.currentList1[key] = value;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public TV1 GetValue1(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                if (this.currentList1.ContainsKey(key))
                    return currentList1[key];
                return default(TV1);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// 设置映射1的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SetValue(TK key, TV2 value)
        {
            _lock.EnterReadLock();
            try
            {
                if (!this.currentList2.ContainsKey(key))
                    this.currentList2.Add(key, value);
                else
                    this.currentList2[key] = value;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public TV2 GetValue2(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                if (this.currentList2.ContainsKey(key))
                    return currentList2[key];
                return default(TV2);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return this.currentList1.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
    }
}
