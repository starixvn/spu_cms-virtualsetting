namespace SPU.WinService.VirtualSetting
{
    using Fanex.Logging;
    using Fanex.MailDelivery.Client;
    using Quartz;
    using SPU.WinService.VitualSetting;
    using SPU.WinService.VitualSetting.Core;
    using SPU.WinService.VitualSetting.Core.VirtualSetting;
    using SPU.WinService.VitualSetting.Core.VirtualSetting.Log;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public abstract class VirtualSettingBase
    {
        public VirtualSettingBase()
        {
            FailedUpdatePunterList = new List<Punter>();
            VirtualSettingBusiness = new VirtualSettingBusiness();
        }

        public string ServiceName { get; set; }

        public string SiteIds { get; set; }
        
        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

        public List<Punter> FailedUpdatePunterList { get; set; }        
        
        public VirtualSettingBusiness VirtualSettingBusiness { get; set; }

        #region Abstract Methods

        public abstract IEnumerable<Punter> AutoAdjustVirtualSettingsForPunters();        

        public abstract IEnumerable<Punter> GetPunterList();        

        public abstract Dictionary<int, Punter> FilterPunterBaseOnCurrentVirtualRate(IEnumerable<Punter> punters);

        public abstract bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip);

        #endregion

        #region Public Methods

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            try 
            { 
                return VirtualSettingBusiness.GetCurrentCustomerVirtualRate(custIds);
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for GetCurrentCustomerVirtualRate on " + this.ServiceName + " ; " + exception.Message);
            }

            return null;
        }

        public void UpdateVirtualSetting(IEnumerable<Punter> punters, double newVirtualRate)
        {
            try
            {
                var userId = AppSettings.DummyUser;
                var ip = AppSettings.IP;
                var chunkSize = AppSettings.PunterChunkSize;
                var numOfMonth = AppSettings.NumOfMonth;
                var chunkPunters = punters.Partition(chunkSize);

                foreach (var selectedPunters in chunkPunters)
                {
                    var updateResult = UpdatePunterVirtualSetting(selectedPunters, newVirtualRate, userId, ip);
                    if (!updateResult)
                    {
                        FailedUpdatePunterList.AddRange(selectedPunters);

                        Logger.Log.Error("Error for updating virtual settings on " + this.ServiceName);
                    }
                    else
                    {
                        var logResult = VirtualSettingBusiness.LogPunterVirtualSetting(ServiceName, selectedPunters, newVirtualRate, userId);
                        if (!logResult)
                        {
                            Logger.Log.Error("Error for logging virtual settings on " + this.ServiceName);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for UpdateVirtualSetting on " + this.ServiceName + " ; " + exception.Message);
            }
        }

        public void UpdateCustomerMarginHistory(IEnumerable<Punter> punters)
        {
            try
            {
                var numOfMonth = AppSettings.NumOfMonth;
                
                var result = VirtualSettingBusiness.InsertCustomerMarginHistory(punters, numOfMonth);
                if (!result)
                {
                    Logger.Log.Error("Error for logging customer margin history on "  + this.ServiceName);
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for UpdateCustomerMarginHistory on " + this.ServiceName + " ; " + exception.Message);
            }
        }

        #endregion
    }
}