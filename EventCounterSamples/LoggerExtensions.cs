using System;
using Microsoft.Extensions.Logging;

namespace EventCounterSamples
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _startRequest =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(0, nameof(StartRequest)), "Started request: {requestUrl}");

        private static readonly Action<ILogger, string, TimeSpan, Exception> _endRequest =
            LoggerMessage.Define<string, TimeSpan>(LogLevel.Information, new EventId(1, nameof(EndRequest)), "Completed request: {requestUrl} in {duration}");

        public static void StartRequest(this ILogger<SampleApp> logger, string requestUrl)
        {
            _startRequest(logger, requestUrl, null);

            RequestEventSource.Log.StartRequest(requestUrl);
        }

        public static void EndRequest(this ILogger<SampleApp> logger, string requestUrl, TimeSpan duration)
        {
            _startRequest(logger, requestUrl, null);

            // EventCounters use floats, not doubles :(
            RequestEventSource.Log.EndRequest(requestUrl, (float)duration.TotalMilliseconds);
        }
    }
}
