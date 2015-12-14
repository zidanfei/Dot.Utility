using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public class EntityBaseWithIdRepositoryBase<T, TContext> :
        EntityFrameworkRepositoryBase<EntityBaseWithId<T>, TContext> , IEntityBaseWithIdRepositoryBase<T, TContext>
         where TContext : DbContext, new()
    {
        public EntityBaseWithIdRepositoryBase():base()
        {

        }

        public EntityBaseWithIdRepositoryBase(TContext context) : base(context)
        {

        }
    }
}
