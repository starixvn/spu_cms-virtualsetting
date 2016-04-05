// -----------------------------------------------------------------------
// <copyright file="ProPunter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Pro punter class.
    /// </summary>
    public class ProPunter
    {
        public int CustId { get; set; }
        
        //public string Name { get; set; }
        
        public double OldVirtualRate { get; set; }
        
        public double NewVirtualRate { get; set; }

        public DateTime ReportDate { get; set; }

        public short SiteId { get; set; }

        public int Srecommend { get; set; }

        public int Mrecommend { get; set; }

        public int Recommend { get; set; }

        public short SportType { get; set; }

        public short WinlossRange { get; set; }

        public int Betcount { get; set; }

    }
}
