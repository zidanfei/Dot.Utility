using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dot.Utility.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Infrastructure;

/******
         在vs2013IDE中，工具-->库程序包管理器-->程序包管理器控制台
		 Enable-Migrations -Force -ContextTypeName 上下文
         Enable-Migrations -Force -ContextTypeName CodeFirstContext
         add-migration Initial 
         update-database -Verbose
         Update-Database –TargetMigration: Initial6
 
 * **************/

namespace Dot.Utility.EntityFramework.Tests
{
    [TestClass()]
    public class EntityFrameworkRepositoryBaseTests
    {
        public EntityFrameworkRepositoryBaseTests()
        {
            CodeFirstContext context = new CodeFirstContext();
            if (!context.Database.Exists())
            {
                context.Database.Initialize(true);

                context.SaveChanges();
            }
            ToolingFacade facade = new ToolingFacade(typeof(TimeSheet).Assembly.FullName,
                typeof(TimeSheet).Assembly.FullName,
                typeof(Dot.UtilityTests.Migrations.Configuration).FullName,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.BaseDirectory + "\\Dot.UtilityTests.dll.config",
                null,
                new DbConnectionInfo("CodeFirst"));
            facade.Update(null, false);
        }

        [TestMethod()]
        public void GetListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetQueryListTest()
        {
            ProjectService pservice = new ProjectService();
            TimeSheetService service = new TimeSheetService(pservice.DBContext);
            #region test

            //var queryp = pservice.GetQueryList(null, 1, 10, m => m.Id.ToString(), null);
            var queryp = pservice.DBContext.Project.OrderByDescending(m => m.Id.ToString()).Skip(1).Take(10);

            var queryts = service.GetQueryList();

            var rlist = (from pt in
                             (from p in queryp
                              join t in queryts on p.Id equals t.ProjectId into tslist
                              from ts in tslist.DefaultIfEmpty()
                              select new
                              {
                                  Id = p.Id,
                                  ProjectName = p.ProjectName,
                                  WorkTime = ts != null ? ts.WorkTime : 0
                              })

                         group pt by new
                         {
                             Id = pt.Id,
                             ProjectName = pt.ProjectName,
                         } into projs
                         select new ProjectExtend
                         {
                             Id = projs.Key.Id,
                             ProjectName = projs.Key.ProjectName,
                             UseTimeSheetSum = projs.Sum(m => m.WorkTime)
                         }).ToList();
            #endregion
        }


        [TestMethod()]
        public void GetCountTest()
        {
            Assert.Fail();
        }


    }

    #region Model

    public partial class CodeFirstContext : DbContext
    {
        public CodeFirstContext()
            : base("name=CodeFirst")
        {
        }


        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<TimeSheet> TimeSheet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


        }
    }


    [Table("Project")]
    public partial class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string ProjectName { get; set; }

        public List<TimeSheet> TimeSheets
        {
            get;
            set;
        }

    }

    [NotMapped]
    public partial class ProjectExtend : Project
    {
        public decimal? UseTimeSheetSum { get; set; }
    }

    [Table("TimeSheet")]
    public partial class TimeSheet
    {
        public int Id { get; set; }

        [Range(0.1, 8)]
        public decimal WorkTime { get; set; }

        //[Index("IX_CurrentDate", 1, IsUnique = true)]
        public DateTime CurrentDate { get; set; }

        public int? ProjectId { get; set; }


    }

    #endregion

    #region Service

    public class TimeSheetService : EntityFrameworkRepositoryBase<TimeSheet, CodeFirstContext>
    {
        public TimeSheetService()
        {

        }
        public TimeSheetService(CodeFirstContext dbcontext)
            : base(dbcontext)
        {
        }
    }

    public class ProjectService : EntityFrameworkRepositoryBase<Project, CodeFirstContext>
    {
        public ProjectService()
        {

        }
        public ProjectService(CodeFirstContext dbcontext)
            : base(dbcontext)
        {
        }
    }
    #endregion
}
