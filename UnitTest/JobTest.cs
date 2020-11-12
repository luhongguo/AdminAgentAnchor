using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimedTasksService;

namespace UnitTest
{
    [TestClass]
    public class JobTest
    {
        /// <summary>
        /// 礼物采集
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            TipService.StatisticsCollectTipGifts(DateTime.Now.AddDays(-7), 200);//第一次需要改redis中的时间 采集200条数据
        }
        /// <summary>
        /// 按天统计代理的礼物收益
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            TipService.StatisticsAgentTipIncome(DateTime.Now.AddDays(-3), DateTime.Now);//前一天的收益数据
        }
        [TestMethod]
        public void TestTime()
        {
            var a = GetBeginToNow();
            var b = GetEndToNow();
            var c = GetBeginToYesterDay();
            var d = GetEndToYesterDay();
            var e = GetBeginToWeek();
            var f = GetEndToWeek();
            var g = GetBeginToMonth();
            var h = GetEndToMonth();
            return;
        }
        /// <summary>
        /// 获取今天的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBeginToNow()
        {
            return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00.000"));
        }

        /// <summary>
        /// 获取今天的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndToNow()
        {
            return Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59.998"));
        }

        /// <summary>
        /// 获取昨天的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBeginToYesterDay()
        {
            return Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 00:00:00.000"));
        }

        /// <summary>
        /// 获取昨天的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndToYesterDay()
        {
            return Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd 23:59:59.998"));
        }

        /// <summary>
        /// 获取本周的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBeginToWeek()
        {
            var dt = DateTime.Now;
            int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
            DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);
            if (dayOfWeek == 0) weekStartTime = weekStartTime.AddDays(-7);

            return Convert.ToDateTime(weekStartTime.ToString("yyyy-MM-dd 00:00:00.000"));
        }

        /// <summary>
        /// 获取本周的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndToWeek()
        {
            var dt = DateTime.Now;
            int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
            DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);
            if (dayOfWeek == 0) weekStartTime = weekStartTime.AddDays(-7);

            return Convert.ToDateTime(weekStartTime.AddDays(6).ToString("yyyy-MM-dd 23:59:59.998"));
        }

        /// <summary>
        /// 获取本月的开始时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetBeginToMonth()
        {
            var dt = DateTime.Now;
            DateTime startMonth = dt.AddDays(1 - dt.Day);  //本月月初

            return Convert.ToDateTime(startMonth.ToString("yyyy-MM-dd 00:00:00.000"));
        }

        /// <summary>
        /// 获取本月的结束时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndToMonth()
        {
            var dt = DateTime.Now;
            DateTime startMonth = dt.AddDays(1 - dt.Day);  //本月月初

            return Convert.ToDateTime(startMonth.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd 23:59:59.998"));//本月月末
        }
    }
}
