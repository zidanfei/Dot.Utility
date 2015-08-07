using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 返回当前实例加上days个星期后的日期
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime AddWeekdays(this DateTime date, int days)
        {
            var sign = days < 0 ? -1 : 1;
            var unsignedDays = Math.Abs(days);
            var weekdaysAdded = 0;
            while (weekdaysAdded < unsignedDays)
            {
                date = date.AddDays(sign);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    weekdaysAdded++;
            }
            return date;
        }

        /// <summary>
        /// 取得当前实例所在月的第一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// 取得当前实例所在月的最后一天
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        /// <summary>
        /// 忽略时间，只比较两个Datetime实例的日期部分
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="compareTo"></param>
        /// <param name="ignoreTime"></param>
        /// <returns>
        /// 1: instance 晚于 compareTo
        /// 0: instance 等于 compareTo
        /// -1: instance 早于 compareTo
        /// </returns>
        public static int CompareTo(this DateTime instance, DateTime compareTo, bool ignoreTime)
        {
            if (ignoreTime)
            {
                return instance.ToString("yyyyMMdd").CompareTo(compareTo.ToString("yyyyMMdd"));
            }
            else
            {
                return instance.CompareTo(compareTo);
            }
        }

    }
}
