// -----------------------------------------------------------------------
// <copyright file="VirtualSettingPunterService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;
    using Fanex.NAP.Framework.Command;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// VirtualSettingPunterService class.
    /// </summary>
    public class VirtualSettingPunterService : QueryService
    {
        public IEnumerable<Punter> GetPunterList(CriteriaBase criteria)
        {
            return GetData<Punter>(criteria);
        }
      
        public IEnumerable<int> GetPunterIdList(CriteriaBase criteria)
        {
            return GetData<int>(criteria);
        }

        public ICommandResult UpdateVirtualSetting(UpdateVirtualSettingCriteria criteria)
        {
            return ExecuteOutput(criteria);
        }

        public ICommandResult LogVirtualSetting(LogVirtualSettingCriteria criteria)
        {
            return ExecuteOutput(criteria);
        }

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(GetCurrentCustomerVirtualRate criteria)
        {
            return GetData<CustomerVirtualRate>(criteria);
        }

        public ICommandResult InsertCustomerMarginHistory(InsertCustomerMarginHistoryCriteria criteria)
        {
            return ExecuteOutput(criteria);
        }

        public IEnumerable<CustomerMargin> GetCustomerMarginHistory(CriteriaBase criteria)
        {
            return GetData<CustomerMargin>(criteria);
        }
    }
}
