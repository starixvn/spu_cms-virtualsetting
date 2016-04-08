namespace SPU.WinService.VirtualSetting
{
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceProcess;

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

            DoVirtualSetting();

            Logger.Log.Error("Ending updating virtual setting", "SPU.Local");
        }

        protected override void OnStop()
        {
            MailClientHandler.Stop();
        }

        public void DoVirtualSetting()
        {                     
            try
            {                               
                VirtualSettingBusiness virtualSettingBusiness = new VirtualSettingBusiness();

                var proPunters = virtualSettingBusiness.GetProPunterList(AppSettings.SiteName, AppSettings.ServiceExecutionInterval);                               

                if (proPunters != null && proPunters.Count() > 0)
                {
                    UpdateVirtualSetting(virtualSettingBusiness, proPunters);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, "SPU.Local");
            }            
        }

        private void UpdateVirtualSetting(VirtualSettingBusiness virtualSettingBusiness, IEnumerable<ProPunter> proPunters)
        {
            double newVirtualMultiplier = AppSettings.NewVirtualMultiplier;
            int userId = AppSettings.DummyUser;
            string ip = AppSettings.IP;
            int chunkSize = AppSettings.ProPunterChunkSize;

            var chunkProPunters = Extensions.Partition(proPunters, chunkSize);

            foreach (var punters in chunkProPunters)
            {
                bool update = virtualSettingBusiness.UpdateProPunterVirtualSetting(punters, newVirtualMultiplier, userId, ip);
                if (!update)
                {
                    Logger.Log.Error("Error for updating virtual setting", "SPU.Local");
                }
                else
                {
                    bool log = virtualSettingBusiness.LogProPunterVirtualSetting(punters, newVirtualMultiplier, userId);
                    if (!log)
                    {
                        Logger.Log.Error("Error for logging virtual setting", "SPU.Local");
                    }
                }
            }
        }
    }
}