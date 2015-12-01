using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public class EFDbContext<TEntity> : DbContext
    {
        //public IQueryable<TEntity> OrderBy(Action<IOrderable<TEntity>> orderBy) 
        //{
        //    var linq = new Orderable<TEntity>(this );
        //    orderBy(linq);
        //    return linq.Queryable;
        //}
    }
}
