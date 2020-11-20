using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
namespace WorkHourService.Jobs
{
    /// <summary>
    /// 按天统计主播的工时收益 定时任务
    /// </summary>
    public class WorkHourIncomeByDayJob : IJob
    {
        /// <summary>
        /// 按天统计主播的工时收益
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            WorkHourIncomeService.StatisticsAnchorWorkHourIncome(DateTime.Now.Date);
            return Task.FromResult(0);
        }
    }
    /// <summary>
    /// 按天统计主播的直播时长
    /// </summary>
    public class AnchorLiveTimeByDayJob : IJob
    {
        /// <summary>
        /// 按天统计主播的直播时长
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            WorkHourIncomeService.StatisticsAnchorLiveTime(DateTime.Now.Date);
            return Task.FromResult(0);
        }
    }
}
