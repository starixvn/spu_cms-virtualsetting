namespace SPU.WinService.FraudDetection
{
    using System.ServiceProcess;
    using SPU.WinService.VirtualSetting;

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new VirtualSettingService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}