using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace EventCounterSamples
{
    internal class SampleEventListener : EventListener
    {
        private readonly bool _enableCounters;
        private readonly bool _enableEvents;

        public SampleEventListener(bool enableCounters, bool enableEvents)
        {
            _enableCounters = enableCounters;
            _enableEvents = enableEvents;
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // This is called during the constructor of SampleEventSource so we can't access anything on it!
            if (eventSource.Name.StartsWith("Sample-"))
            {
                if (_enableCounters || _enableEvents)
                {
                    var keywords = EventKeywords.None;
                    var args = new Dictionary<string, string>();

                    if (_enableCounters)
                    {
                        keywords |= RequestEventSource.Keywords.Counters;
                        args["EventCounterIntervalSec"] = "5";
                    }

                    if (_enableEvents)
                    {
                        keywords |= RequestEventSource.Keywords.RequestEvents;
                    }

                    EnableEvents(eventSource, EventLevel.Informational, keywords, args);
                }
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
