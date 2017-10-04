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

    public class VirtualSettingForSportsbook : VirtualSettingBase
    {
        public VirtualSettingForSportsbook(string siteIds, string superIds, DateTime adjustDate)
        {
            ServiceName = "Sportsbook";
            AdjustDate = adjustDate;
            SportGroupID = 0;
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
                        FailedUpdatePunterList.Clear();

                        double newVirtualRate = AppSettings.NewVirtualMultiplierForWinning;
                        UpdateVirtualSetting(puntersDict.Values, newVirtualRate);

                        var succesfulUpdatedPunters = puntersDict.Values.Except(FailedUpdatePunterList, PunterEqualityComparer);
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
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForPunters of VirtualSettingForSportbooks : " + exception.Message);
            }

            return null;
        }

        public override IEnumerable<Punter> GetPunterList()
        {
            try
            {
                return VirtualSettingBusiness.GetWinningPunterList(SiteIds, SuperIds, VirtualRateThreshold);
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for GetPunterList of VirtualSettingForSportbooks : " + exception.Message);
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
                        if (currentVR.VRSoccer < VirtualRateThreshold)
                        {
                            puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VRSoccer;
                            puntersDict[currentVR.CustID].SportType = 0;
                        }
                        else if (currentVR.VROtherSports < VirtualRateThreshold)
                        {
                            puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VROtherSports;
                            puntersDict[currentVR.CustID].SportType = 2;
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
                Logger.Log.Error("Error (exception) for FilterPunterBaseOnCurrentVirtualRate of VirtualSettingForSportbooks : " + exception.Message);
            }

            return null;
        }

        public override bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            try
            {
                return UpdatePunterVirtualSettingForAllSports(punters, virtualMultiplier, userId, ip) &&
                    UpdatePunterVirtualSettingForOtherSports(punters, virtualMultiplier, userId, ip);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Private Methods

        private bool UpdatePunterVirtualSettingForAllSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForAllSports = punters.Where(p => p.SportType == 0);
            if (puntersForAllSports != null && puntersForAllSports.Any())
            {
                return VirtualSettingBusiness.UpdatePunterVirtualSettingForAllSports(puntersForAllSports, virtualMultiplier, userId, ip);
            }

            return true;
        }

        private bool UpdatePunterVirtualSettingForOtherSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForOtherSports = punters.Where(p => p.SportType == 2);
            if (puntersForOtherSports != null & puntersForOtherSports.Any())
            {
                return VirtualSettingBusiness.UpdatePunterVirtualSettingForOtherSports(puntersForOtherSports, virtualMultiplier, userId, ip);
            }

            return true;
        }

        #endregion
    }
}