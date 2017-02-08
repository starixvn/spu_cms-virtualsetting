namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.Data.Repository;

    public class InsertCustomerMarginHistoryCriteria : CriteriaBase
    {
        public string CustomerMarginDoc { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey()
        {
            return "NAP_UserLog_InsertCustomerMarginHistory";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(CustomerMarginDoc);
        }
    }
}
