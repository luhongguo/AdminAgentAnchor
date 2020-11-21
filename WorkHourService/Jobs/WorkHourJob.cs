using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
namespace WorkHourService.Jobs
{
    /// <summary>
    /// 按天统计主播的工时收益 定时任务 凌晨6点执行
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
            var endTime = DateTime.Now.Date;
            var startTime = endTime.AddDays(-1);
            WorkHourIncomeService.StatisticsAnchorWorkHourIncome(startTime, endTime);
            return Task.FromResult(0);
        }
    }
    /// <summary>
    /// 统计主播 跨天工时收益   处理跨天数据   中午12点执行前天晚上8点-凌晨3点的数据
    /// </summary>
    public class WorkHourIncomeCrossDayJob : IJob
    {
        /// <summary>
        /// 按天统计主播的工时收益 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var endTime = DateTime.Now.Date.AddHours(3);
            var startTime = endTime.AddHours(-7);
            WorkHourIncomeService.StatisticsAnchorWorkHourIncome(startTime, endTime);
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
