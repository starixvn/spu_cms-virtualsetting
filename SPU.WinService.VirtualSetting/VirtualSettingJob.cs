namespace SPU.WinService.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client.Handlers;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;

    public class VirtualSettingJob: IJob
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

            Logger.Log.Info("Finished Auto Updating Virtual Settings", "SPU.Local");
        }

        public void AutoAdjustVirtualSettings()
        {
            var startTime = DateTime.Now;
            var virtualSettingBusiness = new VirtualSettingBusiness();
            var proPunters = virtualSettingBusiness.GetProPunterList(AppSettings.SiteIds, AppSettings.SuperIds);

            if (proPunters != null && proPunters.Any())
            {
                UpdateVirtualSetting(virtualSettingBusiness, proPunters);

                var numberOfCustomers = proPunters.Count();
                var stopTime = DateTime.Now;
                var mailContent = GetMailContent(startTime, stopTime, numberOfCustomers);

                SendMail(mailContent);
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

        private void SendMail(string mailContent)
        {
            var newMail = new Fanex.MailDelivery.Client.Models.Mail
            {
                Subject = "VR Auto Adjustment finished",
                Title = "SPU VR Auto Mail",
                Body = mailContent,
                MailingListId = AppSettings.MailingListId
            };

            MailClientHandler.SendMail(newMail);
        }

        private string GetMailContent(DateTime startTime, DateTime stopTime, int numberOfCustomers)
        {
            var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @".\MailTemplate\MailInformTemplate.cshtml";
            var body = Utilities.ReadTextFile(mailTemplatePath);
            body = string.Format(body,
                                startTime.ToString("MM/dd/yyyy hh:mm:ss"),
                                stopTime.ToString("MM/dd/yyyy hh:mm:ss"),
                                numberOfCustomers);

            return body;
        }
    }
}
