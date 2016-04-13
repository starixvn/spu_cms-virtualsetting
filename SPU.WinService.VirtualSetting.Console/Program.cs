namespace SPU.WinService.VirtualSetting.Console
{
    using Fanex.Data.Configuration;
    using System;
    using Fanex.MailDelivery.Client.Handlers;

    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Start to run window service manually.");

            DbSettingProviderManager.Current.Start();
            MailClientHandler.Start();

            VirtualSettingJob job = new VirtualSettingJob();
            job.AutoAdjustVirtualSettings();

            MailClientHandler.Stop();

            Console.WriteLine("Window service is running now.");
        }
    }
}