namespace SPU.WinService.VitualSetting.Core.VirtualSetting.Log
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using Service;

    public class RuntimeLog<T> : IRuntimeLog<T> where T : IEquatable<T>
    {
        private Stream _stream;
        BinaryFormatter serializer = new BinaryFormatter();

        public RuntimeLog(Stream stream)
        {
            _stream = stream;
        }

        public void Log(T item)
        {
            var items = GetLogItems();

            items.Add(item);

            serializer.Serialize(_stream, items);
        }

        public ICollection<T> GetLogItems()
        {
            return _stream.Length == 0 ? new HashSet<T>() : (HashSet<T>)serializer.Deserialize(_stream);
        }
    }
}