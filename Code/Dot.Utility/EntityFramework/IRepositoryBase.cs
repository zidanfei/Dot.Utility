using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{
    public interface IRepositoryBase<T, TContext> : IEntityFrameworkRepositoryBase<T, TContext> where T : EntityBase,new()
        where TContext : DbContext, new()
    {
       
    }

    public interface IRepositoryBase<T,Tkey, TContext> : IEntityFrameworkRepositoryBase<T, TContext> where T : EntityBase<Tkey>, new()
       where TContext : DbContext, new()
    {

    }
}
