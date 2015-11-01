using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Demo
{
    public interface IProjectService : Dot.Utility.EntityFramework.IEntityFrameworkRepositoryBase<Project, DBContext>
    {
    }
}
