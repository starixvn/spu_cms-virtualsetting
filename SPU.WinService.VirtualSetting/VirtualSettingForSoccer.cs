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

    public class VirtualSettingForSoccer : VirtualSettingBase
    {
        public VirtualSettingForSoccer(string siteIds, string superIds, DateTime adjustDate)
        {
            ServiceName = "Soccer";
            AdjustDate = adjustDate;
            SportGroupID = 1;
            SiteIds = siteIds;
            SuperIds = superIds;
            VirtualRateThreshold = AppSettings.VirtualRateThresholdForWinning;            
        }

        public override IEnumerable<Punter> AutoAdjustVirtualSettingsForPunters()
        {
            try
            {                
                var punters = GetPunterList();

                if (punters != null && punters.Any())
                {
                    var puntersDict = FilterPunterBaseOnCurrentVirtualRate(punters);

                    if (puntersDict != null && puntersDict.Any())
                    {                        
                        var proPunters = puntersDict.Values.Where(p => p.IsSuperProPunters);
                        var updatedProPunters = AdjustVirtualSettings(proPunters, AppSettings.NewVirtualMultiplierForSuperProPunter);

                        return updatedProPunters;                 
                    }
                }                
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForPunters of VirtualSettingForSoccer : " + exception.Message);
            }

            return null;
        }

        public override IEnumerable<Punter> GetPunterList()
        {
            try
            {                
                return VirtualSettingBusiness.GetWinningPunterSoccerList(SiteIds, SuperIds, VirtualRateThreshold);
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for GetPunterList of VirtualSettingForSoccer : " + exception.Message);
            }

            return null;
        }

        public override Dictionary<int, Punter> FilterPunterBaseOnCurrentVirtualRate(IEnumerable<Punter> punters)
        {
            try
            {
                var puntersDict = punters.Distinct().ToDictionary(p => p.CustID);
                var custIds = string.Join(",", puntersDict.Keys);

                var currentVRs = GetCurrentCustomerVirtualRate(custIds);
                if (currentVRs != null && currentVRs.Any())
                {
                    foreach (var currentVR in currentVRs)
                    {
                        if (currentVR.VRSoccer == VirtualRateThreshold)
                        {
                            puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                            puntersDict[currentVR.CustID].SportType = 1;
                        }
                        else
                        {
                            puntersDict.Remove(currentVR.CustID);
                        }
                    }

                    return puntersDict;
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for FilterPunterBaseOnCurrentVirtualRate of VirtualSettingForSoccer : " + exception.Message);
            }

            return null;
        }

        public override bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            try
            {
                return VirtualSettingBusiness.UpdatePunterVirtualSettingForSoccer(punters, virtualMultiplier, userId, ip);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private Methods

        private IEnumerable<Punter> AdjustVirtualSettings(IEnumerable<Punter> filtedPunters, double newVirtualRate)
        {
            try
            {                                
                if (filtedPunters != null && filtedPunters.Any())
                {
                    FailedUpdatePunterList.Clear();

                    UpdateVirtualSetting(filtedPunters, newVirtualRate);

                    var succesfulUpdatedPunters = filtedPunters.Except(FailedUpdatePunterList, PunterEqualityComparer);
                    if (succesfulUpdatedPunters != null && succesfulUpdatedPunters.Any())
                    {                        
                        foreach (var p in succesfulUpdatedPunters)
                        {
                            p.NewVirtualRate = newVirtualRate;
                        }

                        return succesfulUpdatedPunters;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AdjustVirtualSettings of VirtualSettingForSoccer : " + exception.Message);
            }

            return null;
        }
      
        #endregion       
    }
}
