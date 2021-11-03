using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hcs
{
    public class HcsListWorker : BackgroundService
    {
        private readonly string _clientId;
        private readonly ILogger<HcsListWorker> _logger;
        private readonly HcsService _hcsService;

        public HcsListWorker(string clientId, HcsService hcsService, ILogger<HcsListWorker> logger)
        {
            _clientId = clientId;
            _logger = logger;
            _hcsService = hcsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //return;
                _logger.LogInformation($"HcsListWorker for clientId {_clientId} running at: {DateTimeOffset.Now}");
                try
                {
                    IEnumerable<Guid> transactionGuids = await _hcsService.ExecuteListAsync(_clientId);
                    if (transactionGuids == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"HcsListWorker for clientId {_clientId} error at: {DateTimeOffset.Now}");
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
        }
    }
}
