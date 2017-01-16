namespace SPU.WinService.VirtualSetting.JobScheduler
{
    using Quartz;
    using System;    

    public class QuartzJobScheduler : IJobScheduler, IQuartzJobScheduler
    {
        private IScheduler _scheduler;

        public QuartzJobScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Schedule(Type jobType, int durationInSecond)
        {
            var jobDetail = JobBuilder.Create(jobType)
            .WithIdentity("job" + jobType.ToString(), "group" + jobType.ToString())
            .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger" + jobType.ToString(), "group" + jobType.ToString())
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(durationInSecond).RepeatForever())
                .StartNow()
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void Schedule(Type jobType, string cronException)
        {
            var jobDetail = JobBuilder.Create(jobType)
            .WithIdentity("job" + jobType.ToString(), "group" + jobType.ToString())
            .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger" + jobType.ToString(), "group" + jobType.ToString())
                .WithCronSchedule(cronException)
                .StartNow()
                .Build();

            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void Shutdown()
        {
            _scheduler.Shutdown();
        }

        public void Start()
        {
            _scheduler.Start();
        }

        public void PauseAll()
        {
            _scheduler.PauseAll();
        }

        public void ResumeAll()
        {
            _scheduler.ResumeAll();
        }
    }
}
