using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SPU.WinService.VitualSetting
{
    public static class AppSettings
    {
        public static string SiteName
        {
            get
            {
                return ConfigurationManager.AppSettings["SiteName"] ?? string.Empty;
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

        public static int ProPunterChunkSize
        {
            get
            {
                string count = ConfigurationManager.AppSettings["ProPunterChunkSize"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToInt32(count);
                }

                return 1000;
            }
        }

        public static double NewVirtualMultiplier
        {
            get
            {
                string count = ConfigurationManager.AppSettings["NewVirtualMultiplier"];
                if (!string.IsNullOrEmpty(count))
                {
                    return Convert.ToDouble(count);
                }

                return 1;
            }
        }        
    }
}
