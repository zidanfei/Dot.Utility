using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public interface IEntityBaseWithIdRepositoryBase<T, TContext> : IEntityFrameworkRepositoryBase<EntityBaseWithId<T>, TContext> 
        where TContext : DbContext, new()
    {
    }
}
