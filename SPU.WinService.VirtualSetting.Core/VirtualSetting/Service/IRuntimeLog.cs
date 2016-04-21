using System.Collections.Generic;

namespace SPU.WinService.VitualSetting.Core.VirtualSetting.Service
{
    using System;

    public interface IRuntimeLog<T> where T : IEquatable<T>
    {
        void Log(T item);

        ICollection<T> GetLogItems();
    }
}