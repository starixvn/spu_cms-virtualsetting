namespace SPU.WinService.VitualSetting.Core
{
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static class Utilities
    {
        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }

        public static string GenerateVirtualSettingXmlForUpdating(IEnumerable<Punter> proPunters, double virtualMultiplier)
        {
            var collection = proPunters.Select(x => x.CustID);
            var cusIds = string.Join(",", collection);
            var result = string.Format("<ROOT><Rate CustIds=\"{0}\" VirtualMultiplier =\"{1} \" /></ROOT>", cusIds, virtualMultiplier);

            return result;
        }

        public static string GenerateVirtualSettingXmlForLogging(IEnumerable<Punter> proPunters, double newVirtualRate)
        {
            var xml = new StringBuilder();
            var pattern = "<Rate CustID=\"{0}\" ReportDate=\"{1}\" SiteID=\"{2}\" SRecommend=\"{3}\"" +
                " MRecommend=\"{4}\" Recommend=\"{5}\" OldVirtualRate=\"{6}\" NewVirtualRate =\"{7}\" />";

            foreach (var punter in proPunters)
            {
                xml.AppendFormat(
                                    pattern,
                                    punter.CustID,
                                    punter.ReportDate.ToShortDateString(),
                                    punter.SiteID,
                                    punter.SRecommend,
                                    punter.MRecommend,
                                    punter.Recommend,
                                    punter.OldVirtualRate,
                                    newVirtualRate);
            }

            return "<ROOT>" + xml + "</ROOT>";
        }

        public static string GenerateVirtualSettingXmlForCustomerMarginHistory(IEnumerable<CustomerMargin> data, string serviceName)
        {
            var xml = new StringBuilder();
            var pattern = "<Rate CustID=\"{0}\" ReportMonth=\"{1}\" ReportYear=\"{2}\" Turnover=\"{3}\"" +
                " Winloss=\"{4}\" BetCount=\"{5}\" ServiceName=\"{6}\" />";

            foreach (var item in data)
            {
                xml.AppendFormat(
                                    pattern,
                                    item.CustId,
                                    item.ReportMonth,
                                    item.ReportYear,
                                    item.Turnover,
                                    item.Winloss,
                                    item.BetCount,
                                    serviceName);
            }

            return "<ROOT>" + xml + "</ROOT>";
        }
    }
}
