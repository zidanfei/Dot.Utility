using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dot.Utility
{
    public class WeekHelper
    {
        /// <summary>
        /// 此年有多少周
        /// </summary>
        /// <param name="strYear"></param>
        /// <returns></returns>
        public static int GetWeekCountInYear(int strYear)
        {

            DateTime firstDay = DateTime.Parse(strYear.ToString() + "-01-01");
            int weekOfFirstDay = Convert.ToInt32(firstDay.DayOfWeek);//得到该年的第一天是星期几
            int countWeek;//该年有多少周
            //一年最少有53周，最多有54周
            //如果是平年（365），52×7=364，多余1天，故共有53周
            //如果是闰年（366），52×7=364，多余2天：
            ////如果多余的两天分别是年初和年末，则有54周，其他情况均为53周
            if (DateTime.IsLeapYear(strYear) && weekOfFirstDay == 7)
            {//如果该年是闰年且第一天为星期日
                countWeek = 54;
            }
            else
            {
                countWeek = 53;
            }
            return countWeek;
        }

        /// <summary>
        /// 计算某天在某年的第几周内,且返回这周的起始日期
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetWeekNumAtYear(DateTime dt)
        {
            int week = 0;//返回的周次
            DateTime firstDay = new DateTime(dt.Year, 1, 1);
            //获取该年第一天是星期几
            int firstDayOfWeek = (int)firstDay.DayOfWeek;
            if (0 == firstDayOfWeek)
                firstDayOfWeek = 7;
            DateTime tmpdate = firstDay;
            if (1 != firstDayOfWeek)
                tmpdate = firstDay.AddDays(1 - firstDayOfWeek);

            while (tmpdate <= dt)
            {
                tmpdate = tmpdate.AddDays(7);
                week++;
            }

            return week;
        }
    }
}
