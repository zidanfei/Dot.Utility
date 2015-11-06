/*******************************************************
 * 
 * 作者：胡庆访
 * 创建日期：20141201
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20141201 12:14
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Utility.EntityFramework
{



    /// <summary>
    /// EF 中所有实体的基类。
    /// </summary>
    [Serializable]
    public class EntityBase : EntityBase<int>
    {

    }
    /// <summary>
    /// EF 中所有实体的基类。
    /// </summary>
    [Serializable]
    public class EntityBase<T>:EntityBaseWithId<T>
    {
        public EntityBase()
        {
            IsEnabled = true;
        }

        //public T Id { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 启用（1）、禁用（0）
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [Display(Name = "创建日期")]
        [Required(ErrorMessage = "创建日期不能为空")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [Display(Name = "更新日期")]
        public DateTime? UpdatedOn { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Display(Name = "创建者")]
        public string CreatedUser { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatedUserId { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        [Display(Name = "更新者")]
        public string UpdatedUser { get; set; }

        /// <summary>
        /// 更新者
        /// </summary>
        public string UpdatedUserId { get; set; }


    }
}
