// -----------------------------------------------------------------------
// <copyright file="VirtualSettingHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class VirtualSettingHandler
    {
        public static List<ProPunter> GetProPunterList(GetProPrunterCriteria criteria)
        {
            return VirtualSettingPunterService.GetProPunterList(criteria);
        }

        public static bool UpdateProPunterVirtualSetting(List<ProPunter> proPunters)
        {
            var result = true;

            foreach (var punter in proPunters)
            {
                UpdateVirtualSettingCriteria criteria = new UpdateVirtualSettingCriteria()
                {
                    CustId = punter.CustId,
                    UserId = 0, //todo
                    VirtualMultiplier = "<ROOT> </ROOT>"
                };

                result = VirtualSettingPunterService.UpdateVirtualSetting(criteria) && result;
            }

            return result;
        }
    }
}
