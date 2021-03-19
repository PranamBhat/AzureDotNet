using System;
using Microsoft.Extensions.Logging;

namespace Pranam.Restme.Utils
{
    public static class RestmeLogger
    {
        public static ILogger Logger { get; set; }

        public static void LogError(string errorMessage, Exception ex = null, int eventId = 0)
        {
            Logger?.LogError(eventId, ex, errorMessage);
        }

        public static void LogInfo(string info, Exception ex = null, int eventId = 0)
        {
            Logger?.LogInformation(eventId, ex, info);
        }

        public static void LogDebug(string debugInfo, Exception ex = null, int eventId = 0)
        {
            Logger?.LogDebug(eventId, debugInfo, ex);
        }

        public static void LogFatal(string fatalInfo, Exception ex = null, int eventId = 0)
        {
            Logger?.LogCritical(eventId, fatalInfo, ex);
        }
    }
}