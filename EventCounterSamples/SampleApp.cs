using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EventCounterSamples
{
    internal class SampleApp
    {
        private readonly ILogger<SampleApp> _logger;

        public SampleApp(ILogger<SampleApp> logger)
        {
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                var rando = new Random();
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Generate a fake "request"
                    var requestUrl = $"http://localhost/request/{rando.Next(100)}";

                    // Record the start of the request
                    using (_logger.StartRequest(requestUrl))
                    {
                        // Wait for a random interval
                        await Task.Delay(rando.Next(100) * 10, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
