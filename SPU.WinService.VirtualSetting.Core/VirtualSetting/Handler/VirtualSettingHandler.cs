// -----------------------------------------------------------------------
// <copyright file="VirtualSettingHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.NAP.Framework.Command;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSettingHandler
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingHandler(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<ProPunter> GetProPunterList(GetProPrunterCriteria criteria)
        {
            return _virtualSettingsService.GetProPunterList(criteria);
        }

        public bool UpdateProPunterVirtualSetting(List<ProPunter> proPunters)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXml(proPunters)
                //// and more params should be here
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        #region Utilities

        private string GenerateVirtualSettingXml(List<ProPunter> proPunters)
        {
            return string.Empty;
        }

        #endregion
    }
}
