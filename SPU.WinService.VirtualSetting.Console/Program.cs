namespace SPU.WinService.VirtualSetting.Console
{
    using Fanex.Data.Configuration;
    using System;    

    internal class Program
    {
        private static void Main()
        {            
            Console.WriteLine("Start to run window service manually.");

            DbSettingProviderManager.Current.Start();

            VirtualSettingJob job = new VirtualSettingJob();
            job.DoVirtualSetting();            

            Console.WriteLine("Window service is running now.");
        }
    }
}