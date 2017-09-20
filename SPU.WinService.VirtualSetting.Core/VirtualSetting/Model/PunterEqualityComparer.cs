// -----------------------------------------------------------------------
// <copyright file="PunterEqualityComparer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SPU.WinService.VitualSetting.Core.VirtualSetting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// PunterEqualityComparer class.
    /// </summary>
    public class PunterEqualityComparer : IEqualityComparer<Punter>
    {
        public bool Equals(Punter x, Punter y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null | y == null)
            {
                return false;
            }
            else if (x.CustID == y.CustID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Punter obj)
        {
            return obj.CustID.GetHashCode();
        }
    }
}
