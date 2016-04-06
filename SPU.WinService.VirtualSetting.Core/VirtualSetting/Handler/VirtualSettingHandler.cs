// -----------------------------------------------------------------------
// <copyright file="VirtualSettingHandler.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using Fanex.NAP.Framework.Command;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VirtualSettingHandler
    {
        private VirtualSettingPunterService _virtualSettingsService;

        public VirtualSettingHandler(VirtualSettingPunterService virtualSettingsService = null)
        {
            _virtualSettingsService = virtualSettingsService ?? new VirtualSettingPunterService();
        }

        public IEnumerable<ProPunter> GetProPunterList()
        {
            var criteria = new GetProPrunterCriteria();//todo           

            return _virtualSettingsService.GetProPunterList(criteria);
        }

        public bool UpdateProPunterVirtualSetting(List<ProPunter> proPunters, double virtualMultiplier = 0)
        {
            var criteria = new UpdateVirtualSettingCriteria
            {
                VirtualMultiplier = GenerateVirtualSettingXml(proPunters, virtualMultiplier),
  
                UserId = 0,//todo                
                IP = "127.0.0.1"//todo
            };

            var updateResult = _virtualSettingsService.UpdateVirtualSetting(criteria);

            return updateResult.IsSuccessful;
        }

        #region Utilities

        private string GenerateVirtualSettingXml(List<ProPunter> proPunters, double virtualMultiplier)
        {
            string format = "<Rate CustIds=\"@CustIds\" VirtualMultiplier =\"" + virtualMultiplier + "\" />";
            string cusIds = string.Empty;

            foreach(var punter in proPunters)
            {
                cusIds += punter.CustID.ToString();                
            }

            return "<ROOT>" + format.Replace("@CustIds", cusIds) + "</ROOT>";            
        }

        #endregion
    }
}
