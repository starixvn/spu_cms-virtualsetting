namespace SPU.WinService.VirtualSetting
{
    using System.ServiceProcess;
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using System.Linq;
    using System;
    using SPU.WinService.VitualSetting;

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
            Logger.Log.Error("Starting updating virtual service", "SPU.Local");

            MailClientHandler.Start();

            UpdateVirtualSetting();

            Logger.Log.Error("Ending updating virtual setting", "SPU.Local");
        }

        protected override void OnStop()
        {
            MailClientHandler.Stop();
        }

        public void UpdateVirtualSetting()
        {                     
            try
            {
                int batchMembersNumber = AppSettings.BatchMembersNumber;
                double newVirtualMultiplier = AppSettings.NewVirtualMultiplier;
                string siteName = AppSettings.SiteName;
                int serviceExecutionInterval = AppSettings.ServiceExecutionInterval;
                int userId = AppSettings.DummyUser;
                string IP = AppSettings.IP;

                VirtualSettingBusiness virtualSettingBusiness = new VirtualSettingBusiness();

                var proPunters = virtualSettingBusiness.GetProPunterList(siteName, serviceExecutionInterval);

                if (proPunters != null && proPunters.Count() > 0)
                {
                    bool update = virtualSettingBusiness.UpdateProPunterVirtualSetting(proPunters, newVirtualMultiplier, userId, IP);
                    if (!update)
                    {
                        Logger.Log.Error("Error for updating virtual setting", "SPU.Local");
                    }
                    else
                    {
                        bool log = virtualSettingBusiness.LogProPunterVirtualSetting(proPunters, newVirtualMultiplier, userId);
                        if (!log)
                        {
                            Logger.Log.Error("Error for logging virtual setting", "SPU.Local");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, "SPU.Local");
            }
            
        }
    }
}