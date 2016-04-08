// -----------------------------------------------------------------------
// <copyright file="GetProPrunterCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// GetProPrunterCriteria Class.
    /// </summary>
    public class GetProPrunterCriteria : CriteriaBase
    {
        public int ServiceExeInterval { get; set; }

        public string SiteName { get; set; }

        /// <summary>
        /// Gets Setting Key.
        /// </summary>
        /// <returns>Setting key of the criteria.</returns>
        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetProPunters";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
