// -----------------------------------------------------------------------
// <copyright file="GetProPrunterSoccerCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Pro-Punter Soccer Criteria Class.
    /// </summary>
    public class GetProPrunterSoccerCriteria : CriteriaBase
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
            return "NAP_Report_CustMngt_GetProPunters_Soccer";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
