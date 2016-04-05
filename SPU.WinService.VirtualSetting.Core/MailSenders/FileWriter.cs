// --------------------------------------------------------------------------
// <copyright file="FileWriter.cs" company="Nexcel Solutions Vietnam">
// Copyright (coffee) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2016.07.23">Created</change>
// </history>

namespace SPU.WinService.VirtualSetting.Core.MailSenders
{
    using System.IO;

    public class FileWriter
    {
        public static void WriteFile(string content)
        {
            File.WriteAllText("suspicious-statistics.txt", content);
        }
    }
}