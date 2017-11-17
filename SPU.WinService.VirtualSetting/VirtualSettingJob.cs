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
                Log4NetService.Info("Starting Auto Updating Virtual Settings");
                Logger.Log.Info("Starting Auto Updating Virtual Settings");

                try
                {
                    //var startTime = DateTime.Now;

                    //var puntersForSoccer = ExecuteForSoccer(AppSettings.SiteIds, AppSettings.SuperIds, startTime);

                    //System.Threading.Thread.Sleep(AppSettings.IdleTime * 1000);

                    //var puntersForSportbooks = ExecuteForSportbooks(AppSettings.SiteIds, AppSettings.SuperIds, startTime);

                    //var stopTime = DateTime.Now;

                    //var countAffectedPunters = CountAffectedMembers(puntersForSoccer, puntersForSportbooks);
                    //Log4NetService.Info("Done Virtual Setting For All Sports -> Affected members = " + countAffectedPunters);
                    //Logger.Log.Info("Done Virtual Setting For All Sports -> Affected members = " + countAffectedPunters);

                    //SendMail(startTime, stopTime, countAffectedPunters);

                    //LogRuntime();

                    var business = new VirtualSettingBusiness();
                    var custIdList = "4442013,4676164,7439317,9731919,10528092,11412295,13908210,15682069,16999490,19462980,20154762,21293991,21854837,22202985,22933511,23973772,24345872,24625892,24871658,25129545,25599660,25674480,25827710,25896096,25972220,25998286,26029845,26155887,26255134,26262775,26309003,26338998,26376218,26400516,26497522,26564919,26588373,26595388,26626863,26628938,26642495,26643857,26679088,26681517,26691202,26692247,26693569,26693846,26696458,26702652,26729578,26733140,26741945,26742313,26747694,26748923,26759010,26759281,26777430,26778157,26778442,26781202,26782289,26782754,26787987,26795621,26805689,26805912,26814015,26824986,26826123,26826132,26835098,26848875,26849021,26861542,26873194,26873738,26884235,26884648,26887423,26889452";

                    business.InsertHistoryDataForSpecialCustIdList(custIdList, 1, AppSettings.NumOfMonth, new DateTime(2017, 11, 17, 0, 15, 0), "Soccer");

                }
                catch (Exception ex)
                {
                    Logger.Log.Error(ex.Message);
                }

                Log4NetService.Info("Finished Auto Updating Virtual Settings");
                Logger.Log.Info("Finished Auto Updating Virtual Settings");
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

            return puntersForSoccer;
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

            return puntersForSportbooks;
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