// -----------------------------------------------------------------------
// <copyright file="VirtualSettingPunterService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.NAP.Framework.Command;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// VirtualSettingPunterService class.
    /// </summary>
    public class VirtualSettingPunterService : QueryService
    {        
        public IEnumerable<ProPunter> GetProPunterList(GetProPrunterCriteria criteria)
        {
            return GetData<ProPunter>(criteria);
        }

        public ICommandResult UpdateVirtualSetting(UpdateVirtualSettingCriteria criteria)
        {
            return ExecuteOutput(criteria);
        }
    }
}
