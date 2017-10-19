using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventCounterSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var listener = new SampleEventListener();

            var cts = new CancellationTokenSource();
            var rando = new Random();

            Console.WriteLine("Press Ctrl-C to stop");
            Console.CancelKeyPress += (_, a) =>
            {
                // If cancel has already been pressed, let the process be killed.
                a.Cancel = !cts.IsCancellationRequested;
                cts.Cancel();
            };

            while (!cts.IsCancellationRequested)
            {
                SampleEventSource.Log.RequestStarted("Foo");

                await Task.Delay(rando.Next(1, 5) * 100);

                SampleEventSource.Log.RequestCompleted("Foo", 200);
            }
        }
    }
}
