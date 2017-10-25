using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EventCounterSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Arg parsing
            var enableCounters = false;
            var enableEvents = false;
            var enableLogging = false;

            for (var i = 0; i < args.Length; i += 1)
            {
                if(args[i] == "-c")
                {
                    enableCounters = true;
                }
                else if(args[i] == "-e")
                {
                    enableEvents = true;
                }
                else if(args[i] == "-l")
                {
                    enableLogging = true;
                }
            }

            Console.WriteLine($"Counters={enableCounters};Events={enableEvents};Logging={enableLogging}");

            // Start the listener
            new SampleEventListener(enableCounters, enableEvents);

            var cts = new CancellationTokenSource();

            Console.WriteLine("Press Ctrl-C to stop");
            Console.CancelKeyPress += (_, a) =>
            {
                // If cancel has already been pressed, let the process be killed.
                a.Cancel = !cts.IsCancellationRequested;
                cts.Cancel();
            };

            var cancellationToken = cts.Token;

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole(enableLogging ? LogLevel.Trace : LogLevel.None);

            var sampleApp = new SampleApp(loggerFactory.CreateLogger<SampleApp>());
            await sampleApp.RunAsync(cancellationToken);
        }
    }
}
