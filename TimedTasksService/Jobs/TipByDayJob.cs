using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
namespace TimedTasksService.Jobs
{
    /// <summary>
    /// 按天统计代理的礼物收益 定时任务
    /// </summary>
    public class TipByDayJob : IJob
    {
        /// <summary>
        /// 按天统计代理的礼物收益
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            TipService.StatisticsAgentTipIncome(DateTime.Now);
            return Task.FromResult(0);
        }
    }
    /// <summary>
    /// 每隔2分钟采集一次 打赏礼物数据
    /// </summary>
    public class CollectGiftsJob : IJob
    {
        /// <summary>
        /// 按小时统计用户数据任务
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            TipService.StatisticsCollectTipGifts(DateTime.Now,200);
            return Task.FromResult(0);
        }
    }
}
