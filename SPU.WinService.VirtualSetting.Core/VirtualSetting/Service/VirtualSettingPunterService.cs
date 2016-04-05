// -----------------------------------------------------------------------
// <copyright file="VirtualSettingPunterService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data;

    /// <summary>
    /// VirtualSettingPunterService class.
    /// </summary>
    public static class VirtualSettingPunterService
    {        
        public static List<ProPunter> GetProPunterList(GetProPrunterCriteria criteria)
        {
            var proPunters = new List<ProPunter>();

            using (ObjectDb obj = new ObjectDb(criteria.GetSettingKey()))
            {
                var ds = obj.ExecuteDataSet();
                foreach (var row in ds.Tables[0].Rows)
                {
                    proPunters.Add(new ProPunter { });
                }
            }

            return proPunters;
        }

        public static bool UpdateVirtualSetting(UpdateVirtualSettingCriteria criteria)
        {
            using (ObjectDb obj = new ObjectDb(criteria.GetSettingKey()))
            {
                obj.ExecuteScalar();             
            }

            return true;
        }
    }
}
