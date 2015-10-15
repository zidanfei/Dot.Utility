using Dot.Demo.Definition;
using Dot.Utility.PSWrapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Demo
{
    public class DBMigration
    {
        public static void Init()
        {
            try
            {

                DBContext context = new DBContext();
                if (!context.Database.Exists())
                {
                    context.Database.Initialize(true);
                    //context.Database.ExecuteSqlCommand("ALTER TABLE TimeSheet ADD CONSTRAINT IX_CurrentDate_UserId_ProjectId  UNIQUE (CurrentDate,UserId,ProjectId)");
 
                }
                //ToolingFacade facade = new ToolingFacade(typeof(Dot.Demo.Project).Assembly.FullName,
                //       typeof(Dot.Demo.Project).Assembly.FullName,
                //       typeof(Dot.Demo.Migrations.Configuration).FullName,
                //       AppDomain.CurrentDomain.BaseDirectory + "bin",
                //       AppDomain.CurrentDomain.BaseDirectory + "Web.config",
                //       null,
                //       new DbConnectionInfo(Constant.ConnectionName.EntityString));
                //facade.Update(null, false);

            }
            catch (Exception ex)
            {
                Dot.Utility.Log.LogFactory.BusinessExceptionLog.Error(ex.Message, ex);
            }
        }
    }
}
