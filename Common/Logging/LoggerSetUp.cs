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
            var fileTarget = new FileTarget("target2")
            {
                FileName = "${basedir}/Logs/logfile.txt",
                Layout = "${longdate} | ${level:uppercase=true} | ${message} |  ${exception}",
                DeleteOldFileOnStartup = true                
            };
            config.AddTarget(fileTarget);

            // Step 3. Define rules
            config.AddRuleForOneLevel(LogLevel.Error, fileTarget); // only errors to file
            config.AddRuleForOneLevel(LogLevel.Warn, fileTarget); // only errors to file
            config.AddRuleForOneLevel(LogLevel.Fatal, fileTarget); // only errors to file
            config.AddRuleForAllLevels(consoleTarget); // all to console

            // Step 4. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}
