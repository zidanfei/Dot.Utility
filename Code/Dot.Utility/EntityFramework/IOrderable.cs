using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    /// <summary>
    /// 排序规范
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrderable<T>
    {

        /// <summary>
        /// 递增
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        /// <returns></returns>
        IOrderable<T> Asc<TKey>(global::System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 然后递增
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IOrderable<T> ThenAsc<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 递减
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IOrderable<T> Desc<TKey>(global::System.Linq.Expressions.Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 然后递减
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IOrderable<T> ThenDesc<TKey>(Expression<Func<T, TKey>> keySelector);
        /// <summary>
        /// 排序后的结果集
        /// </summary>
        global::System.Linq.IQueryable<T> Queryable { get; }
    }
}
