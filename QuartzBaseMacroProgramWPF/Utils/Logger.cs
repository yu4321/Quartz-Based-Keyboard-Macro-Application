using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Layout;

namespace QuartzBasedMacroProgram.Utils
{
    public static class Logger
    {
        public static readonly ILog LogWriterMessage = GetAppLoggerMessage();
        public static readonly ILog LogWriterException = GetAppLoggerException();
        public static readonly ILog LogWriterStatistics = GetStatisticsLogger();

        static Logger()
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        /// <summary>
        /// AppLogger를 가져온다.
        /// </summary>
        /// <returns>DeviceLogger</returns>
        private static ILog GetAppLoggerMessage()
        {
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "3MB";
            roller.File = $@"logs\{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}.log";

            roller.StaticLogFileName = true;
            roller.Layout = new PatternLayout("%d{yyMMdd HH:mm:ss.fff} %-5p : %m%n");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            DummyLogger dummyILogger = new DummyLogger("AppLog");
            // 요걸 연결안해주면 log4net 안에서 Null참조 예외가 발생한다.
            dummyILogger.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            dummyILogger.Level = log4net.Core.Level.Info;
            dummyILogger.AddAppender(roller);

            return new NSLogMe(dummyILogger);
        }

        private static ILog GetAppLoggerException()
        {
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "3MB";
            roller.File = $@"logs\{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}_ex.log";

            roller.StaticLogFileName = true;
            roller.Layout = new PatternLayout("%d{yyMMdd HH:mm:ss.fff} %-5p : %m%n");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            DummyLogger dummyILogger = new DummyLogger("AppLogEx");
            // 요걸 연결안해주면 log4net 안에서 Null참조 예외가 발생한다.
            dummyILogger.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            dummyILogger.Level = log4net.Core.Level.Info;
            dummyILogger.AddAppender(roller);

            return new NSLogEx(dummyILogger);
        }

        private static ILog GetStatisticsLogger()
        {
            RollingFileAppender roller = new RollingFileAppender();
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 10;
            roller.MaximumFileSize = "10MB";
            roller.File = $@"logs\{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}_statistics.log";

            roller.StaticLogFileName = true;
            roller.Layout = new PatternLayout("%d{yyMMdd HH:mm:ss.fff} %-5p : %m%n");
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            DummyLogger dummyILogger = new DummyLogger("AppLogEx");
            // 요걸 연결안해주면 log4net 안에서 Null참조 예외가 발생한다.
            dummyILogger.Hierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            dummyILogger.Level = log4net.Core.Level.Info;
            dummyILogger.AddAppender(roller);

            return new NSLogStatistics(dummyILogger);
        }
    }

    public sealed class DummyLogger : log4net.Repository.Hierarchy.Logger
    {
        // Methods
        internal DummyLogger(string name)
            : base(name)
        {
        }
    }

    public class NSLogMe : log4net.Core.LogImpl
    {
        public NSLogMe(DummyLogger log) : base(log)
        {
        }
    }

    public class NSLogEx : log4net.Core.LogImpl
    {
        public NSLogEx(DummyLogger log) : base(log)
        {
        }

        public override void Error(object message, Exception exception)
        {
            base.Error(exception);
        }

        public override void Warn(object message, Exception exception)
        {
            base.Warn(exception);
        }
    }

    public class NSLogStatistics : log4net.Core.LogImpl
    {
        public NSLogStatistics(DummyLogger log) : base(log)
        {
        }
    }
}

