// -----------------------------------------------------------------------
// <copyright file="UpdateVirtualSettingCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// UpdateVirtualSettingCriteria Class.
    /// </summary>
    public class UpdateVirtualSettingCriteria : CriteriaBase
    {
        public string VirtualMultiplier { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        /*
        @virtualmultiplier = '<ROOT>
                                <Rate CustIds="1,2,3" VirtualMultiplier="1.5" />
                                <Rate CustIds="4" VirtualMultiplier="1.5" />
                                <Rate CustIds="5,6,7" VirtualMultiplier="1.5" />
                             </ROOT>'
        */

        /// Gets Setting Key.
        /// </summary>
        /// <returns>Setting key of the criteria.</returns>
        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdVirtualMultiplier_Multiple";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
