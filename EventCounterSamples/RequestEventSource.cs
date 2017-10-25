using System.Diagnostics.Tracing;

namespace EventCounterSamples
{
    [EventSource(Name = "Sample-EventSource")]
    internal class RequestEventSource : EventSource
    {
        public static readonly RequestEventSource Log = new RequestEventSource();
        private readonly EventCounter _requestsStarted;
        private readonly EventCounter _requestsCompleted;
        private readonly EventCounter _requestDuration;

        private RequestEventSource()
        {
            _requestsStarted = new EventCounter("RequestsStarted", this);
            _requestsCompleted = new EventCounter("RequestsCompleted", this);
            _requestDuration = new EventCounter("RequestDuration", this);
        }

        [Event(eventId: 1, Level = EventLevel.Informational, Keywords = Keywords.RequestEvents, Message = "Started request: {0}")]
        public void StartRequest(string requestUrl)
        {
            if (IsEnabled())
            {
                if (IsEnabled(EventLevel.LogAlways, Keywords.Counters))
                {
                    _requestsStarted.WriteMetric(1.0f);
                }

                if (IsEnabled(EventLevel.Informational, Keywords.RequestEvents))
                {
                    WriteEvent(1, requestUrl);
                }
            }
        }

        [Event(eventId: 2, Level = EventLevel.Informational, Keywords = Keywords.RequestEvents, Message = "Completed request: {0} in {1}ms")]
        public void EndRequest(string requestUrl, float durationInMilliseconds)
        {
            if (IsEnabled())
            {
                if (IsEnabled(EventLevel.LogAlways, Keywords.Counters))
                {
                    _requestsCompleted.WriteMetric(1.0f);
                    _requestDuration.WriteMetric(durationInMilliseconds);
                }

                if (IsEnabled(EventLevel.Informational, Keywords.RequestEvents))
                {
                    WriteEvent(2, requestUrl, durationInMilliseconds);
                }
            }
        }

        public static class Keywords
        {
            public const EventKeywords Counters = (EventKeywords)0x1;
            public const EventKeywords RequestEvents = (EventKeywords)0x2;
        }
    }
}
