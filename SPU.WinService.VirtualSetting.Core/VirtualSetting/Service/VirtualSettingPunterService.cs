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
        public static IEnumerable<ProPunter> GetProPunterList(GetProPrunterCriteria criteria)
        {            
            using (ObjectDb obj = new ObjectDb(criteria.GetSettingKey()))
            {
                return obj.Query<ProPunter>();             
            }            
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
