using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public interface IEntityFrameworkRepositoryBase<T, TContext>
        where T : class, new()
        where TContext : DbContext, new()
    {
        TContext DBContext { get; set; }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <returns></returns>
        IList<T> GetList();

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="pageCount">The page count.</param>
        /// <returns></returns>
        IList<T> GetList(out int pageCount);

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        IList<T> GetList(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetQueryList();

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pageIndex">当前页面</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="orderby">正序条件</param>
        /// <param name="orderbyDescending">降序条件</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// pageIndex
        /// or
        /// pageSize
        /// </exception>
        /// <returns></returns>
        //IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, Func<T, string> orderby, Func<T, string> orderbyDescending);

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// pageIndex
        /// or
        /// pageSize
        /// </exception>
        /// <returns></returns>
        IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// <param name="predicate">查询条件</param>
        /// <param name="pageIndex">当前页面</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">总数</param>
        /// <param name="pageCount">分页个数</param>
        /// <param name="orderby">正序条件</param>
        /// <param name="orderbyDescending">降序条件</param>
        /// <exception cref="System.ArgumentException">
        /// pageIndex
        /// or
        /// pageSize
        /// </exception> 
        IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, Func<T, object> orderby, Func<T, object> orderbyDescending, string include = null);


        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pageIndex">当前页面</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="total">总数</param>
        /// <param name="pageCount">分页个数</param>
        /// <param name="orderby">正序条件</param>
        /// <param name="orderbyDescending">降序条件</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">
        /// pageIndex
        /// or
        /// pageSize
        /// </exception>
        IList<T> GetList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total, out int pageCount, Func<T, object> orderby, Func<T, object> orderbyDescending, string include = null);

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderby">正序条件</param>
        /// <param name="orderbyDescending">降序条件</param>
        /// <returns></returns>
        IList<T> GetList(Expression<Func<T, bool>> predicate, Func<T, object> orderby, Func<T, object> orderbyDescending);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlstr"></param>
        /// <param name="orderby">排序列</param>
        /// <param name="pageIndex">从1开始</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<TOut> GetPaging<TOut>(string sqlstr, string orderby, int pageIndex, int pageSize, params object[] parameters);

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int GetCount(string sqlstr, params object[] parameters);

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        int GetCount(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <returns></returns>
        int GetCount();

        /// <summary>
        /// 获取某个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        T Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 插入实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Add(T entity);

        /// <summary>
        /// 更新实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(T entity);


        /// <summary>
        /// 删除实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(T entity);

        /// <summary>
        /// 保存所有变更。
        /// </summary>
        void Save();

    }
}
