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
    using System.Threading.Tasks;

    public class VirtualSettingForSoccer : VirtualSettingBase
    {
        public VirtualSettingForSoccer(string siteIds, string superIds, DateTime adjustDate) 
            : base()
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
                        
                        var normalPunters = puntersDict.Values.Where(p => !p.IsSuperProPunters);
                        var updatedNormalPunters = AdjustVirtualSettings(normalPunters, AppSettings.NewVirtualMultiplierForWinning);

                        if (updatedProPunters != null && updatedNormalPunters != null)
                        {
                            return updatedProPunters.Union(updatedNormalPunters, PunterEqualityComparer);
                        }
                        else if (updatedProPunters != null)
                        {
                            return updatedProPunters;
                        }
                        else if (updatedNormalPunters != null)
                        {
                            return updatedNormalPunters;
                        }                        
                    }
                }                
            }
            catch (Exception exception)
            {
                IsSuccessful = false;
                Logger.Log.Error("Error (exception) for AutoAdjustVirtualSettingsForPunters of VirtualSettingForSoccer : " + exception.Message);
            }

            return IsSuccessful ? new List<Punter>() : null;
        }

        public override IEnumerable<Punter> GetPunterList()
        {
            var startTime = DateTime.Now;
            var endTime = startTime;

            try
            {                
                var punters = VirtualSettingBusiness.GetWinningPunterSoccerList(SiteIds, SuperIds, VirtualRateThreshold);
                try
                {
                    endTime = DateTime.Now;
                    Log4NetService.Info(string.Format("Timespan of calling GetWinningPunterSoccerList, starTime = {0}, endTime = {1}", startTime, endTime));
                    Logger.Log.Info("Timespan of calling GetWinningPunterSoccerList, starTime = {0}, endTime = {1}", startTime, endTime);

                    if (punters != null)
                    {
                        var info = "List of Soccer (" + punters.Count() + ")";
                        Task.Factory.StartNew(() =>
                        {
                            Logger.Log.Info(info);
                            Log4NetService.Info(info + " ==> {" + string.Join(",", punters.Select(p => p.CustID)) + "}");
                        });
                    }
                }
                catch
                {
                }

                return punters;
            }
            catch (Exception exception)
            {
                IsSuccessful = false;
                endTime = DateTime.Now;
                Log4NetService.Info(string.Format("Error - Timespan of calling GetWinningPunterSoccerList, starTime = {0}, endTime = {1}", startTime, endTime));
                Logger.Log.Info("Error - Timespan of calling GetWinningPunterSoccerList, starTime = {0}, endTime = {1}", startTime, endTime);

                Logger.Log.Error("Error (exception) for GetPunterList of VirtualSettingForSoccer : " + exception.Message);
            }

            return IsSuccessful ? new List<Punter>() : null;
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
                IsSuccessful = false;
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
                IsSuccessful = false;
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

                    return succesfulUpdatedPunters;
                }
            }
            catch (Exception exception)
            {
                IsSuccessful = false;
                Logger.Log.Error("Error (exception) for AdjustVirtualSettings of VirtualSettingForSoccer : " + exception.Message);
            }

            return IsSuccessful ? new List<Punter>() : null;
        }
      
        #endregion       
    }
}
