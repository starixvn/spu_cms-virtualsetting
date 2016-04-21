// -----------------------------------------------------------------------
// <copyright file="LogVirtualSettingCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Log Virtual Setting Criteria Class.
    /// </summary>
    public class LogVirtualSettingCriteria : CriteriaBase
    {
        public string ProPunterVRSettingsDoc { get; set; }

        public int CreatorId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        /* @ProPunterVRSettingsDoc ='<ROOT>
                                        <Rate CustID="123456" ReportDate="2016-04-04" SiteID="22" SRecommend="123" MRecommend="456" Recommend="789" OldVirtualRate="0.2" NewVirtualRate="0.8" />
                                        <Rate CustID="7891011" ReportDate="2016-04-05" SiteID="122" SRecommend="21123" MRecommend="11456" Recommend="12789" OldVirtualRate="0.5" NewVirtualRate="0.7" />
                                     </ROOT>'
        */

        /// Gets Setting Key.
        /// </summary>
        /// <returns>Setting key of the criteria.</returns>
        public override string GetSettingKey()
        {
            return "NAP_UserLog_UpdateVirtualRateSettings";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
