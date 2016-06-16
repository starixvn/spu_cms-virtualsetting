namespace SPU.WinService.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;

    public class VirtualSettingJob : IJob
    {
        private VirtualSettingBusiness _virtualSettingBusiness = new VirtualSettingBusiness();
        
        public void Execute(IJobExecutionContext context)
        {
            Logger.Log.Info("Starting Auto Updating Virtual Settings", "SPU.Local");

            if (IsExecutedDate())
            {
                try
                {                    
                    var startTime = DateTime.Now;

                    var siteIds = AppSettings.SiteIds;
                    var superIds = AppSettings.SuperIds;

                    var numberOfWinningPunters = AutoAdjustVirtualSettingsForWinningPunters(siteIds, superIds);
                    var numberOfLosingMembers = AutoAdjustVirtualSettingsForLosingPunters(siteIds, superIds);

                    var stopTime = DateTime.Now;

                    SendMail(startTime, stopTime, numberOfWinningPunters + numberOfLosingMembers);

                    LogRuntime();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message, "SPU.Local");
                }

                Logger.Log.Info("Finished Auto Updating Virtual Settings", "SPU.Local");
            }
        }

        public int AutoAdjustVirtualSettingsForWinningPunters(string siteIds, string superIds)
        {
            var virtualRateThresholdForWining = AppSettings.VirtualMultiplierForWinning;
            var newVirtualRate = AppSettings.NewVirtualMultiplierForWinning;
            var expiredDays = AppSettings.AdjustDateExpiredDays;
            var numberOfWinningPunters = 0;

            var winningPunters = _virtualSettingBusiness.GetWinningPunterList(siteIds, superIds, virtualRateThresholdForWining);      

            if (winningPunters != null && winningPunters.Any())
            {
                winningPunters = FilterPunterBaseOnAdjustDate(winningPunters, expiredDays);

                var winningPuntersDict = FilterWinningPunterBaseOnCurrentVirtualRate(winningPunters, virtualRateThresholdForWining);

                UpdateVirtualSetting(winningPuntersDict.Values, newVirtualRate);

                numberOfWinningPunters = winningPuntersDict.Count;
            }

            return numberOfWinningPunters;
        }

        public int AutoAdjustVirtualSettingsForLosingPunters(string siteIds, string superIds)
        {                     
            var virtualRateThresholdForLosing = AppSettings.VirtualMultiplierForLosing;
            var newVirtualRate = AppSettings.NewVirtualMultiplierForLosing;
            var expiredDays = AppSettings.AdjustDateExpiredDays;
            var numberOfLosingMembers = 0;
            
            var losingPunters = _virtualSettingBusiness.GetLosingPunterList(siteIds, superIds, virtualRateThresholdForLosing);

            if (losingPunters != null && losingPunters.Any())
            {
                losingPunters = FilterPunterBaseOnAdjustDate(losingPunters, expiredDays);

                var losingPuntersDict = FilterLosingPunterBaseOnCurrentVirtualRate(losingPunters, virtualRateThresholdForLosing);

                UpdateVirtualSetting(losingPuntersDict.Values, newVirtualRate);

                numberOfLosingMembers = losingPuntersDict.Count;
            }

            return numberOfLosingMembers;
        }

        private IEnumerable<Punter> FilterPunterBaseOnAdjustDate(IEnumerable<Punter> punters, short expiredDays)
        {                    
            var custIds = string.Join(",", punters.Select(p => p.CustID));

            var filteredPunters = _virtualSettingBusiness.FilterPunterBaseOnAdjustDate(custIds, expiredDays);

            return punters.Where(p => filteredPunters.Contains(p.CustID));          
        }

        private Dictionary<int, Punter> FilterWinningPunterBaseOnCurrentVirtualRate(IEnumerable<Punter> winningPunters, double virtualRateThresholdForWinning)
        {
            var winningPuntersDict = winningPunters.Distinct().ToDictionary(p => p.CustID);
            var custIds = string.Join(",", winningPuntersDict.Keys);

            var currentVRs = _virtualSettingBusiness.GetCurrentCustomerVirtualRate(custIds);

            foreach (var currentVR in currentVRs)
            {
                if (currentVR.VirtualRate < virtualRateThresholdForWinning)
                {
                    winningPuntersDict[currentVR.CustID].OldVirtualRate = currentVR.VirtualRate;
                }
                else
                {
                    winningPuntersDict.Remove(currentVR.CustID);
                }
            }

            return winningPuntersDict;
        }

        private Dictionary<int, Punter> FilterLosingPunterBaseOnCurrentVirtualRate(IEnumerable<Punter> losingPunters, double virtualRateThresholdForLosing)
        {            
            var losingPuntersDict = losingPunters.ToDictionary(p => p.CustID);
            var custIds = string.Join(",", losingPuntersDict.Keys);

            var currentVRs = _virtualSettingBusiness.GetCurrentCustomerVirtualRate(custIds);

            foreach (var currentVR in currentVRs)
            {
                if (currentVR.VirtualRate >= virtualRateThresholdForLosing)
                {
                    losingPuntersDict[currentVR.CustID].OldVirtualRate = currentVR.VirtualRate;
                }
                else
                {
                    losingPuntersDict.Remove(currentVR.CustID);
                }
            }

            return losingPuntersDict;
        }

        private void UpdateVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate)
        {            
            var userId = AppSettings.DummyUser;
            var ip = AppSettings.IP;
            var chunkSize = AppSettings.PunterChunkSize;
            var chunkPunters = punters.Partition(chunkSize);

            foreach (var selectedPunters in chunkPunters)
            {
                var updateResult = _virtualSettingBusiness.UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, userId, ip);
                if (!updateResult)
                {
                    Logger.Log.Error("Error for updating virtual settings", "SPU.Local");
                }
                else
                {
                    var logResult = _virtualSettingBusiness.LogPunterVirtualSetting(selectedPunters, newVirtualRate, userId);
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

        private bool IsExecutedDate()
        {
            var currentDate = DateTime.Today;            
            return currentDate.Day == 1 || currentDate.Day == 16 || currentDate == AppSettings.SpeciaExecuteDate;
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