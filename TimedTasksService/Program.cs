using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimedTasksService.Jobs;

namespace TimedTasksService
{
    public class Program
    {
        static void Main(string[] args)
        {
            //1、调度器
            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sched = sf.GetScheduler().Result;
            //2、创建一个任务
            IJobDetail job = JobBuilder.Create<TipByDayJob>()
              .WithIdentity("job1", "group1")
              .Build();

            //3、创建一个触发器
            //DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithCronSchedule("0/30 * * * * ?")     //每天凌晨0点10分执行      0 10 0 * * ?                                       
                .Build();
            sched.ScheduleJob(job, trigger);


            //2、创建第二个任务
            IJobDetail job2 = JobBuilder.Create<CollectGiftsJob>()
              .WithIdentity("job2", "group2")
              .Build();

            //3、创建二个触发器
            //DateTimeOffset runTime = DateBuilder.EvenMinuteDate(DateTimeOffset.UtcNow);
            ITrigger trigger2 = TriggerBuilder.Create()
                .WithIdentity("trigger2", "group2")
                .WithCronSchedule("0 0/2 * * * ?")     //每天隔两分钟执行一次                                   
                .Build();

            sched.ScheduleJob(job2, trigger2);

            //启动任务
            sched.Start();
            Console.Read();
        }
    }
}
