using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public abstract class EntityFrameworkRepositoryBase<T, TContext> : IEntityFrameworkRepositoryBase<T, TContext>, IDisposable
        where T : class, new()
        where TContext : DbContext, new()
    {
        public EntityFrameworkRepositoryBase(TContext context)
        {
            this._dbContext = context;
            this._objectSet = this._dbContext.Set<T>();
        }

        public EntityFrameworkRepositoryBase()
            : this(new TContext())
        {
        }
        protected TContext _dbContext;

        public TContext DBContext
        {
            get { return _dbContext; }
            set { _dbContext = value; }
        }

        protected DbSet<T> _objectSet;

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <returns></returns>
        public virtual IList<T> GetList()
        {
            return _objectSet.ToList();
        }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="pageCount">The page count.</param>
        /// <returns></returns>
        public virtual IList<T> GetList(out int pageCount)
        {
            pageCount = _objectSet.Count();
            return _objectSet.ToList();
        }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual IList<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).ToList();
        }

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryList()
        {
            return _objectSet;
        }

        /// <summary>
        /// 获取多条数据
        /// 懒查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate);
        }

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
        public virtual IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize,
            Func<T, string> orderby, Func<T, string> orderbyDescending, string include = null)
        {

            if (pageIndex <= 0)
                throw new ArgumentException("pageIndex");
            if (pageSize <= 0)
                throw new ArgumentException("pageSize");
            IQueryable<T> expr = _objectSet;
            if (predicate != null)
            {
                expr = expr.Where(predicate);
            }
            if (!string.IsNullOrEmpty(include))
            {
                expr = expr.Where(predicate).Include(include);
            }
            int startn = (pageIndex - 1) * pageSize;
            if (orderby != null)
            {
                return expr.OrderBy(orderby).Skip(startn).Take(pageSize).AsQueryable<T>();
            }
            else if (orderbyDescending != null)
            {
                return expr.OrderByDescending(orderbyDescending).Skip(startn).Take(pageSize).AsQueryable<T>();
            }
            else
            {
                return expr.Skip(startn).Take(pageSize).AsQueryable<T>();
            }
        }


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
        public virtual IList<T> GetList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int total,
            out int pageCount, Func<T, string> orderby, Func<T, string> orderbyDescending, string include = null)
        {

            if (pageIndex <= 0)
                throw new ArgumentException("pageIndex");
            if (pageSize <= 0)
                throw new ArgumentException("pageSize");
            IQueryable<T> expr = _objectSet;
            if (predicate != null)
            {
                expr = expr.Where(predicate);
            }
            total = expr.Count();
            if (!string.IsNullOrEmpty(include))
            {
                expr = expr.Include(include);
            }
            pageCount = int.Parse((total / pageSize).ToString());
            pageCount = (total % pageSize != 0) ? pageCount + 1 : pageCount;

            int startn = (pageIndex - 1) * pageSize;
            if (orderby != null)
            {
                return expr.OrderBy(orderby).Skip(startn).Take(pageSize).ToList();
            }
            else if (orderbyDescending != null)
            {
                return expr.OrderByDescending(orderbyDescending).Skip(startn).Take(pageSize).ToList();
            }
            else
            {
                return expr.Skip(startn).Take(pageSize).ToList();
            }
        }

        /// <summary>
        /// 获取多条数据
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderby">正序条件</param>
        /// <param name="orderbyDescending">降序条件</param>
        /// <returns></returns>
        public virtual IList<T> GetList(Expression<Func<T, bool>> predicate, Func<T, string> orderby, Func<T, string> orderbyDescending)
        {

            IQueryable<T> expr = _objectSet;
            if (predicate != null)
            {
                expr = expr.Where(predicate);
            }
            if (orderby != null)
            {
                return expr.OrderBy(orderby).ToList();
            }
            else if (orderbyDescending != null)
            {
                return expr.OrderByDescending(orderbyDescending).ToList();
            }
            else
            {
                return expr.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlstr"></param>
        /// <param name="orderby">排序列</param>
        /// <param name="pageIndex">从1开始</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<TOut> GetPaging<TOut>(string sqlstr, string orderby, int pageIndex, int pageSize, params object[] parameters)
        {
            int start = 1;
            int end = 10;
            if (pageIndex > 0 && pageSize > 0)
            {
                start = pageSize * (pageIndex - 1) + 1;
                end = pageSize * ((pageIndex - 1) + 1);
            }
            Regex order = new Regex("-(asc|desc)$", RegexOptions.IgnoreCase);
            if (order.IsMatch(orderby))
            {
                orderby = order.Replace(orderby, " $1");
            }
            string sql = string.Format(@"
                     SELECT  *  FROM 
                     ( SELECT  ROW_NUMBER()  OVER  ( ORDER BY {1} )  AS RowNumber,  *  
                     FROM 
                     ({0} ) A_
                     )
                     AS A1_
                     WHERE RowNumber BETWEEN  {2} AND {3} ORDER BY {1}  ;"
                     , sqlstr, orderby, start, end
                );
            if (parameters.Count() == 0)
                return _dbContext.Database.SqlQuery<TOut>(sql);
            return _dbContext.Database.SqlQuery<TOut>(sql, parameters);
        }

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).Count();
        }

        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual object GetMax(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector)
        {
            return _objectSet.Where(predicate).Max(selector);
        }

        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual object GetMin(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> selector)
        {
            return _objectSet.Where(predicate).Min(selector);
        }

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <returns></returns>
        public virtual int GetCount()
        {
            return _objectSet.Count();
        }

        /// <summary>
        /// 获取某个实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns></returns>
        public virtual T Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// 插入实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Add(T entity)
        {
            _objectSet.Add(entity);
        }

        /// <summary>
        /// 更新实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Update(T entity)
        {

            //_dbContext.Entry(entity).State = EntityState.Modified;
        }


        /// <summary>
        /// 删除实体
        /// 保存到数据库，请调用Save方法
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(T entity)
        {
            _objectSet.Remove(entity);
        }


        /// <summary>
        /// 保存所有变更。
        /// </summary>
        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
