using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public class RepositoryBase<T, TContext> :   EntityFrameworkRepositoryBase<T, TContext> ,IRepositoryBase<T, TContext>, IDisposable
         where T : EntityBase,new()
         where TContext : DbContext, new()
    {
        public override void Add(T entity)
        {
            entity.CreatedOn = DateTime.Now;
            base.Add(entity);
        }

        public override void Update(T entity)
        {
            entity.UpdatedOn = DateTime.Now;
            base.Update(entity);
        }
    }

    public class RepositoryBase<T,Tkey, TContext> : EntityFrameworkRepositoryBase<T,TContext>, IRepositoryBase<T, Tkey, TContext>, IDisposable
        where T : EntityBase< Tkey>, new()
        where TContext : DbContext, new()
    {
        public override void Add(T entity)
        {
            entity.CreatedOn = DateTime.Now;
            base.Add(entity);
        }

        public override void Update(T entity)
        {
            entity.UpdatedOn = DateTime.Now;
            base.Update(entity);
        }
    }
}
