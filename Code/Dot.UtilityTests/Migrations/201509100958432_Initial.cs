namespace Dot.UtilityTests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeSheet",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkTime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentDate = c.DateTime(nullable: false),
                        ProjectId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeSheet", "ProjectId", "dbo.Project");
            DropIndex("dbo.TimeSheet", new[] { "ProjectId" });
            DropTable("dbo.TimeSheet");
            DropTable("dbo.Project");
        }
    }
}
