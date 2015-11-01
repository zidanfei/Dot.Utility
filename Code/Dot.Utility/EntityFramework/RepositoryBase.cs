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
}
