namespace SPU.WinService.VitualSetting
{
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;
    using System;
    using System.Configuration;
    using System.IO;

    public static class AppSettings
    {
        public static string SiteIds
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteIds"] ?? string.Empty;
            }
        }

        public static string SuperIds
        {
            get
            {
                return ConfigurationManager.AppSettings["SuperIds"] ?? string.Empty;
            }
        }

        public static int ServiceExecutionInterval
        {
            get
            {
                string interval = ConfigurationManager.AppSettings["ServiceExecutionInterval"];
                if (!string.IsNullOrEmpty(interval))
                {
                    return Convert.ToInt32(interval);
                }

                return 7;
            }
        }

        public static int DummyUser
        {
            get
            {
                string dummyUser = ConfigurationManager.AppSettings["DummyUser"];
                if (!string.IsNullOrEmpty(dummyUser))
                {
                    return Convert.ToInt32(dummyUser);
                }

                return 0;
            }
        }

        public static string IP
        {
            get
            {
                return ConfigurationManager.AppSettings["IP"] ?? string.Empty;
            }
        }

        public static int PunterChunkSize
        {
            get
            {
                string count = ConfigurationManager.AppSettings["PunterChunkSize"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToInt32(count);
                }

                return 1000;
            }
        }

        public static double NewVirtualMultiplierForWinning
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplierForWinning"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 1;
            }
        }

        public static double NewVirtualMultiplierForWinning2
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplierForWinning2"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 3;
            }
        }

        public static decimal Margin200
        {
            get
            {
                string count = ConfigurationManager.AppSettings["Margin200"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDecimal(count);
                }

                return 4;
            }
        }
      
        public static short AdjustDateExpiredDays
        {
            get
            {
                string count = ConfigurationManager.AppSettings["AdjustDateExpiredDays"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToInt16(count);
                }

                return 90;
            }
        }

        public static int MailingListId
        {
            get
            {
                string mailingListId = ConfigurationManager.AppSettings["MailingListId"];

                if (!string.IsNullOrEmpty(mailingListId))
                {
                    return Convert.ToInt32(mailingListId);
                }

                return 80;
            }
        }

        public static double VirtualMultiplierForWinning
        {
            get
            {
                string value = ConfigurationManager.AppSettings["VirtualMultiplierForWinning"];

                if (!string.IsNullOrEmpty(value))
                {
                    return Convert.ToDouble(value);
                }

                return 1.0;
            }
        }
       
        public static string LogFile
        {
            get
            {
                var logFile = AppDomain.CurrentDomain.BaseDirectory + @"\Runtime Log\Runtime Log.bin";
                var logFolder = Directory.GetParent(logFile).FullName;

                if (!Directory.Exists(logFolder))
                {
                    Directory.CreateDirectory(logFolder);
                }

                return logFile;
            }
        }

        public static DateTime SpeciaExecuteDate
        {
            get
            {
                var time = ConfigurationManager.AppSettings["SpecialExecuteDate"] ?? @"1/1/1";
                return DateTime.Parse(time);
            }
        }

        public static bool FirstTime
        {
            get
            {
                using (var file = File.Open(LogFile, FileMode.OpenOrCreate))
                {
                    var log = new RuntimeLog<DateTime>(file);
                    return log.GetLogItems().Count == 0;
                }
            }
        }

        public static string JobCronEveryDay
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["JobCronEveryDay"].ToString();
                }
                catch
                {
                    return "0 15 0 * * ?";
                }
            }
        }

        public static int NumOfMonth
        {
            get
            {
                try
                {
                    var value = ConfigurationManager.AppSettings["NumOfMonth"];

                    if (!string.IsNullOrEmpty(value))
                    {
                        return Convert.ToInt32(value);
                    }

                    return 12;
                }
                catch
                {
                    return 12;
                }
            }
        }
    }
}