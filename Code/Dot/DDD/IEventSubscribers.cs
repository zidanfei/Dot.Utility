using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.DDD
{
    /// <summary>
    /// 某个事件的处理者列表。
    /// </summary>
    public interface IEventSubscribers
    {
        /// <summary>
        /// 对应的事件类型。
        /// </summary>
        Type EventType { get; }

        /// <summary>
        /// 返回当前已有的监听者个数。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 直接向所有监听者发布该事件。
        /// </summary>
        /// <param name="eventModel"></param>
        void Publish(object eventModel);
    }
}
