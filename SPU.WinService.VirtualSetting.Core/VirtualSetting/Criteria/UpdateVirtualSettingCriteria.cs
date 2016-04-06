// -----------------------------------------------------------------------
// <copyright file="UpdateVirtualSettingCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class UpdateVirtualSettingCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int UserId { get; set; }

        public string VirtualMultiplier { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        /*
        @virtualmultiplier = '<ROOT>
							<Rate SportType="1" BetType="1" IsLive="0" VirtualMultiplier="1.5" />	
							<Rate SportType="1" BetType="1" IsLive="1" VirtualMultiplier="1.5" />	
							<Rate SportType="99" BetType="1" IsLive="0" VirtualMultiplier="1.5" />	
							<Rate SportType="99" BetType="1" IsLive="1" VirtualMultiplier="1.5" />	
						</ROOT>'
        */

        /// Gets Setting Key.
        /// </summary>
        /// <returns>Setting key of the criteria.</returns>
        public override string GetSettingKey()
        {
            return "VS_UpdateVirtualSetting";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
