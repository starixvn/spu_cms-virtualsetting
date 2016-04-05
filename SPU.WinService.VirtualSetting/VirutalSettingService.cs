namespace SPU.WinService.VirtualSetting
{
    using System.ServiceProcess;    
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;    

    public partial class VirtualSettingService : ServiceBase
    {                
        public VirtualSettingService()
        {
            InitDependencies();     
            DbSettingProviderManager.Current.Start();
        }

        private void InitDependencies()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            RunOnStart();
        }

        protected override void OnStop()
        {         
            MailClientHandler.Stop();
        }

        public void RunOnStart()
        {
            Logger.Log.Error("Starting SPU.VirtualSetting service", "SPU VirtualSetting LOCAL");

            MailClientHandler.Start();

            Logger.Log.Error("Ending updating virtual setting", "SPU VirtualSetting LOCAL");
        }
    }
}