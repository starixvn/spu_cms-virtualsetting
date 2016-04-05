namespace SPU.WinService.VirtualSetting.Console
{
    using System;    
    internal class Program
    {
        private static void Main()
        {
            //var queue = new MessageQueueReceiver();
            //queue.Clear();

            var sut = new VirtualSettingService();

            Console.WriteLine("Start to run window service manually.");

            sut.RunOnStart();

            Console.WriteLine("Window service is running now.");
            //while (true) { }
        }
    }
}