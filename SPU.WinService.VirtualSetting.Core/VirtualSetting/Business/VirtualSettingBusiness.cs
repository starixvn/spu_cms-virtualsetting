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
                ServiceExecutionInterval = AppSettings.ServiceExecutionInterval,
                SiteName = AppSettings.SiteName
            };

            return _virtualSettingsService.GetProPunterList(criteria);
        }

        public bool UpdateProPunterVirtualSetting(IEnumerable<ProPunter> proPunters, double virtualMultiplier = 0)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXml(proPunters, virtualMultiplier),
  
                UserId = AppSettings.DummyUser,
                IP = AppSettings.IP
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        public bool LogProPunterVirtualSetting(IEnumerable<ProPunter> proPunters)
        {
            var criteria = new LogVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXml(proPunters),

                CreatorId = AppSettings.DummyUser,                
            };

            var updateResult = _virtualSettingsService.LogVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        #region Utilities

        private string GenerateVirtualSettingXml(IEnumerable<ProPunter> proPunters, double virtualMultiplier = 0)
        {
            string format = "<Rate CustIds=\"@CustIds\" VirtualMultiplier =\"" + virtualMultiplier + "\" />";
            
            var collection = proPunters.Select(x => x.CustID);
            var cusIds = string.Join(",", collection);

            return "<ROOT>" + format.Replace("@CustIds", cusIds) + "</ROOT>";            
        }

        #endregion
    }
}
