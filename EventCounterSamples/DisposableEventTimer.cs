using System;
using System.Diagnostics;

namespace EventCounterSamples
{
    internal struct DisposableEventTimer : IDisposable
    {
        private readonly Action<object, TimeSpan> _endAction;
        private readonly object _state;
        private readonly Stopwatch _stopwatch;

        public DisposableEventTimer(Action<object, TimeSpan> endAction, object state)
        {
            _endAction = endAction;
            _state = state;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _endAction(_state, _stopwatch.Elapsed);
        }
    }
}
