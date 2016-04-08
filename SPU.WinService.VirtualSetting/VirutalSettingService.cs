namespace SPU.WinService.VirtualSetting
{
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using Quartz;
    using Quartz.Impl;
    using SPU.WinService.VitualSetting;
    using System.ServiceProcess;

    public partial class VirtualSettingService : ServiceBase
    {
        private static IScheduler _scheduler;

        public VirtualSettingService()
        {
            InitializeComponent();
            DbSettingProviderManager.Current.Start();
        }

        protected override void OnStart(string[] args)
        {
            MailClientHandler.Start();

            try
            {
                ScheduleVirtualSettingJob();
            }
            catch
            {
                Logger.Log.Error("Error for scheduling virtual setting", "SPU.Local");
            }

        }
        
        protected override void OnStop()
        {
            MailClientHandler.Stop();

            _scheduler.Shutdown(true);
        }      
   
        private void ScheduleVirtualSettingJob()
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();

            IJobDetail job = JobBuilder.Create<VirtualSettingJob>()
                .WithIdentity("UpdateVirtualSetting", "VirtualSetting")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("TriggerUpdateVirtualSetting", "VirtualSetting")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(AppSettings.ServiceExecutionInterval)                    
                    .RepeatForever())
                .Build();

            _scheduler.ScheduleJob(job, trigger);
        }
    }
}