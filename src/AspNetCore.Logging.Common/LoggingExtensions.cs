using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace AspNetCore.Logging.Common
{
    public static class LoggingExtensions
    {
        public static void LogTraceContent(this object logObject, ILogger logger, string message, params object[] args)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace($"{string.Format(message, args)}{Environment.NewLine}{JsonSerializer.Serialize(logObject)}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogTraceContent Exception");
            }
        }

        public static void LogTraceContent(this object logObject, ILogger logger, Exception exception, string message, params object[] args)
        {
            try
            {
                if (logger.IsEnabled(LogLevel.Trace))
                {
                    logger.LogTrace(exception, $"{string.Format(message, args)}{Environment.NewLine}{JsonSerializer.Serialize(logObject)}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogTraceContent Exception");
            }
        }
    }
}
