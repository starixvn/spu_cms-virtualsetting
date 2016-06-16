namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    public class GetCustomerBasedOnAdjustDateCriteria : CriteriaBase
    {
        public string CustIds { get; set; }

        public short ExpiredDays { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetCustAdjustDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
