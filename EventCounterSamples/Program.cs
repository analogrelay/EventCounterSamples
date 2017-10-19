using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EventCounterSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            Console.WriteLine("Press Ctrl-C to stop");
            Console.CancelKeyPress += (_, a) =>
            {
                // If cancel has already been pressed, let the process be killed.
                a.Cancel = !cts.IsCancellationRequested;
                cts.Cancel();
            };

            var cancellationToken = cts.Token;

            //await RunSimpleSample(cancellationToken);
            //await RunDisposableSample(cancellationToken);
            await RunActivitySample(cancellationToken);
        }

        private static async Task RunActivitySample(CancellationToken cancellationToken)
        {
            var listener = new SampleEventListener();
            var rando = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                var activity = ActivityEventSource.Log.Request("Foo");
                try
                {
                    await Task.Delay(rando.Next(1, 5) * 100);
                }
                finally
                {
                    activity.End(200);
                }
            }
        }

        private static async Task RunDisposableSample(CancellationToken cancellationToken)
        {
            var listener = new SampleEventListener();
            var rando = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                using (DisposableEventSource.Log.Request("Foo"))
                {
                    await Task.Delay(rando.Next(1, 5) * 100);
                }
            }
        }

        private static async Task RunSimpleSample(CancellationToken cancellationToken)
        {
            var listener = new SampleEventListener();
            var rando = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                SimpleEventSource.Log.RequestStarted("Foo");

                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await Task.Delay(rando.Next(1, 5) * 100);
                }
                finally
                {
                    SimpleEventSource.Log.RequestCompleted("Foo", 200, stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
