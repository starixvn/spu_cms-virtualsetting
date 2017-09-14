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

    public class VirtualSettingForAllSports : VirtualSettingBase
    {
        public VirtualSettingForAllSports(string siteIds, string superIds)
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
                FailedUpdatePunterList.Clear();

                var newVirtualRate = AppSettings.NewVirtualMultiplierForWinning;

                var punters = GetPunterList();

                if (punters != null && punters.Any())
                {
                    var puntersDict = FilterPunterBaseOnCurrentVirtualRate(punters);

                    if (puntersDict != null && puntersDict.Any())
                    {
                        UpdateVirtualSetting(puntersDict.Values, newVirtualRate);

                        var succesfulUpdatedPunters = puntersDict.Values.Except(FailedUpdatePunterList);
                        if (succesfulUpdatedPunters != null && succesfulUpdatedPunters.Any())
                        {
                            UpdateCustomerMarginHistory(succesfulUpdatedPunters);
                            return succesfulUpdatedPunters;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForPunters of VirtualSettingForAllSports : " + exception.Message, "SPU.Local");
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
                Logger.Log.Error("Error (exception) for GetPunterList of VirtualSettingForAllSports : " + exception.Message, "SPU.Local");
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
                        puntersDict[currentVR.CustID].SportType = 0;
                    }
                    else if (currentVR.VirtualRateForOtherSports < VirtualRateThreshold)
                    {
                        puntersDict[currentVR.CustID].OldVirtualRate = currentVR.VirtualRateForOtherSports;
                        puntersDict[currentVR.CustID].SportType = 2;
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
            return UpdatePunterVirtualSettingForAllSports(punters, virtualMultiplier, userId, ip) &&
                UpdatePunterVirtualSettingForOtherSports(punters, virtualMultiplier, userId, ip);
        }

        #region Private Methods

        private bool UpdatePunterVirtualSettingForAllSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForAllSports = punters.Where(p => p.SportType == 0);
            return VirtualSettingBusiness.UpdatePunterVirtualSettingForAllSports(puntersForAllSports, virtualMultiplier, userId, ip);
        }

        private bool UpdatePunterVirtualSettingForOtherSports(IEnumerable<Punter> punters, double virtualMultiplier, int userId, string ip)
        {
            var puntersForOtherSports = punters.Where(p => p.SportType == 2);
            return VirtualSettingBusiness.UpdatePunterVirtualSettingForOtherSports(puntersForOtherSports, virtualMultiplier, userId, ip);
        }

        #endregion
    }
}