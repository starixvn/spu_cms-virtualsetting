namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class VirtualSettingBusiness
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingBusiness(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<Punter> GetWinningPunterSoccerList(string siteNames, string superIds, double virtualRateThreshold)
        {
            var proPuntercriteria = new GetProPrunterSoccerCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds,
                VirtualRateThreshold = virtualRateThreshold
            };

            return _virtualSettingsService.GetPunterList(proPuntercriteria);
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

        public bool UpdatePunterVirtualSettingForAllSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = Utilities.GenerateVirtualSettingXmlForUpdating(punters, virtualMultiplier),
                UserId = userId,
                IP = ip,
                SportType = 0
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool UpdatePunterVirtualSettingForSoccer(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = Utilities.GenerateVirtualSettingXmlForUpdating(punters, virtualMultiplier),
                UserId = userId,
                IP = ip,
                SportType = 1
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool UpdatePunterVirtualSettingForOtherSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = Utilities.GenerateVirtualSettingXmlForUpdating(punters, virtualMultiplier),
                UserId = userId,
                IP = ip,
                SportType = 2
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool LogPunterVirtualSetting(string serviceName, IEnumerable<Punter> punters, double newVirtualRate, int userId, DateTime adjustDate)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                PunterVRSettingsDoc = Utilities.GenerateVirtualSettingXmlForLogging(punters, newVirtualRate),
                CreatorId = userId,
                ServiceName = serviceName,
                AdjustDate = adjustDate
            };

            var logResult = _virtualSettingsService.LogVirtualSetting(criteria);

            return logResult.IsSuccessful;
        }

        public bool InsertCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth, string serviceName, DateTime adjustDate)
        {
            var customerMarginList = GetCustomerMarginHistory(punters, sportGroupID, numOfMonth);

            var insertCriteria = new InsertCustomerMarginHistoryCriteria
            {
                CustomerMarginDoc = Utilities.GenerateVirtualSettingXmlForCustomerMarginHistory(customerMarginList, serviceName),
                AdjustDate = adjustDate
            };

            var result = _virtualSettingsService.InsertCustomerMarginHistory(insertCriteria);

            return result.IsSuccessful;
        }

        public IEnumerable<CustomerMargin> GetCustomerMarginHistory(IEnumerable<Punter> punters, byte sportGroupID, int numOfMonth)
        {
            var criteria = new GetCustomerMarginHistoryCriteria
            {
                CustIds = string.Join(",", punters.Select(c => c.CustID).Distinct()),
                SportGroupID = sportGroupID,
                NumOfMonth = numOfMonth
            };

            var customerMarginList = _virtualSettingsService.GetCustomerMarginHistory(criteria);

            return customerMarginList;
        }

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            var criteria = new GetCurrentCustomerVirtualRate
            {
                CustIds = custIds                
            };

            return _virtualSettingsService.GetCurrentCustomerVirtualRate(criteria);
        }

        public IEnumerable<int> GetBACustIds(string custIds)
        {
            var criteria = new GetBAAccountsCriteria
            {
                CustIds = custIds
            };

            return _virtualSettingsService.GetBACustIds(criteria);
        }
    }
}
