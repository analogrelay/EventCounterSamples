using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace EventCounterSamples
{
    internal class SampleEventListener : EventListener
    {
        public SampleEventListener()
        {
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // This is called during the constructor of SampleEventSource so we can't access anything on it!
            if (eventSource.Name == EventSource.GetName(typeof(SampleEventSource)))
            {
                // Everything
                //EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.All, new Dictionary<string, string>() {
                //    { "EventCounterIntervalSec", "1" }
                //});

                // Counters only
                //EnableEvents(eventSource, EventLevel.LogAlways, SampleEventSource.Keywords.Counters, new Dictionary<string, string>() {
                //    { "EventCounterIntervalSec", "1" }
                //});

                // Events only
                EnableEvents(eventSource, EventLevel.LogAlways, SampleEventSource.Keywords.RequestEvents);
            }
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            var message = $"event: {eventData.EventName}({eventData.EventId})[{eventData.Level}] {FormatPayload(eventData)}";
            if (!string.IsNullOrEmpty(eventData.Message))
            {
                message += Environment.NewLine +
                    $"       {string.Format(eventData.Message, eventData.Payload.ToArray())}";
            }
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(message);
            Console.ForegroundColor = oldColor;
        }

        private string FormatPayload(EventWrittenEventArgs eventData)
        {
            var items = FormatPayloadItems(eventData.PayloadNames, eventData.Payload, prefix: null);
            return string.Join(", ", items);
        }

        private IEnumerable<string> FormatPayloadItems(IEnumerable<string> payloadNames, IEnumerable<object> payload, string prefix)
        {
            return Enumerable.Zip(payloadNames, payload, (name, value) =>
            {
                return FormatPayloadItem(string.IsNullOrEmpty(prefix) ? name : $"{prefix}.{name}", value);
            }).SelectMany(s => s);
        }

        private IEnumerable<string> FormatPayloadItem(string name, object value)
        {
            if (value is IDictionary<string, object> dict)
            {
                return FormatPayloadItems(dict.Keys, dict.Values, name);
            }
            else
            {
                return new[] { $"{name}: {value.ToString()}" };
            }
        }
    }
}