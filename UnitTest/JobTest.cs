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
           TipService.StatisticsCollectTipGifts(DateTime.Now,200);//第一次需要改redis中的时间 采集200条数据
        }
        /// <summary>
        /// 按天统计代理的礼物收益
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            TipService.StatisticsAgentTipIncome(DateTime.Now);//前一天的收益数据
        }
    }
}
