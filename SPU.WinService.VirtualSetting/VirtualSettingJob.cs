using System.Diagnostics;

namespace SPU.WinService.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;

    public class VirtualSettingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!context.PreviousFireTimeUtc.HasValue || CheckTime())
            {
                Logger.Log.Info("Starting Auto Updating Virtual Settings", "SPU.Local");

                try
                {
                    LogRuntime();
                    AutoAdjustVirtualSettings();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message, "SPU.Local");
                }

                Logger.Log.Info("Finished Auto Updating Virtual Settings", "SPU.Local");
            }
        }

        public void AutoAdjustVirtualSettings()
        {
            var startTime = DateTime.Now;
            var virtualSettingBusiness = new VirtualSettingBusiness();
            var proPunters = virtualSettingBusiness.GetProPunterList(AppSettings.SiteIds, AppSettings.SuperIds);
            var numberOfCustomers = 0;

            if (proPunters != null && proPunters.Any())
            {
                var proPuntersDict = FilterProPunterBaseOnCurrentVirtualRate(virtualSettingBusiness, proPunters);

                UpdateVirtualSetting(virtualSettingBusiness, proPuntersDict.Values);

                numberOfCustomers = proPuntersDict.Count;
            }

            var stopTime = DateTime.Now;

            SendMail(startTime, stopTime, numberOfCustomers);
        }

        private Dictionary<int, ProPunter> FilterProPunterBaseOnCurrentVirtualRate(VirtualSettingBusiness virtualSettingBusiness, IEnumerable<ProPunter> proPunters)
        {
            var virtualRateForWinning = AppSettings.VirtualMultiplierForWinning;
            var proPuntersDict = proPunters.ToDictionary(p => p.CustID);
            var custIds = string.Join(",", proPuntersDict.Keys);

            var currentVRs = virtualSettingBusiness.GetCurrentCustomerVirtualRate(custIds);            

            foreach (var currentVR in currentVRs)
            {
                if (currentVR.VirtualRate < virtualRateForWinning)
                {
                    proPuntersDict[currentVR.CustID].OldVirtualRate = currentVR.VirtualRate;
                }
                else
                {
                    proPuntersDict.Remove(currentVR.CustID);                    
                }
            }

            return proPuntersDict;
        }

        private void UpdateVirtualSetting(VirtualSettingBusiness virtualSettingBusiness, IEnumerable<ProPunter> proPunters)
        {
            var newVirtualMultiplier = AppSettings.NewVirtualMultiplier;
            var userId = AppSettings.DummyUser;
            var ip = AppSettings.IP;
            var chunkSize = AppSettings.ProPunterChunkSize;
            var chunkProPunters = proPunters.Partition(chunkSize);

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

        private void SendMail(DateTime startTime, DateTime stopTime, int numberOfCustomers)
        {
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @".\MailTemplate\MailInformTemplate.cshtml";
            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, startTime.ToString("MM/dd/yyyy hh:mm:ss"), stopTime.ToString("MM/dd/yyyy hh:mm:ss"), numberOfCustomers);

            var newMail = new Fanex.MailDelivery.Client.Models.Mail
            {
                Subject = "VR Auto Adjustment finished",
                Title = "SPU VR Auto Mail",
                Body = body,
                MailingListId = AppSettings.MailingListId
            };

            MailClientHandler.SendMail(newMail);
        }

        private bool CheckTime()
        {
            var currentDate = DateTime.Now;
            var lastMonthDay = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            return lastMonthDay == currentDate.Day || lastMonthDay / 2 == currentDate.Day;
        }

        private void LogRuntime()
        {
            using (var logStream = File.Open(AppSettings.LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var logger = new RuntimeLog<DateTime>(logStream);
                logger.Log(DateTime.Now);
            }
        }
    }
}