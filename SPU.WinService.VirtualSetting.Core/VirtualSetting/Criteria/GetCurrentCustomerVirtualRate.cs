namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    public class GetCurrentCustomerVirtualRate : CriteriaBase
    {
        public string CustIds { get; set; }

        public bool IsVRSoccer { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_Report_CustMngt_GetCustomersVirtualRate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
