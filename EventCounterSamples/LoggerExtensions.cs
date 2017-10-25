using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EventCounterSamples
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> _startRequest =
            LoggerMessage.Define<string>(LogLevel.Information, new EventId(0, nameof(StartRequest)), "Started request: {requestUrl}");

        private static readonly Action<ILogger, string, TimeSpan, int, Exception> _endRequest =
            LoggerMessage.Define<string, TimeSpan, int>(LogLevel.Information, new EventId(1, nameof(EndRequest)), "Completed request: {requestUrl} in {duration} with status {statusCode}");

        public static RequestEvent StartRequest(this ILogger<SampleApp> logger, string requestUrl)
        {
            _startRequest(logger, requestUrl, null);

            RequestEventSource.Log.StartRequest(requestUrl);

            return new RequestEvent(logger, requestUrl, Stopwatch.StartNew());
        }

        public static void EndRequest(this ILogger<SampleApp> logger, string requestUrl, int statusCode, TimeSpan duration)
        {
            _endRequest(logger, requestUrl, duration, statusCode, null);

            // EventCounters use floats, not doubles :(
            RequestEventSource.Log.EndRequest(requestUrl, statusCode, (float)duration.TotalMilliseconds);
        }

        public struct RequestEvent
        {
            private readonly ILogger<SampleApp> _logger;
            private readonly string _requestUrl;
            private readonly Stopwatch _stopwatch;

            public RequestEvent(ILogger<SampleApp> logger, string requestUrl, Stopwatch stopwatch)
            {
                _logger = logger;
                _requestUrl = requestUrl;
                _stopwatch = stopwatch;
            }

            public void End(int statusCode)
            {
                _stopwatch.Stop();
                _logger.EndRequest(_requestUrl, statusCode, _stopwatch.Elapsed);
            }
        }
    }
}
