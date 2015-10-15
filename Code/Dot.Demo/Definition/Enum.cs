using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Demo.Definition
{
    public enum ProjectState
    {
        /// <summary>
        /// 准备中
        /// </summary>
        [Display(Name = "准备中")]
        [Description("准备中")]
        Preparing = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        [Display(Name = "进行中")]
        Going = 1,
        /// <summary>
        /// 已撤销
        /// </summary>
        [Description("已撤销")]
        [Display(Name = "已撤销")]
        Canceled = 2,
        /// <summary>
        /// 已结项
        /// </summary>
        [Description("已结项")]
        [Display(Name = "已结项")]
        Done = 4,
        /// <summary>
        ///已中止
        /// </summary>
        [Description("已中止")]
        [Display(Name = "已中止")]
        Stoped = 8,
        /// <summary>
        /// 已关闭
        /// </summary>
        [Description("已关闭")]
        [Display(Name = "已关闭")]
        Closed = 16
    }
    

}
