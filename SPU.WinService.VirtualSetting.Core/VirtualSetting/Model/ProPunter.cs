// -----------------------------------------------------------------------
// <copyright file="ProPunter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;

    /// <summary>
    /// Pro punter class.
    /// </summary>
    public class ProPunter
    {
        public DateTime rptDate { get; set; }
        
        public short SiteID { get; set; }
        
        public int CustID { get; set; }

        public int srecommend { get; set; }

        public int mrecommend { get; set; }

        public int recommend { get; set; }
        
        public double VirtualRate { get; set; }

        public short SportType { get; set; }
    }
}
