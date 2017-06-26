using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Data
{
    public enum DataState
    {
        // 摘要: 
        //     该实体未由上下文跟踪。刚使用新运算符或某个 System.Data.Entity.DbSet Create 方法创建实体后，实体就处于此状态。
        Detached = 1,
        //
        // 摘要: 
        //     实体将由上下文跟踪并存在于数据库中，其属性值与数据库中的值相同。
        Unchanged = 2,
        //
        // 摘要: 
        //     实体将由上下文跟踪，但是在数据库中还不存在。
        Added = 4,
        //
        // 摘要: 
        //     实体将由上下文跟踪并存在于数据库中，但是已被标记为在下次调用 SaveChanges 时从数据库中删除。
        Deleted = 8,
        //
        // 摘要: 
        //     实体将由上下文跟踪并存在于数据库中，已修改其中的一些或所有属性值。
        Modified = 16,
    }
}
