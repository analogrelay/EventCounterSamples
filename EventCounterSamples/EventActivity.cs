using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace EventCounterSamples
{
    internal static class EventActivity
    {
        public static EventActivity<T1> Create<T1>(object state, Action<object, T1, TimeSpan> endActivity)
            => new EventActivity<T1>(state, (o, a1, ts) => endActivity(o, a1, ts));
    }

    public struct EventActivity<T1>
    {
        private readonly object _source;
        private readonly Action<object, T1, TimeSpan> _endActivity;
        private readonly Stopwatch _stopwatch;

        public EventActivity(object source, Action<object, T1, TimeSpan> endActivity)
        {
            _source = source;
            _endActivity = endActivity;
            _stopwatch = Stopwatch.StartNew();
        }

        public void End(T1 arg0)
        {
            _stopwatch.Stop();
            _endActivity(_source, arg0, _stopwatch.Elapsed);
        }
    }

    // Ye olde T1,T2; T1,T2,T3; etc.
}
