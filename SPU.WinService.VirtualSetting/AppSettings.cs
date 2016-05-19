using System.IO;
using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;

namespace SPU.WinService.VitualSetting
{
    using System;
    using System.Configuration;

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

        public static double NewVirtualMultiplierForLosing
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplierForLosing"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 1;
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
                    return Convert.ToInt32(value);
                }

                return 1.0;
            }
        }

        public static double VirtualMultiplierForLosing
        {
            get
            {
                string value = ConfigurationManager.AppSettings["VirtualMultiplierForLosing"];

                if (!string.IsNullOrEmpty(value))
                {
                    return Convert.ToInt32(value);
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
    }
}