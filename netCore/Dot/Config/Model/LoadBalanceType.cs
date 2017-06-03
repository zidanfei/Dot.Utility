using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Config.Model
{ 
    /// <summary>
    /// 数据库类型 主，从,自选最优
    /// </summary>
    public enum LoadBalanceDbType
    {
        /// <summary>
        /// 主
        /// </summary>
        Main,
        /// <summary>
        /// 从
        /// </summary>
        Sub,
        /// <summary>
        /// 自选最优
        /// </summary>
        Best
    }

    /// <summary>
    /// 应用服务器类型 主，从,自选最优
    /// </summary>
    public enum LoadBalanceApplicationType
    {
        /// <summary>
        /// 随机
        /// </summary>
        Random,
        /// <summary>
        ///根据配置来选择
        /// </summary>
        Setting,
        /// <summary>
        /// 自选最优
        /// </summary>
        Best
    }
}
