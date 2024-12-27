using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMG.Logging
{
    public static class LogSystem
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static LogSystem()
        {
            // Đảm bảo thư mục Logs tồn tại
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Khởi tạo log4net
            XmlConfigurator.Configure(new FileInfo("App.config")); // hoặc Web.config
        }

        public static void Info(string message)
        {
            if (log.IsInfoEnabled)
                log.Info(message);
        }

        public static void Warning(string message)
        {
            if (log.IsWarnEnabled)
                log.Warn(message);
        }

        public static void Error(string message, Exception ex = null)
        {
            if (log.IsErrorEnabled)
                log.Error(message, ex);
        }
        public static void Error(Exception ex)
        {
            if (log.IsErrorEnabled)
                log.Error(ex);
        }
        public static void Debug(string message)
        {
            if (log.IsDebugEnabled)
                log.Debug(message);
        }
    }
}
