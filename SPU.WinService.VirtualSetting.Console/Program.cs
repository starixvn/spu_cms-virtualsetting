namespace SPU.WinService.VirtualSetting.Console
{
    using Fanex.Data.Configuration;
    using System;
    using Fanex.MailDelivery.Client.Handlers;
    using SPU.WinService.VitualSetting;

    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Start to run window service manually.");

            DbSettingProviderManager.Current.Start();
            MailClientHandler.Start();

            var siteIds = AppSettings.SiteIds;
            var superIds = AppSettings.SuperIds;

            VirtualSettingJob job = new VirtualSettingJob();
            job.AutoAdjustVirtualSettingsForWinningPunters(siteIds, superIds);
            job.AutoAdjustVirtualSettingsForLosingPunters(siteIds, superIds);

            MailClientHandler.Stop();

            Console.WriteLine("Window service is running now.");
        }
    }
}