
using Dot.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Adapter
{
    public static class TypeAdapterFactory
    {
        #region Members

        private static ITypeAdapterFactory _currentTypeAdapterFactory = null;
        private static readonly object lock_flag = new object();
        #endregion

        #region Public Static Methods

        /// <summary>
        /// Set the current type adapter factory
        /// </summary>
        /// <param name="adapterFactory">The adapter factory to set</param>
        public static void SetCurrent(ITypeAdapterFactory adapterFactory)
        {
            _currentTypeAdapterFactory = adapterFactory;
        }
        /// <summary>
        /// Create a new type adapter from currect factory
        /// </summary>
        /// <returns>Created type adapter</returns>
        public static ITypeAdapter CreateAdapter()
        {
            if (_currentTypeAdapterFactory == null)
                lock (lock_flag)
                {
                    if (_currentTypeAdapterFactory == null)
                        _currentTypeAdapterFactory = ObjectContainer.CreateInstance<ITypeAdapterFactory>();
                }

            return _currentTypeAdapterFactory.Create();
        }
        #endregion
    }
}
