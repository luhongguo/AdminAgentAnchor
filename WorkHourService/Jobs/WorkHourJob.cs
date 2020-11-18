using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
namespace WorkHourService.Jobs
{
    /// <summary>
    /// 按天统计代理的礼物收益 定时任务
    /// </summary>
    public class WorkHourIncomeByDayJob : IJob
    {
        /// <summary>
        /// 按天统计代理的礼物收益
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            WorkHourIncomeService.StatisticsAnchorWorkHourIncome(DateTime.Now.Date);
            return Task.FromResult(0);
        }
    }
}
