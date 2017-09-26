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
            PunterEqualityComparer = new PunterEqualityComparer();
        }

        public string ServiceName { get; set; }

        public DateTime AdjustDate { get; set; }

        public byte SportGroupID { get; set; }

        public string SiteIds { get; set; }

        public string SuperIds { get; set; }

        public double VirtualRateThreshold { get; set; }

        public List<Punter> FailedUpdatePunterList { get; set; }

        public VirtualSettingBusiness VirtualSettingBusiness { get; set; }

        public PunterEqualityComparer PunterEqualityComparer { get; set; }

        #region Abstract Methods

        public abstract IEnumerable<Punter> AutoAdjustVirtualSettingsForPunters();

        public abstract IEnumerable<Punter> GetPunterList();

        public abstract Dictionary<int, Punter> FilterPunterBaseOnCurrentVirtualRate(IEnumerable<Punter> punters);

        public abstract bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip);

        #endregion

        #region Methods

        public IEnumerable<CustomerVirtualRate> GetCurrentCustomerVirtualRate(string custIds)
        {
            try
            {
                return VirtualSettingBusiness.GetCurrentCustomerVirtualRate(custIds);
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for GetCurrentCustomerVirtualRate on " + ServiceName + " ; " + exception.Message);
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

                        Logger.Log.Error("Error for updating virtual settings on " + ServiceName);
                    }
                    else
                    {                        
                        LoggingUpdateVirtualSetting(newVirtualRate, userId, selectedPunters);
                        UpdateCustomerMarginHistory(selectedPunters);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for UpdateVirtualSetting on " + ServiceName + " ; " + exception.Message);
            }
        }

        private void LoggingUpdateVirtualSetting(double newVirtualRate, int userId, IEnumerable<Punter> selectedPunters)
        {
            try
            {
                var logResult = VirtualSettingBusiness.LogPunterVirtualSetting(ServiceName, selectedPunters, newVirtualRate, userId, AdjustDate);
                if (!logResult)
                {
                    Logger.Log.Error("Error for logging virtual settings on " + ServiceName);
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for LoggingUpdateVirtualSetting on " + ServiceName + " ; " + exception.Message);
            }
        }

        private void UpdateCustomerMarginHistory(IEnumerable<Punter> punters)
        {
            try
            {
                var numOfMonth = AppSettings.NumOfMonth;

                var result = VirtualSettingBusiness.InsertCustomerMarginHistory(punters, SportGroupID, numOfMonth, AdjustDate);
                if (!result)
                {
                    Logger.Log.Error("Error for logging customer margin history on " + ServiceName);
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for UpdateCustomerMarginHistory on " + ServiceName + " ; " + exception.Message);
            }
        }

        #endregion
    }
}