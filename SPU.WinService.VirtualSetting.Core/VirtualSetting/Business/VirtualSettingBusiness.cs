// -----------------------------------------------------------------------
// <copyright file="VirtualSettingBusiness.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.NAP.Framework.Command;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSettingBusiness
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingBusiness(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<ProPunter> GetProPunterList()
        {
            var criteria = new GetProPrunterCriteria
            {
                ServiceExeInterval = AppSettings.ServiceExecutionInterval,
                SiteName = AppSettings.SiteName
            };

            return _virtualSettingsService.GetProPunterList(criteria);
        }

        public bool UpdateProPunterVirtualSetting(IEnumerable<ProPunter> proPunters, double virtualMultiplier = 0)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXmlForUpdating(proPunters, virtualMultiplier),
  
                UserId = AppSettings.DummyUser,
                IP = AppSettings.IP
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool LogProPunterVirtualSetting(IEnumerable<ProPunter> proPunters, double newVirtualMultiplier = 0)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                ProPunterVRSettingsDoc = GenerateVirtualSettingXmlForLogging(proPunters, newVirtualMultiplier),

                CreatorId = AppSettings.DummyUser,                
            };

            var updateResult = _virtualSettingsService.LogVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        #region Utilities

        private string GenerateVirtualSettingXmlForUpdating(IEnumerable<ProPunter> proPunters, double virtualMultiplier = 0)
        {
            string format = "<Rate CustIds=\"@CustIds\" VirtualMultiplier =\"" + virtualMultiplier + "\" />";
            
            var collection = proPunters.Select(x => x.CustID);
            var cusIds = string.Join(",", collection);

            return "<ROOT>" + format.Replace("@CustIds", cusIds) + "</ROOT>";            
        }

        private string GenerateVirtualSettingXmlForLogging(IEnumerable<ProPunter> proPunters, double newVirtualMultiplier = 0)
        {
            //string format = "<Rate CustID=\"@CustId\" ReportDate=\"@ReportDate\" SiteID=\"@SiteID\" SRecommend=\"@SRecommend\" MRecommend=\"@MRecommend\" Recommend=\"@Recommend\" OldVirtualRate=\"OldVirtualRate\" NewVirtualRate =\"" + NewVirtualMultiplier + "\" />";

            string xml = string.Empty;

            foreach(var punter in proPunters)
            {
                xml += "<Rate CustID=\"" + punter.CustID + "\"" +
                       " ReportDate=\"" + punter.ReportDate.ToShortDateString() + "\"" +
                       " SiteID=\"" + punter.SiteID + "\""+
                       " SRecommend=\"" + punter.SRecommend + "\"" +
                       " MRecommend=\"" + punter.MRecommend + "\"" +
                       " Recommend=\"" + punter.Recommend + "\"" +
                       " OldVirtualRate=\"" + punter.OldVirtualRate + "\"" +
                       " NewVirtualRate=\"" + newVirtualMultiplier + "\"" + "/>";                       
            }

            return "<ROOT>" + xml + "</ROOT>";
        }


        #endregion
    }
}
