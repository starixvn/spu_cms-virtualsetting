namespace SPU.WinService.VitualSetting.Core.VirtualSetting.Log
{
    using log4net;
    using log4net.Config;
    using System;
    using System.Data.SqlClient;

    public static class Log4NetService
    {
        private static readonly ILog Log = LogManager.GetLogger("LogService");

        static Log4NetService()
        {
            BasicConfigurator.Configure();
            XmlConfigurator.Configure();
        }

        public static void Info(string info)
        {
            Log.Info(info);
        }

        public static void Error(string message, Exception exception)
        {
            var sqlException = exception as SqlException;
            if (sqlException != null)
            {
                message += "\r\nProcedure: " + sqlException.Procedure;
                message += "\r\nLine Number: " + sqlException.LineNumber;
                message += "\r\nServer: " + sqlException.Server;
            }

            Log.Error(message, exception);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Warning(string message)
        {
            Log.Warn(message);
        }

        public static void Warning(string message, Exception exception)
        {
            var sqlException = exception as SqlException;
            if (sqlException != null)
            {
                message += "\r\nProcedure: " + sqlException.Procedure;
                message += "\r\nLine Number: " + sqlException.LineNumber;
                message += "\r\nServer: " + sqlException.Server;
            }

            Log.Warn(message, exception);
        }
    }
}
