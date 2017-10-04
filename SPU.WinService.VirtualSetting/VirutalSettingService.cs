namespace SPU.WinService.VirtualSetting
{
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Quartz.Impl;
    using SPU.WinService.VirtualSetting.JobScheduler;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;
    using System.ServiceProcess;

    public partial class VirtualSettingService : ServiceBase
    {
        private QuartzJobScheduler _jobScheduler;

        public VirtualSettingService()
        {
            InitializeComponent();
            InitializeScheduler();
            this.CanShutdown = true;
            DbSettingProviderManager.Current.Start();
        }

        private void InitializeScheduler()
        {
            var schedulderFacttory = new StdSchedulerFactory();
            var scheduler = schedulderFacttory.GetScheduler();

            _jobScheduler = new QuartzJobScheduler(scheduler);
            _jobScheduler.Schedule(typeof(VirtualSettingJob), AppSettings.JobCronEveryDay);
        }

        protected override void OnStart(string[] args)
        {
            Log4NetService.Info("OnStart Fanex.AutoAdjustVR.Service for scheduling to update Virtual Settings");
            Logger.Log.Info("OnStart Fanex.AutoAdjustVR.Service for scheduling to update Virtual Settings");

            MailClientHandler.Start();

            base.OnStart(args);

            _jobScheduler.Start();
        }

        protected override void OnStop()
        {
            Log4NetService.Info("OnStop Fanex.AutoAdjustVR.Service for updating Virtual Settings");
            Logger.Log.Info("OnStop Fanex.AutoAdjustVR.Service for updating Virtual Settings");

            MailClientHandler.Stop();

            base.OnStop();

            _jobScheduler.Shutdown();
        }

        protected override void OnShutdown()
        {
            Log4NetService.Info("OnShutdown Fanex.AutoAdjustVR.Service for updating Virtual Settings");
            Logger.Log.Info("OnShutdown Fanex.AutoAdjustVR.Service for updating Virtual Settings");

            MailClientHandler.Stop();

            _jobScheduler.Shutdown();
        }       
    }
}