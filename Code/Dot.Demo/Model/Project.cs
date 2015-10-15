namespace Dot.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;
    using System.Data.Entity.Spatial;

    [Table("Project")]
    public partial class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "项目名称不能为空！")]
        [StringLength(150, ErrorMessage = "项目名称长度不超过50！")]
        [Index("IX_ProjectName", 1, IsUnique = true)]
        public string ProjectName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(50,ErrorMessage ="项目类型长度不超过50！")]
        public string ProjectType { get; set; }

        public int? ContractState { get; set; }

        public Guid? Manager { get; set; }

        public string ManagerName { get; set; }

        public int ProjectState { get; set; }

      
        [StringLength(150)]
        public string DeliverLocation { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

      

        [Description("项目启动会日期")]
        public DateTime? StartMeetDate { get; set; }

        [Description("内部工时")]
        public decimal? InnerTimeSheetSum { get; set; }


        [Description("合同工时")]
        public decimal? ContractTimeSheetSum { get; set; }
        

    }

    [NotMapped]
    public partial class ProjectExtend : Project
    {

    }
    public class ProjectConfig : EntityTypeConfiguration<Project>
    {
        public ProjectConfig()
        {
            this.HasKey(c => c.Id);
        }
    }
}
