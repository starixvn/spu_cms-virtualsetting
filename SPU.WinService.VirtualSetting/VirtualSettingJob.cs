using Fanex.Logging;
using Quartz;
using SPU.WinService.VitualSetting;
using SPU.WinService.VitualSetting.Core;
using SPU.WinService.VitualSetting.Core.VirtualSetting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPU.WinService.VirtualSetting
{
    public class VirtualSettingJob: IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Logger.Log.Info("Starting updating virtual service", "SPU.Local");

            try
            {
                DoVirtualSetting();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.Message, "SPU.Local");
            }

            Logger.Log.Info("Ending updating virtual setting", "SPU.Local");
        }

        public void DoVirtualSetting()
        {
            VirtualSettingBusiness virtualSettingBusiness = new VirtualSettingBusiness();

            var proPunters = virtualSettingBusiness.GetProPunterList(AppSettings.SiteName);

            if (proPunters != null && proPunters.Count() > 0)
            {
                UpdateVirtualSetting(virtualSettingBusiness, proPunters);
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
