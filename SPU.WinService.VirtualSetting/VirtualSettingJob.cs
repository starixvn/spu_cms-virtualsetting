namespace SPU.WinService.VirtualSetting
{
    using Fanex.Logging;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VirtualSettingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Logger.Log.Info("Starting Auto Updating Virtual Settings", "SPU.Local");

            try
            {
                AutoAdjustVirtualSettings();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, "SPU.Local");
            }

            Logger.Log.Info("Ending Auto Updating Virtual Settings", "SPU.Local");
        }

        public void AutoAdjustVirtualSettings()
        {
            var virtualSettingBusiness = new VirtualSettingBusiness();
            var proPunters = virtualSettingBusiness.GetProPunterList(AppSettings.SiteName);

            if (proPunters != null && proPunters.Count() > 0)
            {
                UpdateVirtualSetting(virtualSettingBusiness, proPunters);
            }
        }

        private void UpdateVirtualSetting(VirtualSettingBusiness virtualSettingBusiness, IEnumerable<ProPunter> proPunters)
        {
            var newVirtualMultiplier = AppSettings.NewVirtualMultiplier;
            var userId = AppSettings.DummyUser;
            var ip = AppSettings.IP;
            var chunkSize = AppSettings.ProPunterChunkSize;
            var chunkProPunters = Extensions.Partition(proPunters, chunkSize);

            foreach (var punters in chunkProPunters)
            {
                var updateResult = virtualSettingBusiness.UpdateProPunterVirtualSetting(punters, newVirtualMultiplier, userId, ip);
                if (!updateResult)
                {
                    Logger.Log.Error("Error for updating virtual settings", "SPU.Local");
                }
                else
                {
                    var logResult = virtualSettingBusiness.LogProPunterVirtualSetting(punters, newVirtualMultiplier, userId);
                    if (!logResult)
                    {
                        Logger.Log.Error("Error for logging virtual settings", "SPU.Local");
                    }
                }
            }
        }
    }    
}
