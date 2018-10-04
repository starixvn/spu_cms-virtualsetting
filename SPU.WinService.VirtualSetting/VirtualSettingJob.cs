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
    using System.Threading;

    public class VirtualSettingJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (IsStopService())
            {
                return;
            }

            if (IsExecutedDate())
            {
                Log4NetService.Info("Starting Auto Updating Virtual Settings");                
                Logger.Log.Info("Starting Auto Updating Virtual Settings");                

                try
                {
                    var startTime = DateTime.Now;

                    var puntersForSoccer = ExecuteForSoccer(AppSettings.SiteIds, AppSettings.SuperIds, startTime);

                    if (puntersForSoccer != null)
                    {
                        Thread.Sleep(AppSettings.IdleTime * 1000);

                        var puntersForSportbooks = ExecuteForSportbooks(AppSettings.SiteIds, AppSettings.SuperIds, startTime);

                        var stopTime = DateTime.Now;

                        var countAffectedPunters = CountAffectedMembers(puntersForSoccer, puntersForSportbooks);
                        Log4NetService.Info("Done Virtual Setting For All Sports -> Affected members = " + countAffectedPunters);
                        Logger.Log.Info("Done Virtual Setting For All Sports -> Affected members = " + countAffectedPunters);

                        SendMail(startTime, stopTime, countAffectedPunters);
                        LogRuntime();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message);
                }

                Log4NetService.Info("Finished Auto Updating Virtual Settings");
                Logger.Log.Info("Finished Auto Updating Virtual Settings");
            }
            else
            {
                string message = "Is NOT the execution date for Updating Virtual Settings";
                Log4NetService.Info(message);
                Logger.Log.Info(message);
                /// Send email to SPU only
                SendMail("VR Auto Mail Service - Still alive", "VR Auto Mail Service - Still alive", message, AppSettings.MailingIdToSPU);
            }
        }
      
        #region Private Method

        private IEnumerable<Punter> ExecuteForSoccer(string siteIds, string superIds, DateTime adjustDate)
        {
            Log4NetService.Info("Starting Virtual Setting For Soccer");
            Logger.Log.Info("Starting Virtual Setting For Soccer");

            var virtualSettingForSoccer = new VirtualSettingForSoccer(siteIds, superIds, adjustDate);
            var puntersForSoccer = virtualSettingForSoccer.AutoAdjustVirtualSettingsForPunters();

            var count = puntersForSoccer != null ? puntersForSoccer.Count() : 0;
            var custIds = count > 0 ? string.Join(";", puntersForSoccer.Select(p => string.Join(",", p.CustID, p.OldVirtualRate, p.NewVirtualRate))) : string.Empty;

            Log4NetService.Info("Done Virtual Setting For Soccer -> Affected members (Soccer) = " + count + " => {" + custIds + "}");
            Logger.Log.Info("Done Virtual Setting For Soccer -> Affected members (Soccer) = " + count);

            return virtualSettingForSoccer.IsSuccessful ? puntersForSoccer : null;
        }

        private IEnumerable<Punter> ExecuteForSportbooks(string siteIds, string superIds, DateTime adjustDate)
        {
            Log4NetService.Info("Starting Virtual Setting For Sportsbook");
            Logger.Log.Info("Starting Virtual Setting For Sportsbook");

            var virtualSettingForSportbooks = new VirtualSettingForSportsbook(siteIds, superIds, adjustDate);
            var puntersForSportbooks = virtualSettingForSportbooks.AutoAdjustVirtualSettingsForPunters();
            
            var count = puntersForSportbooks != null ? puntersForSportbooks.Count() : 0;
            var custIds = count > 0 ? string.Join(";", puntersForSportbooks.Select(p => string.Join(",", p.CustID, p.OldVirtualRate, p.NewVirtualRate))) : string.Empty;

            Log4NetService.Info("Done Virtual Setting For Sportsbook -> Affected members (Sportsbook) = " + count + " => {" + custIds + "}");
            Logger.Log.Info("Done Virtual Setting For Sportsbook -> Affected members (Sportsbook) = " + count);

            return virtualSettingForSportbooks.IsSuccessful ? puntersForSportbooks : null;
        }

        private int CountAffectedMembers(IEnumerable<Punter> puntersForSoccer, IEnumerable<Punter> puntersForSportbooks)
        {
            if (puntersForSoccer != null && puntersForSportbooks != null)
            {
                return puntersForSoccer.Union(puntersForSportbooks, new PunterEqualityComparer()).Count();
            }
            else if (puntersForSoccer != null)
            {
                return puntersForSoccer.Count();
            }
            else if (puntersForSportbooks != null)
            {
                return puntersForSportbooks.Count();
            }
            else
            {
                return 0;
            }
        }

        private void SendMail(string subject, string tile, string body, int mailingListId)
        {            
            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = subject + " (" + AppSettings.Environment + ")" ,
                Title = tile,
                Body = body,
                MailingListId = mailingListId
            };

            MailClientHandler.SendFunctionMail(newMail);
        }

        private void SendMail(DateTime startTime, DateTime stopTime, int numberOfCustomers)
        {
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @".\MailTemplate\MailInformTemplate.cshtml";
            var body = Utilities.ReadTextFile(mailTemplatePath);

            body = string.Format(body, startTime.ToString("MM/dd/yyyy hh:mm:ss"), stopTime.ToString("MM/dd/yyyy hh:mm:ss"), numberOfCustomers);

            var newMail = new Fanex.MailDelivery.Core.Models.Mail
            {
                Subject = "VR Auto Adjustment finished" + " (" + AppSettings.Environment + ")",
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

        private bool IsStopService()
        {
            return AppSettings.IsStopService;
        }

        #endregion Private Method
    }
}