using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dot.Utility.Security
{
    /// <summary>
    /// 线程安全字典类
    /// </summary>
    /// <typeparam name="TK"></typeparam>
    /// <typeparam name="TV"></typeparam>
    public class ThreadSafeDictionary<TK, TV> : IEnumerable<KeyValuePair<TK, TV>>
    {
        public Action<TK, TV> OnAddAction;

        /// <summary>
        /// Gets/adds/replaces an item by key.
        /// </summary>
        /// <param name="key">Key to get/set value</param>
        /// <returns>Item associated with this key</returns>
        public TV this[TK key]
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.ContainsKey(key) ? _items[key] : default(TV);
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
                    _items[key] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// Gets count of items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Count;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Internal collection to store items.
        /// </summary>
        protected readonly IDictionary<TK, TV> _items;

        /// <summary>
        /// Used to synchronize access to _items list.
        /// </summary>
        protected readonly ReaderWriterLockSlim _lock;

        /// <summary>
        /// Creates a new ThreadSafeSortedList object.
        /// </summary>
        public ThreadSafeDictionary()
        {
            _items = new Dictionary<TK, TV>();
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        /// Checks if collection contains spesified key.
        /// </summary>
        /// <param name="key">Key to check</param>
        /// <returns>True; if collection contains given key</returns>
        public bool ContainsKey(TK key)
        {
            _lock.EnterReadLock();
            try
            {
                return _items.ContainsKey(key);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Removes an item from collection.
        /// </summary>
        /// <param name="key">Key of item to remove</param>
        public bool Remove(TK key)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!_items.ContainsKey(key))
                {
                    return false;
                }

                _items.Remove(key);
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

       /// <summary>
       /// Add 
       /// </summary>
       /// <param name="key"></param>
       /// <param name="value"></param>
       /// <returns></returns>
        public bool Add(TK key,TV value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (!_items.ContainsKey(key))
                {
                    _items.Add(key, value);
                }

                if (OnAddAction != null)
                    OnAddAction(key, value);

                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets all items in collection.
        /// </summary>
        /// <returns>Item list</returns>
        public List<TV> GetAllItems()
        {
            _lock.EnterReadLock();
            try
            {
                return new List<TV>(_items.Values);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Removes all items from list.
        /// </summary>
        public void ClearAll()
        {
            _lock.EnterWriteLock();
            try
            {
                _items.Clear();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets then removes all items in collection.
        /// </summary>
        /// <returns>Item list</returns>
        public List<TV> GetAndClearAllItems()
        {
            _lock.EnterWriteLock();
            try
            {
                var list = new List<TV>(_items.Values);
                _items.Clear();
                return list;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #region IEnumerable<KeyValuePair<TK,TV>> 成员

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            _lock.EnterReadLock();
            try
            {
                return _items.GetEnumerator();
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
                return _items.GetEnumerator();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        #endregion

        public ICollection<TK> Keys
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Keys;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public ICollection<TV> Values
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _items.Values;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }
    }
}
