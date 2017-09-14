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
        public VirtualSettingForSoccer(string siteIds, string superIds)
        {
            SiteIds = siteIds;
            SuperIds = superIds;

            VirtualRateThreshold = AppSettings.VirtualMultiplierForWinning;
            FailedUpdatePunterList = new List<Punter>();
            VirtualSettingBusiness = new VirtualSettingBusiness();
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
                        var updatedSuperProPunters = AutoAdjustVirtualSettingsForSuperProPunters(puntersDict);
                        var updatedNormalPunters = AutoAdjustVirtualSettingsForNormalPunters(puntersDict);

                        if (updatedSuperProPunters != null && updatedNormalPunters != null)
                        {
                            return updatedSuperProPunters.Union(updatedNormalPunters);
                        }
                        else
                        {
                            if (updatedSuperProPunters != null)
                            {
                                return updatedSuperProPunters;
                            }
                            else
                            {
                                return updatedNormalPunters;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForPunters of VirtualSettingForSoccer : " + exception.Message, "SPU.Local");
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
                Logger.Log.Error("Error (exception) for GetPunterList of VirtualSettingForSoccer : " + exception.Message, "SPU.Local");
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

                foreach (var currentVR in currentVRs)
                {
                    if (currentVR.VirtualRateForSoccer < VirtualRateThreshold)
                    {
                        puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VirtualRateForSoccer;
                        puntersDict[currentVR.CustID].SportType = 1;                      
                    }
                    else
                    {
                        puntersDict.Remove(currentVR.CustID);
                    }
                }

                return puntersDict;
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for FilterPunterBaseOnCurrentVirtualRate of VirtualSettingBase : " + exception.Message, "SPU.Local");
            }

            return null;
        }

        public override bool UpdatePunterVirtualSetting(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            return VirtualSettingBusiness.UpdatePunterVirtualSettingForSoccer(punters, virtualMultiplier, userId, ip);
        }

        #region Private Methods

        private IEnumerable<Punter> AutoAdjustVirtualSettingsForSuperProPunters(Dictionary<int, Punter> punters)
        {            
            try
            {
                FailedUpdatePunterList.Clear();

                var newVirtualRate = AppSettings.NewVirtualMultiplierForSuperProPunter;

                var filtedPunters = FilterSuperProPunter(punters);

                if (filtedPunters != null && filtedPunters.Any())
                {
                    UpdateVirtualSetting(filtedPunters, newVirtualRate);

                    var succesfulUpdatedPunters = filtedPunters.Except(FailedUpdatePunterList);
                    if (succesfulUpdatedPunters != null && succesfulUpdatedPunters.Any())
                    {
                        UpdateCustomerMarginHistory(succesfulUpdatedPunters);
                        return succesfulUpdatedPunters;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForSuperProPunters of VirtualSettingForSoccer : " + exception.Message, "SPU.Local");
            }

            return null;
        }

        private IEnumerable<Punter> AutoAdjustVirtualSettingsForNormalPunters(Dictionary<int, Punter> punters)
        {            
            try
            {
                FailedUpdatePunterList.Clear();

                var newVirtualRate = AppSettings.NewVirtualMultiplierForWinning;

                var filtedPunters = FilterNormalPunter(punters);

                if (filtedPunters != null && filtedPunters.Any())
                {
                    UpdateVirtualSetting(filtedPunters, newVirtualRate);

                    var succesfulUpdatedPunters = filtedPunters.Except(FailedUpdatePunterList);
                    if (succesfulUpdatedPunters != null && succesfulUpdatedPunters.Any())
                    {
                        UpdateCustomerMarginHistory(succesfulUpdatedPunters);
                        return succesfulUpdatedPunters;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForNormalPunters of VirtualSettingForSoccer : " + exception.Message, "SPU.Local");
            }

            return null;
        }

        private IEnumerable<Punter> FilterSuperProPunter(Dictionary<int, Punter> punters)
        {
            return punters.Values.Where(p => p.IsSuperProPunters = true);
        }

        private IEnumerable<Punter> FilterNormalPunter(Dictionary<int, Punter> punters)
        {
            return punters.Values.Where(p => p.IsSuperProPunters = false);
        }

        #endregion       
    }
}
