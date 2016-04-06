namespace SPU.WinService.VirtualSetting
{
    using System.ServiceProcess;    
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using System.Linq;

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
            Logger.Log.Error("Starting SPU.VirtualSetting service", "SPU VirtualSetting LOCAL");

            MailClientHandler.Start();

            UpdateVirtualSetting();

            Logger.Log.Error("Ending updating virtual setting", "SPU VirtualSetting LOCAL");
        }

        protected override void OnStop()
        {         
            MailClientHandler.Stop();
        }

        public void UpdateVirtualSetting()
        {                     
            VirtualSettingHandler virtualSettingHandler = new VirtualSettingHandler();
            
            var proPunters = virtualSettingHandler.GetProPunterList();

            if (proPunters != null && proPunters.Count() > 0)
            {
                bool update = virtualSettingHandler.UpdateProPunterVirtualSetting(proPunters.ToList());

                if(!update)
                {
                    //write log for exception.
                }
            }
        }
    }
}