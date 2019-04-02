namespace SPU.WinService.VirtualSetting.Console
{
    using Fanex.Data.Configuration;
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;
    using System;

    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Start to run window service manually.");

            DbSettingProviderManager.Current.Start();
            MailClientHandler.Start();

            VirtualSettingJob job = new VirtualSettingJob();
            job.Execute(null);

            MailClientHandler.Stop();

            Console.WriteLine("Window service is running now.");
        }
        
    }
}