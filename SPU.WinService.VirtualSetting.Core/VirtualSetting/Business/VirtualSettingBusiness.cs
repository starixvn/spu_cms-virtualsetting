// -----------------------------------------------------------------------
// <copyright file="VirtualSettingBusiness.cs" company="Microsoft">
namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class VirtualSettingBusiness
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingBusiness(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<ProPunter> GetProPunterList(string siteNames, string superIds)
        {
            var criteria = new GetProPrunterCriteria
            {
                SiteIds = siteNames,
                SuperIds = superIds
            };

            return _virtualSettingsService.GetProPunterList(criteria);
        }

        public bool UpdateProPunterVirtualSetting(IEnumerable<ProPunter> proPunters, double virtualMultiplier, int userId, string IP)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXmlForUpdating(proPunters, virtualMultiplier),
                UserId = userId,
                IP = IP
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool LogProPunterVirtualSetting(IEnumerable<ProPunter> proPunters, double newVirtualMultiplier, int userId)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                ProPunterVRSettingsDoc = GenerateVirtualSettingXmlForLogging(proPunters, newVirtualMultiplier),
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

        private string GenerateVirtualSettingXmlForUpdating(IEnumerable<ProPunter> proPunters, double virtualMultiplier)
        {
            var collection = proPunters.Select(x => x.CustID);
            var cusIds = string.Join(",", collection);

            return string.Format("<ROOT><Rate CustIds=\"{0}\" VirtualMultiplier =\"{1} \" /></ROOT>", cusIds,
                virtualMultiplier);
        }

        private string GenerateVirtualSettingXmlForLogging(IEnumerable<ProPunter> proPunters, double newVirtualMultiplier)
        {
            var xml = new StringBuilder();

            foreach (var punter in proPunters)
            {
                xml.AppendFormat("<Rate CustID=\"{0}\" ReportDate=\"{1}\" SiteID=\"{2}\" SRecommend=\"{3}\" MRecommend=\"{4}\" Recommend=\"{5}\" OldVirtualRate=\"{6}\" NewVirtualRate =\"{7}\" />",
                                    punter.CustID,
                                    punter.ReportDate.ToShortDateString(),
                                    punter.SiteID,
                                    punter.SRecommend,
                                    punter.MRecommend,
                                    punter.Recommend,
                                    punter.OldVirtualRate,
                                    newVirtualMultiplier);
            }

            return "<ROOT>" + xml + "</ROOT>";
        }

        #endregion
    }
}
