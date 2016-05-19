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

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            var criteria = new GetCurrentCustomerVirtualRate
            {
                CustIds = custIds
            };

            return _virtualSettingsService.GetCurrentCustomerVirtualRate(criteria);
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

        #endregion
    }
}
