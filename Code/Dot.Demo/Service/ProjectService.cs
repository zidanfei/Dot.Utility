using Dot.IOC;
using Dot.Utility.EntityFramework;
using Dot.Utility.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Demo
{
    [Export(typeof(IProjectService))]
    public class ProjectService : EntityFrameworkRepositoryBase<Project, DBContext>, IProjectService
    {
        public ProjectService()
        {

        }
        public ProjectService(DBContext dbcontext)
            : base(dbcontext)
        {
        }


        public override void Add(Project entity)
        {
            ValidateUpdateProject(entity);
            entity.CreatedOn = DateTime.Now;
            base.Add(entity);
        }

        public override void Update(Project entity)
        {
            ValidateUpdateProject(entity);
            entity.UpdatedOn = DateTime.Now;
            base.Update(entity);
        }
 

        /// <summary>
        /// 校验项目
        /// </summary>
        /// <param name="entity"></param>
        void ValidateUpdateProject(Project entity)
        {
            entity.ProjectName = entity.ProjectName.Trim();
            var count = GetCount(m => m.ProjectName == entity.ProjectName && m.Id != entity.Id);
            if (count > 0)
            {
                throw new ValidationException("已存在相同项目名称的项目！");
            }
        }
    }
}
