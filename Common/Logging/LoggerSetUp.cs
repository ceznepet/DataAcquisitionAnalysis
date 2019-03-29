using NLog;
using NLog.Config;
using NLog.Targets;
using System;

namespace Common.Logging
{
    public class LoggerSetUp
    {
        public static void SetUpLogger()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets
            var consoleTarget = new ColoredConsoleTarget("target1")
            {
                Layout = @"${longdate} | ${level:uppercase=true} | ${message}"
            };
            config.AddTarget(consoleTarget);
            var time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            var fileTarget = new FileTarget("target2")
            {
                FileName = "${basedir}/Logs/logfile_"+ time +".txt",
                Layout = "${longdate} | ${level:uppercase=true} | ${message} |  ${exception}",
                DeleteOldFileOnStartup = false                
            };
            config.AddTarget(fileTarget);

            config.AddRuleForAllLevels(consoleTarget);
            config.AddRuleForAllLevels(fileTarget);

            // Step 4. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}
