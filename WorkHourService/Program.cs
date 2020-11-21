using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using WorkHourService.Jobs;

namespace WorkHourService
{
    public class Program
    {
        static void Main(string[] args)
        {
            //1、调度器
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler().Result;
            ////2、创建一个任务
            string WorkHourIncomeByDayJobCron = ConfigurationManager.AppSettings["WorkHourIncomeByDayJobCron"];
            IJobDetail job = JobBuilder.Create<WorkHourIncomeByDayJob>()
              .WithIdentity("job10", "group10")
              .Build();

            //3、创建一个触发器
            //DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger10", "group10")
                .WithCronSchedule(WorkHourIncomeByDayJobCron)     //每天凌晨0点10分执行      0 10 0 * * ?                                       
                .Build();
            sched.ScheduleJob(job, trigger);

            //2、创建第二个任务
            string AnchorLiveTimeByDayJobCron = ConfigurationManager.AppSettings["AnchorLiveTimeByDayJobCron"];
            IJobDetail job2 = JobBuilder.Create<AnchorLiveTimeByDayJob>()
              .WithIdentity("job11", "group11")
              .Build();

            //3、创建二个触发器
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger11", "group11")
                .WithCronSchedule(AnchorLiveTimeByDayJobCron)                    
                .Build();          
            sched.ScheduleJob(job2, trigger2);

            ////3、创建三个任务
            string WorkHourIncomeCrossDayJobCron = ConfigurationManager.AppSettings["WorkHourIncomeCrossDayJobCron"];
            IJobDetail job3 = JobBuilder.Create<WorkHourIncomeCrossDayJob>()
              .WithIdentity("job12", "group12")
              .Build();

            //3、创建三个触发器
            ITrigger trigger3 = TriggerBuilder.Create()
                .WithIdentity("trigger12", "group12")
                .WithCronSchedule(WorkHourIncomeCrossDayJobCron)                                       
                .Build();
            sched.ScheduleJob(job3, trigger3);

            //启动任务
            sched.Start();
            Console.Read();
        }
    }
}
