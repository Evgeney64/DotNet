using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hcs
{
    public class HcsTransactionWorker : BackgroundService
    {
        private readonly string _clientId;
        private readonly ILogger<HcsTransactionWorker> _logger;
        private readonly HcsService _hcsService;

        public HcsTransactionWorker(string clientId, HcsService hcsService, ILogger<HcsTransactionWorker> logger)
        {
            _clientId = clientId;
            _logger = logger;
            _hcsService = hcsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"HcsTransactionWorker for clientId {_clientId} running at: {DateTimeOffset.Now}");
                try
                {
                    Guid? transactionGuid = await _hcsService.ExecuteTransactionAsync(_clientId, SysOperationCode.Unknown);
                    if (transactionGuid == null)
                    {
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"HcsTransactionWorker for clientId {_clientId} error at: {DateTimeOffset.Now}");
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }
    }
}
