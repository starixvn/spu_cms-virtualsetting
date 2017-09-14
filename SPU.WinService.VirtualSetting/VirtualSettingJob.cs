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

                    var virtualSettingForSoccer = new VirtualSettingForSoccer(siteIds, superIds);
                    var puntersForSoccer = virtualSettingForSoccer.AutoAdjustVirtualSettingsForPunters();

                    var virtualSettingForAllSports = new VirtualSettingForAllSports(siteIds, superIds);
                    var puntersForAllSports = virtualSettingForAllSports.AutoAdjustVirtualSettingsForPunters();

                    var countUpdatedPunters = 0;
                    if (puntersForSoccer != null && puntersForAllSports != null)
                    {
                        countUpdatedPunters = puntersForSoccer.Union(puntersForAllSports).GroupBy(p => p.CustID).Count();
                    }
                    else
                    {
                        if (puntersForSoccer != null)
                        {
                            countUpdatedPunters = puntersForSoccer.Count();
                        }
                        else
                        {
                            countUpdatedPunters = puntersForAllSports.Count();
                        }
                    }

                    var stopTime = DateTime.Now;

                    SendMail(startTime, stopTime, countUpdatedPunters);

                    LogRuntime();
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message, "SPU.Local");
                }

                Logger.Log.Info("Finished Auto Updating Virtual Settings", "SPU.Local");
            }
        }
               
        #region Private Method

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

        #endregion Private Method
    }
}