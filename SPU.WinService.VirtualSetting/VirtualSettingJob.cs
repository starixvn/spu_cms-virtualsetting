namespace SPU.WinService.VirtualSetting
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class VirtualSettingJob : IJob
    {
        private VirtualSettingBusiness _virtualSettingBusiness = new VirtualSettingBusiness();
        private List<Punter> _failedUpdatePunterList = new List<Punter>();

        public void Execute(IJobExecutionContext context)
        {
            if (IsExecutedDate())
            {
                Logger.Log.Info("Starting Auto Updating Virtual Settings", "SPU.Local");

                try
                {
                    var startTime = DateTime.Now;

                    var siteIds = AppSettings.SiteIds;
                    var superIds = AppSettings.SuperIds;

                    var numberOfWinningPunters = AutoAdjustVirtualSettingsForWinningPunters(siteIds, superIds);

                    var stopTime = DateTime.Now;

                    SendMail(startTime, stopTime, numberOfWinningPunters);

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
            var numberOfWinningPunters = 0;

            var winningPunters = _virtualSettingBusiness.GetWinningPunterList(siteIds, superIds, virtualRateThresholdForWining);

            if (winningPunters != null && winningPunters.Any())
            {
                var winningPuntersDict = FilterWinningPunterBaseOnCurrentVirtualRate(winningPunters, virtualRateThresholdForWining);
                _failedUpdatePunterList.Clear();

                UpdateVirtualSettingWithTwoLevel(winningPuntersDict.Values);

                UpdateCustomerMarginHistory(winningPuntersDict.Values);

                numberOfWinningPunters = winningPuntersDict.Count - _failedUpdatePunterList.Count;
            }

            return numberOfWinningPunters;
        }
        
        #region Private Method

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

        private void UpdateVirtualSettingWithTwoLevel(IEnumerable<Punter> punters)
        {
            var numOfMonth = AppSettings.NumOfMonth;
            var newVirtualRate = AppSettings.NewVirtualMultiplierForWinning;
            var newVirtualRate2 = AppSettings.NewVirtualMultiplierForWinning2;
            var margin200 = AppSettings.Margin200;

            var customerHistoryList = _virtualSettingBusiness.GetCustomerMarginHistory(punters, numOfMonth);
            var groupCustomers = customerHistoryList.GroupBy(t => t.CustId).Select(t => new CustomerMargin
            {
                CustId = t.ElementAt(0).CustId,
                Turnover = t.Sum(m => m.Turnover),
                Winloss = t.Sum(m => m.Winloss)
            });
            var highMarginCustomers = groupCustomers.Where(t => t.Winloss * 100 / t.Turnover >= margin200).Select(m => m.CustId).Distinct();
            var lowMarginCustomers = groupCustomers.Where(t => t.Winloss * 100 / t.Turnover < margin200).Select(m => m.CustId).Distinct();

            var highMarginPunterrList = GetPunterFromCustIdList(highMarginCustomers, punters);
            var lowMarginPunterrList = GetPunterFromCustIdList(lowMarginCustomers, punters);

            if (highMarginPunterrList.Count() > 0)
            {
                UpdateVirtualSetting(highMarginPunterrList, newVirtualRate2);
            }

            if (lowMarginPunterrList.Count() > 0)
            {
                UpdateVirtualSetting(lowMarginPunterrList, newVirtualRate);
            }
        }

        private IEnumerable<Punter> GetPunterFromCustIdList(IEnumerable<int> custIds, IEnumerable<Punter> punters)
        {
            var result = new List<Punter>();
            foreach (var custId in custIds)
            {
                var punter = punters.Where(t => t.CustID == custId).FirstOrDefault();

                if (punter != null)
                {
                    result.Add(punter);
                }
            }

            return result;
        }

        private void UpdateVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate)
        {
            var userId = AppSettings.DummyUser;
            var ip = AppSettings.IP;
            var chunkSize = AppSettings.PunterChunkSize;
            var numOfMonth = AppSettings.NumOfMonth;
            var chunkPunters = punters.Partition(chunkSize);

            foreach (var selectedPunters in chunkPunters)
            {
                var updateResult = _virtualSettingBusiness.UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, userId, ip);
                if (!updateResult)
                {
                    _failedUpdatePunterList.AddRange(selectedPunters);

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

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Adjustment finished",
                Title = "VR Auto Mail Service",
                Body = body,
                MailingListId = AppSettings.MailingListId
            };

            MailClientHandler.SendFunctionMail(newMail);
        }

        private bool IsExecutedDate()
        {
            var currentDate = DateTime.Today;
            var specialDay = AppSettings.SpeciaExecuteDate;

            var isSpecialDay = currentDate.Year == specialDay.Year && currentDate.Month == specialDay.Month && currentDate.Day == specialDay.Day;

            return currentDate.Day == 1 || currentDate.Day == 16 || isSpecialDay;
        }

        private void LogRuntime()
        {
            using (var logStream = File.Open(AppSettings.LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                var logger = new RuntimeLog<DateTime>(logStream);
                logger.Log(DateTime.Now);
            }
        }

        private void UpdateCustomerMarginHistory(IEnumerable<Punter> punters)
        {
            try
            {
                var numOfMonth = AppSettings.NumOfMonth;

                punters = punters.Except(_failedUpdatePunterList);

                var result = _virtualSettingBusiness.InsertCustomerMarginHistory(punters, numOfMonth);
                if (!result)
                {
                    Logger.Log.Error("Error for logging customer margin history", "SPU.Local");
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for logging customer margin history: " + exception.Message, "SPU.Local");
            }
        }

        #endregion Private Method
    }
}