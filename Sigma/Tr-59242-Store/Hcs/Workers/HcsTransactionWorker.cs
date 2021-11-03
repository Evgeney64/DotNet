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
        SysOperationCode _sysOperationCode;
        private readonly ILogger<HcsTransactionWorker> _logger;
        private readonly HcsService _hcsService;

        public HcsTransactionWorker(string clientId, SysOperationCode sysOperationCode, HcsService hcsService, ILogger<HcsTransactionWorker> logger)
        {
            _clientId = clientId;
            _sysOperationCode = sysOperationCode;
            _logger = logger;
            _hcsService = hcsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //return;
                _logger.LogInformation($"HcsTransactionWorker for clientId {_clientId} running at: {DateTimeOffset.Now}");
                try
                {
                    Guid? transactionGuid = await _hcsService.ExecuteTransactionAsync(_clientId, _sysOperationCode);
                    if (transactionGuid == null)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }

                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"HcsTransactionWorker for clientId {_clientId} error at: {DateTimeOffset.Now}");
                    //await Task.Delay(TimeSpan.FromSeconds(30));
                }
            }
        }
    }
}
