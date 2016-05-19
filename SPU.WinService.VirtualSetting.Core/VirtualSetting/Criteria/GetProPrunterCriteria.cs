// -----------------------------------------------------------------------
// <copyright file="GetProPrunterCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Pro-Punter Criteria Class.
    /// </summary>
    public class GetProPrunterCriteria : CriteriaBase
    {        
        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

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
