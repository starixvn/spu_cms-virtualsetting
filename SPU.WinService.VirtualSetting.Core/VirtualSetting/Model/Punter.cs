// -----------------------------------------------------------------------
// <copyright file="Punter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;

    /// <summary>
    /// Punter class.
    /// </summary>
    public class Punter
    {
        public DateTime ReportDate { get; set; }
        
        public short SiteID { get; set; }
        
        public int CustID { get; set; }

        public int SRecommend { get; set; }

        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public double OldVirtualRate { get; set; }

        public double NewVirtualRate { get; set; }

        public short SportType { get; set; }

        public bool IsSuperProPunters { get; set; }

        public bool IsBA { get; set; }
    }
}
