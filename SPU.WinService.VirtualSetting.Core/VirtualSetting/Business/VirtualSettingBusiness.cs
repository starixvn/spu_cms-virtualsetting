namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    public class VirtualSettingBusiness
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingBusiness(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<Punter> GetWinningPunterList(string siteNames, string superIds, double virtualRateThreshold)
        {
            var proPuntercriteria = new GetProPrunterCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return _virtualSettingsService.GetPunterList(proPuntercriteria);
        }

        public IEnumerable<Punter> GetLosingPunterList(string siteNames, string superIds, double virtualRateThreshold)
        {
            var losingPuntercriteria = new GetLosingPrunterCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return _virtualSettingsService.GetPunterList(losingPuntercriteria);
        }

        public bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXmlForUpdating(punters, virtualMultiplier),
                UserId = userId,
                IP = ip
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool LogPunterVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate, int userId)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                PunterVRSettingsDoc = GenerateVirtualSettingXmlForLogging(punters, newVirtualRate),
                CreatorId = userId
            };

            var logResult = _virtualSettingsService.LogVirtualSetting(criteria);

            return logResult.IsSuccessful;
        }

        public bool InsertCustomerMarginHistory(IEnumerable<Punter> punters, int numOfMonth)
        {
            var criteria = new GetCustomerMarginHistoryCriteria
            {
                CustIds = string.Join(",", punters.Select(c => c.CustID).Distinct()),
                NumOfMonth = numOfMonth
            };

            var customerMarginList = _virtualSettingsService.GetCustomerMarginHistory(criteria);
            var insertCriteria = new InsertCustomerMarginHistoryCriteria
            {
                CustomerMarginDoc = GenerateVirtualSettingXmlForCustomerMarginHistory(customerMarginList)
            };

            var result = _virtualSettingsService.InsertCustomerMarginHistory(insertCriteria);

            return result.IsSuccessful;
        }

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            var criteria = new GetCurrentCustomerVirtualRate
            {
                CustIds = custIds
            };

            return _virtualSettingsService.GetCurrentCustomerVirtualRate(criteria);
        }

        public IEnumerable<int> FilterPunterBaseOnAdjustDate(string custIds, short expiredDays)
        {
            var criteria = new GetCustomerBasedOnAdjustDateCriteria
            {
                CustIds = custIds,
                ExpiredDays = expiredDays
            };

            return _virtualSettingsService.GetPunterIdList(criteria);
        }

        #region Utilities

        private string GenerateVirtualSettingXmlForUpdating(IEnumerable<Punter> proPunters, double virtualMultiplier)
        {
            var collection = proPunters.Select(x => x.CustID);
            var cusIds = string.Join(",", collection);
            var result = string.Format("<ROOT><Rate CustIds=\"{0}\" VirtualMultiplier =\"{1} \" /></ROOT>", cusIds, virtualMultiplier);

            return result;
        }

        private string GenerateVirtualSettingXmlForLogging(IEnumerable<Punter> proPunters, double newVirtualRate)
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

        private string GenerateVirtualSettingXmlForCustomerMarginHistory(IEnumerable<CustomerMargin> data)
        {
            var xml = new StringBuilder();
            var pattern = "<Rate CustID=\"{0}\" ReportMonth=\"{1}\" ReportYear=\"{2}\" Turnover=\"{3}\"" +
                " Winloss=\"{4}\" BetCount=\"{5}\" />";

            foreach (var item in data)
            {
                xml.AppendFormat(
                                    pattern,
                                    item.CustId,
                                    item.ReportMonth,
                                    item.ReportYear,
                                    item.Turnover,
                                    item.Winloss,
                                    item.BetCount);
            }

            return "<ROOT>" + xml + "</ROOT>";
        }

        #endregion
    }
}
