using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public abstract class EntityFrameworkRepositoryBase<T, TContext> : IDisposable
        where T : class,new()
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

        public virtual IList<T> GetList()
        {
            return _objectSet.ToList();
        }

        public virtual IList<T> GetList(out int pageCount)
        {
            pageCount = _objectSet.Count();
            return _objectSet.ToList();
        }

        public virtual IQueryable<T> GetQueryList()
        {
            return _objectSet;
        }

        public virtual IQueryable<T> GetQueryList(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate);
        }

        public virtual IEnumerable<T> GetList(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).ToList();
        }

        public virtual IList<T> GetList(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize, out int pageCount, Func<T, string> orderby, Func<T, string> orderbyDescending)
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
            var count = expr.Count();
            pageCount = int.Parse((count / pageSize).ToString());
            pageCount = (count % pageSize != 0) ? pageCount + 1 : pageCount;

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

        public virtual int GetCount(Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).Count();
        }

        public virtual T Get(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return _objectSet.Where(predicate).FirstOrDefault();
        }

        public virtual void Add(T entity)
        {
            _objectSet.Add(entity);
        }

        public virtual void Update(T entity)
        {
            //_dbContext.Entry(entity).State = EntityState.Modified;
        }

        //public abstract void Update(T entity);

        public virtual void Delete(T entity)
        {
            _objectSet.Remove(entity);
        }
        /// <summary>
        /// 
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
