namespace SPU.WinService.VirtualSetting.Console
{
    using System;    

    internal class Program
    {
        private static void Main()
        {
            var virtualSettingService = new VirtualSettingService();

            Console.WriteLine("Start to run window service manually.");

            virtualSettingService.RunOnStart();

            Console.WriteLine("Window service is running now.");
        }
    }
}