namespace SPU.WinService.VitualSetting.Core
{
    using System.IO;

    public static class Utilities
    {
        public static string ReadTextFile(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
