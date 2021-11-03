using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hcs
{
    using Model;
    using System.Security.Cryptography;

    public partial class HcsService
    {
        private readonly IDataStore _dataStore;
        private readonly IDataSource _dataSource;
        private readonly IDataService _dataService;
        private readonly ICryptoService _cryptoService;


        //public HcsService(IDataStore dataStore, IDataSource dataSource)
        //{
        //    _dataStore = dataStore;
        //    _dataSource = dataSource;
        //}

        public HcsService(IDataStore dataStore, IDataSource dataSource, IDataService dataService)
        {
            _dataStore = dataStore;
            _dataSource = dataSource;
            _dataService = dataService;
            _cryptoService = (ICryptoService)Activator.CreateInstance(Type.GetType("Hcs.Cryptography.CryptoProService, Hcs.Cryptography.CryptoPro"));
        }

        public async Task<IEnumerable<Guid>> RunTransactionsAsync(TransactionInfo transactionInfo)
        {
            Guid listGuid = Guid.NewGuid();
            IEnumerable<ObjectInfo> objectList = transactionInfo.Objects;
            if (objectList == null || !objectList.Any())
            {
                objectList = await _dataSource.ListAsync(listGuid, transactionInfo);
            }
            TransactionInfo transactionInfo2 = new TransactionInfo
            {
                ClientId = transactionInfo.ClientId,
                ListGuid = listGuid,
                OperationCode = transactionInfo.OperationCode,
                InitialStatus = transactionInfo.InitialStatus,
                Objects = objectList,
                Params = transactionInfo.Params,
            };
            IEnumerable<Guid> transactionGuids = await _dataStore.CreateTransactionsAsync(transactionInfo2);
            return transactionGuids;
        }
        public async Task<TransactionResultInfo> GetTransactionResultAsync(Guid transactionGuid)
        {
            TransactionResultInfo transactionResult = await _dataStore.GetTransactionResultAsync(transactionGuid);
            return transactionResult;
        }
        public async Task StartTransactionAsync(Guid transactionGuid)
        {
            await _dataStore.SetTransactionStatusAsync(transactionGuid, TransactionStatus.Ready);
        }
        public async Task StopTransactionAsync(Guid transactionGuid)
        {
            await _dataStore.SetTransactionStatusAsync(transactionGuid, TransactionStatus.Suspended);
        }

        public async Task<Guid?> ExecuteTransactionAsync(string clientId, SysOperationCode sysOperationCode)
        {
            TransactionInfo transactionInfo = TransactionInfo.Create(clientId, sysOperationCode);
            TransactionInfo2 transactionInfo2 = await _dataStore.AcquireTransactionAsync(transactionInfo);
            if (transactionInfo2 != null)
            {
                TransactionResultInfo transactionResultInfo = null;
                Guid transactionGuid = transactionInfo2.TransactionGuid;
                SysOperationCode operationCode = transactionInfo2.OperationCode;
                try
                {
                    switch (operationCode)
                    {
                        case SysOperationCode.AccountClose:
                            break;
                        case SysOperationCode.AccountExport:
                            transactionResultInfo = await this.ExportAccountAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.AccountImport:
                            transactionResultInfo = await this.ImportAccountAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.AckImport:
                            transactionResultInfo = await this.ImportAckAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.AttachmentPost:
                            break;
                        case SysOperationCode.ContractExport:
                            break;
                        case SysOperationCode.ContractImport:
                            transactionResultInfo = await this.ImportContractAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.DeviceExport:
                            transactionResultInfo = await this.ExportDeviceAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.DeviceImport:
                            transactionResultInfo = await this.ImportDeviceAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.DeviceValueExport:
                            transactionResultInfo = await this.ExportDeviceValueAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.DeviceValueImport:
                            transactionResultInfo = await this.ImportDeviceValueAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.HouseAnnul:
                            break;
                        case SysOperationCode.HouseExport:
                            transactionResultInfo = await this.ExportHouseAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.HouseImport:
                            transactionResultInfo = await this.ImportHouseAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.NotificationImport:
                            transactionResultInfo = await this.ImportNotificationAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.NsiExport:
                            transactionResultInfo = await this.ExportNsiAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.OrderImport:
                            transactionResultInfo = await this.ImportOrderAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.OrganizationExport:
                            transactionResultInfo = await this.ExportOrganizationAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.PaymentDocumentExport:
                            transactionResultInfo = await this.ExportPaymentDocumentAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.PaymentDocumentImport:
                            transactionResultInfo = await this.ImportPaymentDocumentAsync(transactionGuid, null);
                            break;
                        case SysOperationCode.SettlementImport:
                            transactionResultInfo = await this.ImportSettlementAsync(transactionGuid, null);
                            break;
                        default:
                            break;
                    }
                }
                finally
                {
                    TransactionStatus transactionStatus = transactionResultInfo != null ? transactionResultInfo.Status : TransactionStatus.Ready;
                    await _dataStore.ReleaseTransactionAsync(transactionGuid, transactionStatus);
                }
            }
            return transactionInfo2?.TransactionGuid;
        }

        public async Task<IEnumerable<Guid>> ExecuteListAsync(string clientId)
        {
            ListInfo listInfo = await _dataSource.ListInfoAsync();
            if (listInfo != null)
            {
                TransactionInfo transactionInfo = new TransactionInfo
                {
                    ClientId = clientId,
                    OperationCode = listInfo.OperationCode,
                    InitialStatus = TransactionStatus.Ready,
                    Params = listInfo.Params,
                };
                IEnumerable<Guid> transactionGuids = await this.RunTransactionsAsync(transactionInfo);
                return transactionGuids;
            }
            return null;
        }
    }

    public partial class HcsService //: IDisposable
    {
        private bool disposed = false;
        //private readonly IDataSource2 _dataSource;
        //private readonly IDataStore2 _dataStore;
        //private readonly IDataService2 _dataService;

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (this._dataSource != null && this._dataSource is IDisposable)
        //        {
        //            ((IDisposable)this._dataSource).Dispose();
        //        }
        //        if (this._dataStore != null && this._dataStore is IDisposable)
        //        {
        //            ((IDisposable)this._dataStore).Dispose();
        //        }
        //        if (this._dataService != null && this._dataService is IDisposable)
        //        {
        //            ((IDisposable)this._dataService).Dispose();
        //        }
        //    }
        //}
        //public void Dispose()
        //{
        //    if (!this.disposed)
        //    {
        //        this.Dispose(true);
        //        this.disposed = true;
        //    }
        //    GC.SuppressFinalize(this);
        //}

        // note: дубль
        private IEnumerable<ObjectInfoError> createObjectInfoError<TResultData>(IEnumerable<ObjectInfo> objectInfo, IEnumerable<TResultData> resultData)
        {
            if (typeof(IResultEntity).IsAssignableFrom(typeof(TResultData)))
            {
                var objectInfoErrors = resultData
                    .OfType<IResultEntity>()
                    .Where(ss => ss.objectId != null)
                    .SelectMany(
                        ss => ss.ResultErrors,
                        (ss, err) => new ObjectInfoError
                        {
                            ObjectId = ss.objectId,
                            ErrorCode = err.ErrorCode,
                            ErrorDescription = err.ErrorDescription,
                        })
                    .ToArray();
                foreach (var objectInfoErrorsGroup in objectInfoErrors.GroupBy(ss => ss.ObjectId))
                {
                    var objectInfoItem = objectInfo
                        .Where(ss => ss.ObjectId == objectInfoErrorsGroup.Key)
                        .FirstOrDefault();
                    if (objectInfoItem != null)
                    {
                        objectInfoItem.Errors = objectInfoErrorsGroup.ToArray();
                    }
                }
                return objectInfoErrors;
            }
            return Enumerable.Empty<ObjectInfoError>();
        }
        private IEnumerable<ObjectInfoError> checkObjectInfoError(IEnumerable<ObjectInfo> objectInfo)
        {
            var objectInfoErrors = objectInfo
                .Where(ss => ss.Errors != null)
                .SelectMany(ss => ss.Errors)
                .ToArray();
            return objectInfoErrors;
        }
        public async Task<TransactionResultInfo> ImportAsync<TRequestData, TResultData>(
            Guid transactionGuid,
            Action<string, string> log)
            where TRequestData : class, ITransactionEntity
            where TResultData : class, ITransactionEntity
        {
            var executor = new HCSExecutor<TRequestData, TResultData>(this._dataSource, this._dataStore, this._dataService, log);
            var result = await executor.ImportAsync(transactionGuid);
            return result;
        }
        public async Task<TransactionResultInfo> ExportAsync<TRequestData, TResultData>(
            Guid transactionGuid,
            Action<string, string> log)
            where TRequestData : class, ITransactionEntity
            where TResultData : class, ITransactionEntity
        {
            var executor = new HCSExecutor<TRequestData, TResultData>(this._dataSource, this._dataStore, this._dataService, log);
            var result = await executor.ExportAsync(transactionGuid);
            return result;
        }
    }

    public partial class HcsService
    {
        private IEnumerable<AccountExportRequest> createCheckExportRequest(IEnumerable<AccountImportRequest> importRequest)
        {
            var suitableAccounts = importRequest
                .Where(ss => ss.AccountGUID == null);
            if (!suitableAccounts.Any())
            {
                return new AccountExportRequest[0];
            }
            var fiasGuids = suitableAccounts
                .SelectMany(ss => ss.AccountImportRequestPercentPremises)
                .Where(ss => ss.FIASHouseGuid != null)
                .Select(ss => (Guid)ss.FIASHouseGuid)
                .Distinct()
                .ToList();
            if (fiasGuids.Count() == 0)
            {
                throw new CommonException("Не задан ФИАС.", HcsErrorCode.HCS_DAT_10001.ToString());
            }
            if (fiasGuids.Count() > 1)
            {
                throw new CommonException("ФИАС не уникальный.", HcsErrorCode.HCS_DAT_10001.ToString());
            }

            Guid fiasGuid = fiasGuids.First();
            var accountExportRequest = new AccountExportRequest
            {
                TransportGUID = Guid.NewGuid(),
                FIASHouseGuid = fiasGuid,
            };

            return new AccountExportRequest[] { accountExportRequest };
        }
        private IEnumerable<AccountExportResult> createCheckExportResult(IEnumerable<AccountImportRequest> importRequest, IEnumerable<AccountExportResult> exportResult)
        {
            var suitableAccounts = importRequest
                .Where(ss => ss.AccountGUID == null);
            if (!suitableAccounts.Any())
            {
                return new AccountExportResult[0];
            }
            var suitableResults = exportResult
                .Where(ss => ss.AccountNumber != null)
                .Join(suitableAccounts, ss => ss.AccountNumber, ss => ss.AccountNumber, (o, i) => o)
                .ToList();

            return suitableResults;
        }
        private IEnumerable<AccountImportResult> updateImportRequest(IEnumerable<AccountImportRequest> importRequest, IEnumerable<AccountExportResult> exportResult)
        {
            var dataResultErrors = new List<AccountImportResultError>();
            //var dataRequests = new List<AccountImportRequest>();
            foreach (var item in importRequest)
            {
                if (item.AccountGUID == null)
                {
                    var existedItems = exportResult
                        .Where(ss => ss.AccountNumber == item.AccountNumber);
                    if (existedItems.Count() > 1)
                    {
                        dataResultErrors.Add(item.CreateError<AccountImportResultError>("В ГИС ЖКХ существует несколько ЛС с указанным номером."));
                        //continue;
                    }
                    else if (existedItems.Count() == 1)
                    {
                        var existedItem = existedItems.First();
                        item.AccountGUID = existedItem.AccountGUID;
                    }
                }
                //dataRequests.Add(item);
            }

            var dataResults = dataResultErrors.CreateErrorResult<AccountImportResult, AccountImportResultError>();
            return dataResults;
        }

        #region Close
        /*
        private IEnumerable<AccountExportRequest> createCloseExportRequest(IEnumerable<AccountCloseRequest> closeRequest)
        {
            var accountExportRequests = closeRequest
                .GroupBy(ss => ss.FIASHouseGuid)
                .Select(ss => new AccountExportRequest
                {
                    TransportGUID = Guid.NewGuid(),
                    FIASHouseGuid = ss.Key,
                })
                .ToList();
            return accountExportRequests;
        }
        private IEnumerable<AccountExportResult> processCloseExportResult(IEnumerable<AccountExportResult> closeResult)
        {
            // оставляем только дубли и ЛС
            var doubleResult = closeResult
                .Where(r => r.CloseDate == null)
                .GroupBy(r => r.AccountNumber)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g);

            return doubleResult;
        }
        private IEnumerable<AccountImportRequest> createCloseImportRequest(IEnumerable<AccountCloseRequest> closeRequest, IEnumerable<AccountExportResult> exportResult, Guid? lastAccountGuid)
        {
            var multipleAccounts = exportResult
                .Where(ss => ss.AccountNumber != null)
                .GroupBy(ss => ss.AccountNumber)
                .Where(ss => ss.Count() > 1)
                .SelectMany(ss => ss)
                .Join(closeRequest, ss => ss.AccountNumber, ss => ss.AccountNumber, (o, i) => new
                {
                    existedAccount = i,
                    exportAccount = o,
                })
                .Where(a => a.exportAccount.AccountGUID.CompareTo(lastAccountGuid ?? Guid.Empty) > 0)
                .OrderBy(a => a.exportAccount.AccountGUID)
                .Take(100)
                .ToList();

            IList<AccountImportRequest> requestList = new List<AccountImportRequest>();
            foreach (var multipleAccount in multipleAccounts)
            {
                if (multipleAccount.exportAccount.AccountGUID != multipleAccount.existedAccount.AccountGUID)
                {
                    #region
                    var exportAccount = multipleAccount.exportAccount;
                    var existedAccount = multipleAccount.existedAccount;
                    AccountImportRequest request = new AccountImportRequest
                    {
                        objectId = existedAccount.objectId,
                        TransportGUID = exportAccount.TransportGUID,

                        AccountGUID = exportAccount.AccountGUID,
                        AccountNumber = exportAccount.AccountNumber,
                        IsRSOAccount = exportAccount.IsRSOAccount,
                        CreationDate = exportAccount.CreationDate,
                        LivingPersonsNumber = exportAccount.LivingPersonsNumber,
                        TotalSquare = exportAccount.TotalSquare,
                        ResidentialSquare = exportAccount.ResidentialSquare,
                        HeatedArea = exportAccount.HeatedArea,

                        AccountCloseReasonCode = existedAccount.CloseReasonCode,
                        AccountCloseReasonGUID = existedAccount.CloseReasonGUID,
                        CloseDate = existedAccount.CloseDate,
                        CloseDescription = existedAccount.CloseDescription,

                        IsRenter = exportAccount.IsRenter,
                        IsAccountsDivided = exportAccount.IsAccountsDivided,
                        OrgVersionGUID = exportAccount.OrgVersionGUID,
                    };
                    request.AccountImportRequestPercentPremises = exportAccount.AccountExportResultPercentPremises.Select(p => new AccountImportRequestPercentPremise
                    {
                        objectId = existedAccount.objectId,
                        TransportGUID = p.TransportGUID,
                        AccountImportTransportGUID = p.AccountExportTransportGUID,

                        FIASHouseGuid = p.FIASHouseGuid,
                        PremisesGUID = p.PremisesGUID,
                        LivingRoomGUID = p.LivingRoomGUID,
                        SharePercent = p.SharePercent,
                    }).ToList();

                    requestList.Add(request);
                    #endregion
                }
            }

            return requestList;
        }
        private IEnumerable<AccountCloseResult> createCloseResult(IEnumerable<AccountCloseRequest> closeRequest, IEnumerable<AccountExportResult> exportResult, IEnumerable<AccountImportResult> importResult, IEnumerable<ObjectInfo> objectInfo)
        {
            IList<AccountCloseResult> closeResult = new List<AccountCloseResult>();
            foreach (var closeRequestItem in closeRequest)
            {
                var closeRes = new AccountCloseResult
                {
                    objectId = closeRequestItem.objectId,
                    TransportGUID = closeRequestItem.TransportGUID,

                    AccountCloseResultAccounts = new List<AccountCloseResultAccount>(),
                };
                var exportResultGroup = exportResult
                    .Where(ss => ss.AccountNumber == closeRequestItem.AccountNumber);
                foreach (var exportResultItem in exportResultGroup)
                {
                    var importResultItem = importResult
                        .Where(ss => ss.TransportGUID == exportResultItem.TransportGUID)
                        .FirstOrDefault();
                    if (importResultItem != null)
                    {
                        var closeResAccount = new AccountCloseResultAccount
                        {
                            objectId = importResultItem.objectId,
                            TransportGUID = importResultItem.TransportGUID,
                            AccountCloseTransportGUID = closeRes.TransportGUID,

                            AccountNumber = exportResultItem.AccountNumber,
                            AccountGUID = (Guid)importResultItem.AccountGUID,
                            UpdateDate = importResultItem.UpdateDate,

                            AccountCloseResultAccountErrors = new List<AccountCloseResultAccountError>(),
                        };
                        if (importResultItem.AccountImportResultErrors != null)
                        {
                            foreach (var importResultError in importResultItem.AccountImportResultErrors)
                            {
                                var closeResAccountError = new AccountCloseResultAccountError
                                {
                                    objectId = importResultError.objectId,
                                    TransportGUID = importResultError.TransportGUID,
                                    AccountCloseAccountTransportGUID = closeResAccount.TransportGUID,

                                    ErrorCode = importResultError.ErrorCode,
                                    ErrorDescription = importResultError.ErrorDescription,
                                };
                                closeResAccount.AccountCloseResultAccountErrors.Add(closeResAccountError);
                            }
                        }
                        closeRes.AccountCloseResultAccounts.Add(closeResAccount);
                    }
                }
                closeResult.Add(closeRes);
            }

            return closeResult;
        }
        */
        /*
        private IEnumerable<AccountCloseResult> createCloseResult(IEnumerable<AccountCloseRequest> closeRequest, IEnumerable<AccountExportResult> exportResult, IEnumerable<AccountImportResult> importResult, IEnumerable<ObjectInfo> objectInfo)
        {
            IList<AccountCloseResult> closeResult = new List<AccountCloseResult>();
            var importResultGroup = importResult
                .Join(exportResult, ss => ss.TransportGUID, ss => ss.TransportGUID, (o, i) => new
                {
                    importResult = o,
                    i.AccountNumber,
                })
                .GroupBy(ss => ss.AccountNumber)
                .Join(closeRequest, ss => ss.Key, ss => ss.AccountNumber, (o, i) => new
                {
                    importResults = o,
                    AccountNumber = o.Key,
                    i.TransportGUID,
                    i.objectId,
                })
                .ToList();
            foreach (var importResultItem in importResultGroup)
            {
                var closeRes = new AccountCloseResult
                {
                    objectId = importResultItem.objectId,
                    TransportGUID = importResultItem.TransportGUID,

                    AccountCloseResultAccounts = new List<AccountCloseResultAccount>(),
                };
                foreach (var importResult1 in importResultItem.importResults)
                {
                    var closeResAccount = new AccountCloseResultAccount
                    {
                        objectId = importResult1.importResult.objectId,
                        TransportGUID = importResult1.importResult.TransportGUID,
                        AccountCloseTransportGUID = importResultItem.TransportGUID,

                        AccountNumber = importResult1.AccountNumber,
                        AccountGUID = (Guid)importResult1.importResult.AccountGUID,
                        UpdateDate = importResult1.importResult.UpdateDate,

                        AccountCloseResultAccountErrors = new List<AccountCloseResultAccountError>(),
                    };
                    if (importResult1.importResult.AccountImportResultErrors != null)
                    {
                        foreach (var importResultError in importResult1.importResult.AccountImportResultErrors)
                        {
                            var closeResAccountError = new AccountCloseResultAccountError
                            {
                                objectId = importResultError.objectId,
                                TransportGUID = importResultError.TransportGUID,
                                AccountCloseAccountTransportGUID = closeResAccount.TransportGUID,

                                ErrorCode = importResultError.ErrorCode,
                                ErrorDescription = importResultError.ErrorDescription,
                            };
                            closeResAccount.AccountCloseResultAccountErrors.Add(closeResAccountError);
                        }
                    }
                    closeRes.AccountCloseResultAccounts.Add(closeResAccount);
                }
                closeResult.Add(closeRes);
            }

            return closeResult;
        }
        */
        #endregion

        public async Task<TransactionResultInfo> ImportAccountAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            if (_dataSource is ILoggable)
            {
                ((ILoggable)_dataSource).Log = log;
            }
            if (_dataStore is ILoggable)
            {
                ((ILoggable)_dataStore).Log = log;
            }

            TransactionInfo transactionInfo = await this._dataStore.GetTransactionAsync(transactionGuid);
            if (transactionInfo == null)
            {
                return TransactionResultInfo.None(false);
            }
            TransactionResultInfo transactionResultCheck = await this._dataStore.GetTransactionResultAsync(transactionGuid);
            if (transactionResultCheck.Status == TransactionStatus.Completed)
            {
                return transactionResultCheck;
            }

            // получаем текущее состояние транзакции
            var transactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid);

            // выполняем операции
            IEnumerable<AccountExportRequest> requestExportData = null;
            IEnumerable<AccountExportResult> resultExportData = null;
            IEnumerable<AccountImportRequest> requestImportData = null;
            IEnumerable<AccountImportResult> resultImportData = null;

            TransactionLogInfo transactionLog = TransactionLogInfo.Create();
            TransactionResultInfo transactionResult;
            try
            {
                // получение данных
                #region
                if (transactionStateInfo == null)
                {
                    // данные импорта
                    requestImportData = await _dataSource.TakeDataAsync<AccountImportRequest>(transactionGuid, transactionInfo);
                    IEnumerable<ObjectInfoError> objectInfoError = this.checkObjectInfoError(transactionInfo.Objects);
                    // нужен ли экспорт
                    if (transactionInfo.TryGetParamValue("CheckExistence", out string checkExistence))
                    {
                        requestExportData = this.createCheckExportRequest(requestImportData);
                    }
                    if (requestExportData != null && requestExportData.Any())
                    {
                        // сохраняем запрос на экспорт в БД
                        var stateGUID = Guid.NewGuid();
                        var stateData = JsonHelper.Serialize(requestImportData.ToList());
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Composed, stateData, stateGUID);
                        await _dataStore.SaveDataAsync(requestExportData, stateGUID);
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    else
                    {
                        // сохраняем запрос на импорт в БД
                        var stateGUID = Guid.NewGuid();
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Composed, null, stateGUID);
                        await _dataStore.SaveDataAsync(requestImportData, stateGUID);
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                }
                #endregion

                // экспорт
                #region
                if (transactionStateInfo.OperationCode == SysOperationCode.AccountExport)
                {
                    // запрос сформирован
                    #region
                    if (transactionStateInfo.State == TransactionState.Composed)
                    {
                        // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestExportData == null)
                        {
                            requestExportData = await _dataStore.ReadDataAsync<AccountExportRequest>(stateGUID);
                        }

                        var asyncStateGUID = await _dataService.ExportAsync<AccountExportRequest, AccountExportResult>(requestExportData.FirstOrDefault(), stateGUID, log);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Sent, null, stateGUID, asyncStateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                    // запрос отправлен
                    #region
                    if (transactionStateInfo.State == TransactionState.Sent)
                    {
                        // получаем ответ от ГИС ЖКХ и сохраняем в БД (сохраняем только нужные номера ЛС)
                        if (requestImportData == null)
                        {
                            var exportTransactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid, SysOperationCode.AccountExport, TransactionState.Composed);
                            requestImportData = JsonHelper.Deserialize<List<AccountImportRequest>>(exportTransactionStateInfo.Data);
                        }
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestExportData == null)
                        {
                            requestExportData = await _dataStore.ReadDataAsync<AccountExportRequest>(stateGUID);
                        }
                        var asyncRequestState = new AsyncExportState<AccountExportRequest>
                        {
                            State = (Guid)transactionStateInfo.AsyncStateGUID,
                            Data = requestExportData.First(),
                        };
                        try
                        {
                            resultExportData = await _dataService.ExportStateAsync<AccountExportRequest, AccountExportResult>(asyncRequestState, log);
                        }
                        catch (CommonException e)
                        {
                            if (e.Code == ServiceError.INT002012.ToString())
                            {
                                resultExportData = Enumerable.Empty<AccountExportResult>();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        resultExportData = this.createCheckExportResult(requestImportData, resultExportData);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Recieved, null, stateGUID);
                        await _dataStore.SaveDataAsync(resultExportData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                    // запрос получен
                    #region
                    if (transactionStateInfo.State == TransactionState.Recieved)
                    {
                        // формируем запрос на импорт
                        if (requestImportData == null)
                        {
                            var exportTransactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid, SysOperationCode.AccountExport, TransactionState.Composed);
                            requestImportData = JsonHelper.Deserialize<List<AccountImportRequest>>(exportTransactionStateInfo.Data);
                        }
                        if (resultExportData == null)
                        {
                            resultExportData = await _dataStore.ReadDataAsync<AccountExportResult>(transactionStateInfo.StateGUID);
                        }
                        resultImportData = this.updateImportRequest(requestImportData, resultExportData);
                        var stateGUID = Guid.NewGuid();
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Composed, null, stateGUID);
                        await _dataStore.SaveDataAsync(requestImportData, stateGUID);
                        await _dataStore.SaveDataAsync(resultImportData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                }
                #endregion

                // импорт
                #region
                if (transactionStateInfo.OperationCode == SysOperationCode.AccountImport)
                {
                    // запрос сформирован
                    #region
                    if (transactionStateInfo.State == TransactionState.Composed)
                    {
                        // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestImportData == null)
                        {
                            requestImportData = await _dataStore.ReadDataAsync<AccountImportRequest>(stateGUID);
                        }
                        if (resultImportData == null)
                        {
                            resultImportData = await _dataStore.ReadDataAsync<AccountImportResult>(stateGUID);
                        }
                        requestImportData = requestImportData
                            .Where(ss => !resultImportData
                                .Select(ss1 => ss1.TransportGUID)
                                .Contains(ss.TransportGUID))
                            .ToList();
                        var asyncState = await _dataService.ImportAsync<AccountImportRequest, AccountImportResult>(requestImportData, stateGUID, log);
                        if (asyncState.RequestState != null)
                        {
                            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Sent, null, stateGUID, asyncState.RequestState.State);
                        }
                        else
                        {
                            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Recieved, null, stateGUID);
                        }
                        await _dataStore.SaveDataAsync(asyncState.ResultData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                        resultImportData = resultImportData
                            .Concat(asyncState.ResultData)
                            .ToList();
                    }
                    #endregion
                    // запрос отправлен
                    #region
                    if (transactionStateInfo.State == TransactionState.Sent)
                    {
                        // получаем ответ от ГИС ЖКХ и сохраняем в БД
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestImportData == null)
                        {
                            requestImportData = await _dataStore.ReadDataAsync<AccountImportRequest>(stateGUID);
                        }
                        if (resultImportData == null)
                        {
                            resultImportData = await _dataStore.ReadDataAsync<AccountImportResult>(stateGUID);
                        }
                        requestImportData = requestImportData
                            .Where(ss => !resultImportData
                                .Select(ss1 => ss1.TransportGUID)
                                .Contains(ss.TransportGUID))
                            .ToList();
                        var asyncRequestState = new AsyncImportState<AccountImportRequest>
                        {
                            State = (Guid)transactionStateInfo.AsyncStateGUID,
                            Data = requestImportData,
                        };

                        var resultStateImportData = await _dataService.ImportStateAsync<AccountImportRequest, AccountImportResult>(asyncRequestState, log);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Recieved, null, stateGUID);
                        await _dataStore.SaveDataAsync(resultStateImportData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                        resultImportData = resultImportData
                            .Concat(resultStateImportData)
                            .ToList();
                    }
                    #endregion
                    // запрос получен
                    #region
                    if (transactionStateInfo.State == TransactionState.Recieved)
                    {
                        // берем данные из БД и передаем внешнему модулю
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (resultImportData == null)
                        {
                            resultImportData = await _dataStore.ReadDataAsync<AccountImportResult>(stateGUID);
                        }
                        IEnumerable<ObjectInfoError> objectInfoError = this.createObjectInfoError(transactionInfo.Objects, resultImportData);
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        await _dataSource.PassDataAsync(resultImportData, stateGUID, transactionInfo);
                        //var stateData = JsonHelper.Serialize(objectInfo.ToList());
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Transferred, null, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                    // запрос передан
                    #region
                    if (transactionStateInfo.State == TransactionState.Transferred)
                    {
                        //objectInfo = JsonHelper.Deserialize<List<ObjectInfo>>(transactionStateInfo.Data);
                        //objectInfo = await _dataStore.GetTransactionObjectsAsync(transactionGuid);
                    }
                    #endregion
                }
                #endregion

                transactionResult = TransactionResultInfo.Create(transactionInfo.Objects);
            }
            catch (CommonException ex)
            {
                transactionLog.Error = ErrorInfo.Create(ex.Code, ex.Message, ex.ToString());
                transactionResult = TransactionResultInfo.TransactionError(transactionLog.Error, !ex.CanRestart);
                //throw;
            }
            catch (Exception ex)
            {
                transactionLog.Error = ErrorInfo.Create(ex.HResult.ToString(), ex.Message, ex.ToString());
                throw;
            }
            finally
            {
                await _dataStore.SetTransactionLogAsync(transactionGuid, transactionLog);
            }

            if (transactionResult.Status == TransactionStatus.Completed)
            {
                await _dataStore.SetTransactionResultAsync(transactionGuid, transactionResult);
            }
            return transactionResult;
        }
        public async Task<TransactionResultInfo> ExportAccountAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<AccountExportRequest, AccountExportResult>(transactionGuid, log);
            return result;
        }

        //public async Task<TransactionResultInfo> CloseAccountAsync(
        //    Guid transactionGuid,
        //    string transactionData,
        //    IEnumerable<ObjectInfo> objectInfo,
        //    Action<string, string> log)
        //{
        //    if (_dataSource is ILoggable)
        //    {
        //        ((ILoggable)_dataSource).Log = log;
        //    }
        //    if (_dataStore is ILoggable)
        //    {
        //        ((ILoggable)_dataStore).Log = log;
        //    }

        //    // создаем транзакицю, если еще не была создана
        //    if (!await _dataStore.IsTransactionExistsAsync(transactionGuid))
        //    {
        //        await _dataStore.CreateTransactionAsync(transactionGuid, TransactionInfo.Create(SysOperationCode.AccountClose, transactionData));
        //    }
        //    // проверяем результат транзакции
        //    else
        //    {
        //        var transactionResultTest = await this._dataStore.GetTransactionResultAsync(transactionGuid);
        //        if (transactionResultTest.IsCompleted)
        //        {
        //            return transactionResultTest;
        //        }
        //    }
        //    // получаем текущее состояние транзакции
        //    var transactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid);

        //    // выполняем операции
        //    IEnumerable<AccountExportRequest> requestExportData = null;
        //    IEnumerable<AccountExportResult> resultExportData = null;
        //    IEnumerable<AccountImportRequest> requestImportData = null;
        //    IEnumerable<AccountImportResult> resultImportData = null;
        //    IEnumerable<AccountCloseRequest> requestCloseData = null;
        //    IEnumerable<AccountCloseResult> resultCloseData = null;

        //    TransactionLogInfo transactionLog = TransactionLogInfo.Create();
        //    TransactionResultInfo transactionResult;
        //    try
        //    {
        //        // начало
        //        #region
        //        if (transactionStateInfo == null)
        //        {
        //            // формируем запрос на экспорт ЛС по дому и сохраняем его в БД
        //            requestCloseData = await _dataSource.TakeDataAsync<AccountCloseRequest>(transactionGuid, objectInfo);
        //            var stateGUID = transactionGuid;
        //            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountClose, TransactionState.Composed, null, stateGUID);
        //            await _dataStore.SaveDataAsync(requestCloseData, stateGUID);
        //            await _dataStore.SetTransactionObjectsAsync(transactionGuid, objectInfo);
        //            await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);

        //            requestExportData = this.createCloseExportRequest(requestCloseData);
        //            stateGUID = Guid.NewGuid();
        //            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Composed, null, stateGUID);
        //            await _dataStore.SaveDataAsync(requestExportData, stateGUID);
        //            await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
        //        }
        //        #endregion
        //        // экспорт ЛС
        //        #region
        //        if (transactionStateInfo.OperationCode == SysOperationCode.AccountExport)
        //        {
        //            // запрос сформирован
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Composed)
        //            {
        //                // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
        //                var stateGUID = transactionStateInfo.StateGUID;
        //                if (requestExportData == null)
        //                {
        //                    requestExportData = await _dataStore.ReadDataAsync<AccountExportRequest>(stateGUID);
        //                }

        //                var asyncStateGUID = await _dataService.ExportAsync<AccountExportRequest, AccountExportResult>(requestExportData.First(), stateGUID, log);
        //                transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Sent, null, stateGUID, asyncStateGUID);
        //                await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
        //            }
        //            #endregion
        //            // запрос отправлен
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Sent)
        //            {
        //                // получаем ответ от ГИС ЖКХ и сохраняем в БД (сохраняем только незакрытые дубли ЛС)
        //                var stateGUID = transactionStateInfo.StateGUID;
        //                if (requestExportData == null)
        //                {
        //                    requestExportData = await _dataStore.ReadDataAsync<AccountExportRequest>(stateGUID);
        //                }
        //                var asyncRequestState = new AsyncExportState<AccountExportRequest>
        //                {
        //                    State = (Guid)transactionStateInfo.AsyncStateGUID,
        //                    Data = requestExportData.First(),
        //                };
        //                try
        //                {
        //                    resultExportData = await _dataService.ExportStateAsync<AccountExportRequest, AccountExportResult>(asyncRequestState, log);
        //                }
        //                catch (CommonException e)
        //                {
        //                    if (e.Code == ServiceError.INT002012.ToString())
        //                    {
        //                        resultExportData = Enumerable.Empty<AccountExportResult>();
        //                    }
        //                    else
        //                    {
        //                        throw;
        //                    }
        //                }
        //                resultExportData = this.processCloseExportResult(resultExportData);
        //                transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountExport, TransactionState.Recieved, null, stateGUID);
        //                await _dataStore.SaveDataAsync(resultExportData, stateGUID);
        //                await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
        //            }
        //            #endregion
        //        }
        //    #endregion
        //    // начало цикла
        //    #region
        //    Loop:
        //        if (transactionStateInfo.OperationCode == SysOperationCode.AccountExport && transactionStateInfo.State == TransactionState.Recieved || transactionStateInfo.OperationCode == SysOperationCode.AccountImport && transactionStateInfo.State == TransactionState.Recieved)
        //        {
        //            // формируем запрос на закрытие ЛС и сохраняем его в БД или формируем окончательный результат и передаем внешнему модулю
        //            TransactionStateInfo exportTransactionStateInfo = transactionStateInfo.OperationCode == SysOperationCode.AccountExport ?
        //                transactionStateInfo :
        //                await _dataStore.GetTransactionStateAsync(transactionGuid, SysOperationCode.AccountExport, TransactionState.Recieved);
        //            if (resultExportData == null)
        //            {
        //                resultExportData = await _dataStore.ReadDataAsync<AccountExportResult>(exportTransactionStateInfo.StateGUID);
        //            }
        //            if (requestCloseData == null)
        //            {
        //                requestCloseData = await _dataStore.ReadDataAsync<AccountCloseRequest>(transactionGuid);
        //            }
        //            Guid? lastAccountGuid = null;
        //            if (transactionStateInfo.OperationCode == SysOperationCode.AccountImport)
        //            {
        //                lastAccountGuid = JsonHelper.Deserialize<Guid>(transactionStateInfo.Data);
        //            }

        //            var stateGUID = Guid.NewGuid();
        //            requestImportData = this.createCloseImportRequest(requestCloseData, resultExportData, lastAccountGuid);
        //            if (requestImportData.Any())
        //            {
        //                // формируем запрос на закрытие ЛС
        //                transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountImport, TransactionState.Composed, null, stateGUID);
        //                await _dataStore.SaveDataAsync(requestImportData, stateGUID);
        //                await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
        //            }
        //            else
        //            {
        //                // берем данные из БД и передаем внешнему модулю
        //                var resultImportDataAll = new List<AccountImportResult>();
        //                foreach (var importTransactionStateInfo in await _dataStore.GetTransactionStatesAsync(transactionGuid, SysOperationCode.AccountImport, TransactionState.Recieved))
        //                {
        //                    resultImportData = await _dataStore.ReadDataAsync<AccountImportResult>(importTransactionStateInfo.StateGUID);
        //                    resultImportDataAll.AddRange(resultImportData);
        //                }

        //                resultCloseData = this.createCloseResult(requestCloseData, resultExportData, resultImportDataAll, objectInfo);
        //                stateGUID = transactionGuid;
        //                transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AccountClose, TransactionState.Transferred, null, stateGUID);
        //                IEnumerable<ObjectInfoError> objectInfoError = this.createObjectInfoError(objectInfo, resultImportDataAll);
        //                await _dataStore.SaveDataAsync(resultCloseData, stateGUID);
        //                await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
        //                await _dataSource.PassDataAsync(resultCloseData, transactionGuid, objectInfo);
        //                await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
        //            }
        //        }
        //        #endregion
        //        // импорт ЛС
        //        #region
        //        if (transactionStateInfo.OperationCode == SysOperationCode.AccountImport)
        //        {
        //            var stateGUID = transactionStateInfo.StateGUID;
        //            if (requestImportData == null)
        //            {
        //                requestImportData = await _dataStore.ReadDataAsync<AccountImportRequest>(stateGUID);
        //            }
        //            var lastAccountGuid = requestImportData
        //                .OrderBy(ss => ss.AccountGUID)
        //                .Last()
        //                .AccountGUID;

        //            var executor = new HCSExecutor<AccountImportRequest, AccountImportResult>(this._dataSource, this._dataStore, this._dataService, log);
        //            executor.Init(SysOperationCode.AccountImport, transactionGuid, requestImportData, transactionStateInfo);

        //            // запрос сформирован
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Composed)
        //            {
        //                await executor.SendImportDataAsync(stateGUID, null);
        //                transactionStateInfo = executor.CurrentTransactionStateInfo;
        //            }
        //            #endregion
        //            // запрос отправлен
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Sent)
        //            {
        //                await executor.RecieveImportDataAsync(stateGUID, JsonHelper.Serialize(lastAccountGuid));
        //                transactionStateInfo = executor.CurrentTransactionStateInfo;

        //                // зацикливаем
        //                goto Loop;
        //            }
        //            #endregion
        //        }
        //        #endregion

        //        //if (objectInfo == null)
        //        //{
        //        objectInfo = await _dataStore.GetTransactionObjectsAsync(transactionGuid);
        //        //}
        //        transactionResult = TransactionResultInfo.Create(objectInfo);
        //    }
        //    catch (CommonException ex)
        //    {
        //        transactionLog.Error = ErrorInfo.Create(ex.Code, ex.Message, ex.ToString());
        //        transactionResult = TransactionResultInfo.TransactionError(transactionLog.Error, !ex.CanRestart);
        //        //throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        transactionLog.Error = ErrorInfo.Create(ex.HResult.ToString(), ex.Message, ex.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //        await _dataStore.SetTransactionLogAsync(transactionGuid, transactionLog);
        //    }

        //    if (transactionResult.IsCompleted)
        //    {
        //        await _dataStore.SetTransactionResultAsync(transactionGuid, transactionResult);
        //    }
        //    return transactionResult;
        //}
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ImportAckAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<AckImportRequest, AckImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> CancelAckAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<AckImportCancellationRequest, AckImportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ImportSettlementAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<SettlementImportRequest, SettlementImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> AnnulSettlementAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<SettlementImportAnnulmentRequest, SettlementImportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        private async Task<IEnumerable<AttachmentPostRequest>> createUploadRequestAsync(IEnumerable<ContractImportRequest> importRequest)
        {
            var attachmentPostList = new Dictionary<string, AttachmentPostRequest>();
            var suitableContractAttachments = importRequest
                .Where(ss => ss.ContractImportRequestAttachments != null)
                .SelectMany(ss => ss.ContractImportRequestAttachments)
                .Where(ss => ss.AttachmentBody != null)
                .ToList();
            foreach (var contractAttachment in suitableContractAttachments)
            {
                byte[] hash = await _cryptoService.GetHashAsync("", contractAttachment.AttachmentBody);
                string attachmentHASH = BitConverter.ToString(hash).Replace("-", String.Empty);
                AttachmentPostRequest attachmentPost;
                if (!attachmentPostList.TryGetValue(attachmentHASH, out attachmentPost))
                {
                    attachmentPost = new AttachmentPostRequest
                    {
                        objectId = "1",
                        TransactionGUID = contractAttachment.TransactionGUID,
                        TransportGUID = Guid.NewGuid(),
                        Name = contractAttachment.Name,
                        Description = contractAttachment.Description,
                        AttachmentBody = contractAttachment.AttachmentBody,
                        numCopies = 0,
                    };
                    attachmentPostList.Add(attachmentHASH, attachmentPost);
                }
                attachmentPost.numCopies++;
                contractAttachment.AttachmentHASH = attachmentHASH;
                contractAttachment.AttachmentBody = null;
            }

            return attachmentPostList.Values.ToList();
        }
        private async Task<IEnumerable<AttachmentPostResult>> createUploadResultAsync(IEnumerable<AttachmentPostRequest> uploadRequest, IEnumerable<ContractAttachmentData> attachmentData)
        {
            List<AttachmentPostResult> uploadResult = new List<AttachmentPostResult>(uploadRequest.Count());
            foreach (var request in uploadRequest)
            {
                byte[] hash = await _cryptoService.GetHashAsync("", request.AttachmentBody);
                string attachmentHASH = BitConverter.ToString(hash).Replace("-", String.Empty);

                var result = new AttachmentPostResult
                {
                    objectId = request.objectId,
                    TransportGUID = request.TransportGUID,

                    AttachmentHASH = attachmentHASH,
                    AttachmentPostResultCopies = new List<AttachmentPostResultCopy>(),
                };
                foreach (var data in attachmentData.Where(ss => ss.AttachmentTransportGUID == request.TransportGUID))
                {
                    var copyResult = new AttachmentPostResultCopy
                    {
                        objectId = data.Number.ToString(),
                        TransportGUID = Guid.NewGuid(),

                        AttachmentPostTransportGUID = result.TransportGUID,
                        AttachmentGUID = data.AttachmentGUID,
                    };
                    result.AttachmentPostResultCopies.Add(copyResult);
                }
                uploadResult.Add(result);
            }

            return uploadResult;
        }
        private IEnumerable<ContractImportRequest> createRenewedImportRequest(IEnumerable<ContractImportRequest> importRequest, IEnumerable<AttachmentPostResult> uploadResult)
        {
            foreach (var result in uploadResult)
            {
                var suitableContractAttachments = importRequest
                    .Where(ss => ss.ContractImportRequestAttachments != null)
                    .SelectMany(ss => ss.ContractImportRequestAttachments)
                    .Where(ss => ss.AttachmentHASH == result.AttachmentHASH)
                    .ToList();
                for (int i = 0; i < suitableContractAttachments.Count; i++)
                {
                    var copy = result.AttachmentPostResultCopies.FirstOrDefault(ss => ss.objectId == i.ToString());
                    if (copy != null)
                    {
                        suitableContractAttachments[i].AttachmentGUID = copy.AttachmentGUID;
                    }
                }
            }

            return importRequest;
        }

        public async Task<TransactionResultInfo> ImportContractAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            IFileService fileService = this._dataService as IFileService;
            if (fileService == null)
            {
                throw new Exception("Не задан IFileService.");
            }

            if (_dataSource is ILoggable)
            {
                ((ILoggable)_dataSource).Log = log;
            }
            if (_dataStore is ILoggable)
            {
                ((ILoggable)_dataStore).Log = log;
            }

            TransactionInfo transactionInfo = await this._dataStore.GetTransactionAsync(transactionGuid);
            if (transactionInfo == null)
            {
                return TransactionResultInfo.None(false);
            }
            TransactionResultInfo transactionResultCheck = await this._dataStore.GetTransactionResultAsync(transactionGuid);
            if (transactionResultCheck.Status == TransactionStatus.Completed)
            {
                return transactionResultCheck;
            }
            // получаем текущее состояние транзакции
            var transactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid);

            // выполняем операции
            IEnumerable<AttachmentPostRequest> requestAttachmentData = null;
            IEnumerable<AttachmentPostResult> resultAttachmentData = null;
            IEnumerable<ContractImportRequest> requestImportData = null;
            IEnumerable<ContractImportResult> resultImportData = null;

            TransactionLogInfo transactionLog = TransactionLogInfo.Create();
            TransactionResultInfo transactionResult;
            try
            {
                // получение данных
                #region
                if (transactionStateInfo == null)
                {
                    // данные импорта
                    requestImportData = await _dataSource.TakeDataAsync<ContractImportRequest>(transactionGuid, transactionInfo);
                    IEnumerable<ObjectInfoError> objectInfoError = this.checkObjectInfoError(transactionInfo.Objects);
                    // нужен ли аплоад
                    requestAttachmentData = await this.createUploadRequestAsync(requestImportData);
                    if (requestAttachmentData.Any())
                    {
                        // сохраняем запрос на аплоад в БД
                        var stateGUID = Guid.NewGuid();
                        var stateData = JsonHelper.Serialize(requestImportData.ToList());
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AttachmentPost, TransactionState.Composed, stateData, stateGUID);
                        await _dataStore.SaveDataAsync(requestAttachmentData, stateGUID);
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    else
                    {
                        // сохраняем запрос на импорт в БД
                        var stateGUID = Guid.NewGuid();
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Composed, null, stateGUID);
                        await _dataStore.SaveDataAsync(requestImportData, stateGUID);
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                }
                #endregion

                // аплоад
                #region
                if (transactionStateInfo.OperationCode == SysOperationCode.AttachmentPost)
                {
                    var attachmentTransactionStateInfo = await _dataStore.GetTransactionStateAsync(transactionGuid, SysOperationCode.AttachmentPost, TransactionState.Composed);
                    // запрос сформирован или в процессе загрузки
                    #region
                    if (transactionStateInfo.State == TransactionState.Composed || transactionStateInfo.State == TransactionState.Recieved)
                    {
                        if (requestAttachmentData == null)
                        {
                            requestAttachmentData = await _dataStore.ReadDataAsync<AttachmentPostRequest>(attachmentTransactionStateInfo.StateGUID);
                        }

                        Guid stateGUID;
                        ContractAttachmentData currentAttachmentData = transactionStateInfo.State == TransactionState.Recieved ?
                            JsonHelper.Deserialize<ContractAttachmentData>(transactionStateInfo.Data) :
                            new ContractAttachmentData();
                        Guid currentAttachmentTransportGuid = currentAttachmentData.AttachmentTransportGUID;
                        int currentAttachmentNumber = currentAttachmentData.Number;

                        foreach (var attachmentData in requestAttachmentData
                            .OrderBy(ss => ss.TransportGUID)
                            .Where(ss => ss.TransportGUID.CompareTo(currentAttachmentTransportGuid) >= 0))
                        {
                            for (int i = currentAttachmentNumber; i < attachmentData.numCopies; i++)
                            {
                                var attachment = attachmentData.New();
                                await fileService.PostAttachmentAsync(attachment);
                                var data = new ContractAttachmentData
                                {
                                    Number = i,
                                    AttachmentTransportGUID = attachmentData.TransportGUID,
                                    AttachmentGUID = attachment.AttachmentGUID,
                                };
                                var stateData = JsonHelper.Serialize(data);
                                stateGUID = Guid.NewGuid();
                                transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AttachmentPost, TransactionState.Recieved, stateData, stateGUID);
                                await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                            }
                            currentAttachmentNumber = 0;
                        }

                        var contractAttachmentData = (await _dataStore.GetTransactionStatesAsync(transactionGuid, SysOperationCode.AttachmentPost, TransactionState.Recieved))
                            .Select(ss => JsonHelper.Deserialize<ContractAttachmentData>(ss.Data))
                            .ToList();
                        stateGUID = attachmentTransactionStateInfo.StateGUID;
                        resultAttachmentData = await this.createUploadResultAsync(requestAttachmentData, contractAttachmentData);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.AttachmentPost, TransactionState.Transferred, null, stateGUID);
                        await _dataStore.SaveDataAsync(resultAttachmentData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion

                    // данные полностью загружены
                    #region
                    if (transactionStateInfo.State == TransactionState.Transferred)
                    {
                        if (requestImportData == null)
                        {
                            requestImportData = JsonHelper.Deserialize<List<ContractImportRequest>>(attachmentTransactionStateInfo.Data);
                        }
                        if (resultAttachmentData == null)
                        {
                            resultAttachmentData = await _dataStore.ReadDataAsync<AttachmentPostResult>(transactionStateInfo.StateGUID);
                        }

                        // сохраняем запрос на импорт в БД
                        requestImportData = this.createRenewedImportRequest(requestImportData, resultAttachmentData);
                        var stateGUID = Guid.NewGuid();
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Composed, null, stateGUID);
                        await _dataStore.SaveDataAsync(requestImportData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                }
                #endregion

                // импорт
                #region
                if (transactionStateInfo.OperationCode == SysOperationCode.ContractImport)
                {
                    // запрос сформирован
                    #region
                    if (transactionStateInfo.State == TransactionState.Composed)
                    {
                        // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestImportData == null)
                        {
                            requestImportData = await _dataStore.ReadDataAsync<ContractImportRequest>(stateGUID);
                        }
                        //if (resultImportData == null)
                        //{
                        //    resultImportData = _dataStore.ReadData<ContractImportResult>(stateGUID);
                        //}

                        var asyncState = await _dataService.ImportAsync<ContractImportRequest, ContractImportResult>(requestImportData, stateGUID, log);
                        if (asyncState.RequestState != null)
                        {
                            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Sent, null, stateGUID, asyncState.RequestState.State);
                        }
                        else
                        {
                            transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Recieved, null, stateGUID);
                        }
                        resultImportData = asyncState.ResultData;
                        await _dataStore.SaveDataAsync(resultImportData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                    // запрос отправлен
                    #region
                    if (transactionStateInfo.State == TransactionState.Sent)
                    {
                        // получаем ответ от ГИС ЖКХ и сохраняем в БД
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (requestImportData == null)
                        {
                            requestImportData = await _dataStore.ReadDataAsync<ContractImportRequest>(stateGUID);
                        }
                        if (resultImportData == null)
                        {
                            resultImportData = await _dataStore.ReadDataAsync<ContractImportResult>(stateGUID);
                        }
                        var asyncRequestState = new AsyncImportState<ContractImportRequest>
                        {
                            State = (Guid)transactionStateInfo.AsyncStateGUID,
                            Data = requestImportData,
                        };

                        var resultImportStateData = await _dataService.ImportStateAsync<ContractImportRequest, ContractImportResult>(asyncRequestState, log);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Recieved, null, stateGUID);
                        await _dataStore.SaveDataAsync(resultImportStateData, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                        resultImportData = resultImportData
                            .Concat(resultImportStateData)
                            .ToList();
                    }
                    #endregion
                    // запрос получен
                    #region
                    if (transactionStateInfo.State == TransactionState.Recieved)
                    {
                        // берем данные из БД и передаем внешнему модулю
                        var stateGUID = transactionStateInfo.StateGUID;
                        if (resultImportData == null)
                        {
                            resultImportData = await _dataStore.ReadDataAsync<ContractImportResult>(stateGUID);
                        }
                        IEnumerable<ObjectInfoError> objectInfoError = this.createObjectInfoError(transactionInfo.Objects, resultImportData);
                        await _dataSource.PassDataAsync(resultImportData, stateGUID, transactionInfo);
                        //var stateData = JsonHelper.Serialize(objectInfo.ToList());
                        await _dataStore.SetTransactionObjectErrorsAsync(transactionGuid, objectInfoError);
                        transactionStateInfo = TransactionStateInfo.Create(SysOperationCode.ContractImport, TransactionState.Transferred, null, stateGUID);
                        await _dataStore.SetTransactionStateAsync(transactionGuid, transactionStateInfo);
                    }
                    #endregion
                    // запрос отправлен
                    #region
                    if (transactionStateInfo.State == TransactionState.Transferred)
                    {
                        //objectInfo = JsonHelper.Deserialize<List<ObjectInfo>>(transactionStateInfo.Data);
                        //objectInfo = await _dataStore.GetTransactionObjectsAsync(transactionGuid);
                    }
                    #endregion
                }
                #endregion

                transactionResult = TransactionResultInfo.Create(transactionInfo.Objects);
            }
            catch (CommonException ex)
            {
                transactionLog.Error = ErrorInfo.Create(ex.Code, ex.Message, ex.ToString());
                transactionResult = TransactionResultInfo.TransactionError(transactionLog.Error, !ex.CanRestart);
                //throw;
            }
            catch (Exception ex)
            {
                transactionLog.Error = ErrorInfo.Create(ex.HResult.ToString(), ex.Message, ex.ToString());
                throw;
            }
            finally
            {
                await _dataStore.SetTransactionLogAsync(transactionGuid, transactionLog);
            }

            if (transactionResult.Status == TransactionStatus.Completed)
            {
                await _dataStore.SetTransactionResultAsync(transactionGuid, transactionResult);
            }
            return transactionResult;
        }
        public class ContractAttachmentData
        {
            public int Number { get; set; }
            public Guid AttachmentTransportGUID { get; set; }
            public Guid AttachmentGUID { get; set; }
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ImportDeviceAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<DeviceImportRequest, DeviceImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ArchiveDeviceAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<DeviceImportArchiveRequest, DeviceImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ReplaceDeviceAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<DeviceImportReplaceRequest, DeviceImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ExportDeviceAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<DeviceExportRequest, DeviceExportResult>(transactionGuid, log);
            return result;
        }

        public async Task<TransactionResultInfo> ImportDeviceValueAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<DeviceValueImportRequest, DeviceValueImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ExportDeviceValueAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<DeviceValueExportRequest, DeviceValueExportResult>(transactionGuid, log);
            return result;
        }
    }

    #region Annulment
    public enum HouseObjectType
    {
        House = 1,
        Entrance = 2,
        Premise = 3,
        Block = 4,
        LivingRoom = 5,
    }
    public struct HouseAnnulResultCount
    {
        public int Successed;
        public int WithError;
    }
    public struct HouseAnnulResult
    {
        public HouseAnnulResultCount Entrances;
        public HouseAnnulResultCount Premises;
        public HouseAnnulResultCount Blocks;
        public HouseAnnulResultCount LivingRooms;

        public bool HasSuccessResult()
        {
            if (Entrances.Successed > 0 || Premises.Successed > 0 || Blocks.Successed > 0 || LivingRooms.Successed > 0)
            {
                return true;
            }
            return false;
        }
        public bool HasErrorResult()
        {
            if (Entrances.WithError > 0 || Premises.WithError > 0 || Blocks.WithError > 0 || LivingRooms.WithError > 0)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    public partial class HcsService
    {
        private bool ToAnnulment(IHouseAnnulment annulment)
        {
            return annulment.AnnulmentReasonCode != null && annulment.AnnulmentReasonGUID != null;
        }
        public bool ToAnnulment(IEnumerable<HouseImportRequest> houseData)
        {
            foreach (var house in houseData)
            {
                switch ((HouseType)house.houseType)
                {
                    case HouseType.ApartmentHouse:
                        if (house.HouseImportRequestLivingRooms.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        if (house.HouseImportRequestPremises.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        if (house.HouseImportRequestEntrances.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        break;
                    case HouseType.LivingHouse:
                        if (house.HouseImportRequestLivingRooms.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        break;
                    case HouseType.BlockHouse:
                        if (house.HouseImportRequestLivingRooms.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        if (house.HouseImportRequestBlocks.Any(ss => this.ToAnnulment(ss)))
                        {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }
        public HouseAnnulResult CheckResult(IEnumerable<HouseImportResult> houseData)
        {
            HouseAnnulResult result = new HouseAnnulResult();
            foreach (var house in houseData)
            {
                if (house.HouseImportResultEntrances != null)
                {
                    result.Entrances.Successed = house.HouseImportResultEntrances.Count(ss => ss.HouseImportResultEntranceErrors == null || ss.HouseImportResultEntranceErrors.Count == 0);
                    result.Entrances.WithError = house.HouseImportResultEntrances.Count(ss => ss.HouseImportResultEntranceErrors != null && ss.HouseImportResultEntranceErrors.Count > 0);
                }
                if (house.HouseImportResultPremises != null)
                {
                    result.Premises.Successed = house.HouseImportResultPremises.Count(ss => ss.HouseImportResultPremiseErrors == null || ss.HouseImportResultPremiseErrors.Count == 0);
                    result.Premises.WithError = house.HouseImportResultPremises.Count(ss => ss.HouseImportResultPremiseErrors != null && ss.HouseImportResultPremiseErrors.Count > 0);
                }
                if (house.HouseImportResultBlocks != null)
                {
                    result.Blocks.Successed = house.HouseImportResultBlocks.Count(ss => ss.HouseImportResultBlockErrors == null || ss.HouseImportResultBlockErrors.Count == 0);
                    result.Blocks.WithError = house.HouseImportResultBlocks.Count(ss => ss.HouseImportResultBlockErrors != null && ss.HouseImportResultBlockErrors.Count > 0);
                }
                if (house.HouseImportResultLivingRooms != null)
                {
                    result.LivingRooms.Successed = house.HouseImportResultLivingRooms.Count(ss => ss.HouseImportResultLivingRoomErrors == null || ss.HouseImportResultLivingRoomErrors.Count == 0);
                    result.LivingRooms.WithError = house.HouseImportResultLivingRooms.Count(ss => ss.HouseImportResultLivingRoomErrors != null && ss.HouseImportResultLivingRoomErrors.Count > 0);
                }
            }

            return result;
        }
        private bool HasSuccessResult(IEnumerable<HouseImportResult> houseData)
        {
            foreach (var house in houseData)
            {
                if (house.HouseImportResultEntrances != null
                    && house.HouseImportResultEntrances.Count > 0
                    && house.HouseImportResultEntrances.Any(ss => ss.HouseImportResultEntranceErrors == null || ss.HouseImportResultEntranceErrors.Count == 0))
                {
                    return true;
                }
                if (house.HouseImportResultPremises != null
                    && house.HouseImportResultPremises.Count > 0
                    && house.HouseImportResultPremises.Any(ss => ss.HouseImportResultPremiseErrors == null || ss.HouseImportResultPremiseErrors.Count == 0))
                {
                    return true;
                }
                if (house.HouseImportResultBlocks != null
                    && house.HouseImportResultBlocks.Count > 0
                    && house.HouseImportResultBlocks.Any(ss => ss.HouseImportResultBlockErrors == null || ss.HouseImportResultBlockErrors.Count == 0))
                {
                    return true;
                }
                if (house.HouseImportResultLivingRooms != null
                    && house.HouseImportResultLivingRooms.Count > 0
                    && house.HouseImportResultLivingRooms.Any(ss => ss.HouseImportResultLivingRoomErrors == null || ss.HouseImportResultLivingRoomErrors.Count == 0))
                {
                    return true;
                }
            }
            return false;
        }
        private bool HasErrorResult(IEnumerable<HouseImportResult> houseData)
        {
            foreach (var house in houseData)
            {
                if (((IResultEntity)house).ResultErrors.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        private IEnumerable<HouseImportRequestEntrance> EntrancesToAnnul(HouseObjectType annulObject
            , IEnumerable<HouseImportRequestEntrance> annulEntrances
            , IEnumerable<HouseExportResultEntrance> exportEntrances
            , HouseImportRequest importHouse)
        {
            if (annulObject != HouseObjectType.Entrance && annulObject != HouseObjectType.Premise && annulObject != HouseObjectType.LivingRoom)
            {
                throw new ArgumentException("annulObject");
            }

            List<HouseImportRequestEntrance> importEntrances = new List<HouseImportRequestEntrance>();
            if (annulEntrances == null || exportEntrances == null)
            {
                return importEntrances;
            }

            if (annulEntrances.Any(ss => ss.EntranceGUID == null))
            {
                var annulEntrance = annulEntrances.First(ss => ss.EntranceGUID == null);
                if (annulObject == HouseObjectType.Entrance && !this.ToAnnulment(annulEntrance))
                {
                    return importEntrances;
                }
                foreach (var exportEntrance in exportEntrances)
                {
                    HouseImportRequestEntrance importEntrance;
                    if (annulObject != HouseObjectType.Entrance)
                    {
                        var importPremises = this.PremisesToAnnul(annulObject, annulEntrance.HouseImportRequestPremises, exportEntrance.HouseExportResultPremises, importHouse);
                        if (importPremises.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importEntrance = this.Entrance(exportEntrance, annulEntrance, null);
                        foreach (var importPremise in importPremises)
                        {
                            importPremise.HouseImportEntranceTransportGUID = importEntrance.TransportGUID;
                            importEntrance.HouseImportRequestPremises.Add(importPremise);
                        }
                    }
                    else
                    {
                        importEntrance = this.Entrance(exportEntrance, annulEntrance, annulEntrance);
                    }
                    importEntrance.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestEntrances.Add(importEntrance);
                    importEntrances.Add(importEntrance);
                }
            }
            else
            {
                foreach (var annulEntrance in annulEntrances)
                {
                    if (annulObject == HouseObjectType.Entrance && !this.ToAnnulment(annulEntrance))
                    {
                        continue;
                    }

                    var exportEntrance = exportEntrances.FirstOrDefault(ss => ss.EntranceGUID == annulEntrance.EntranceGUID);
                    if (exportEntrance == null)
                    {
                        // ошибка
                        continue;
                    }
                    HouseImportRequestEntrance importEntrance;
                    if (annulObject != HouseObjectType.Entrance)
                    {
                        var importPremises = this.PremisesToAnnul(annulObject, annulEntrance.HouseImportRequestPremises, exportEntrance.HouseExportResultPremises, importHouse);
                        if (importPremises.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importEntrance = this.Entrance(exportEntrance, annulEntrance, null);
                        foreach (var importPremise in importPremises)
                        {
                            importPremise.HouseImportEntranceTransportGUID = importEntrance.TransportGUID;
                            importEntrance.HouseImportRequestPremises.Add(importPremise);
                        }
                    }
                    else
                    {
                        importEntrance = this.Entrance(exportEntrance, annulEntrance, annulEntrance);
                    }
                    importEntrance.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestEntrances.Add(importEntrance);
                    importEntrances.Add(importEntrance);
                }
            }

            return importEntrances;
        }
        private IEnumerable<HouseImportRequestPremise> PremisesToAnnul(HouseObjectType annulObject
            , IEnumerable<HouseImportRequestPremise> annulPremises
            , IEnumerable<HouseExportResultPremise> exportPremises
            , HouseImportRequest importHouse)
        {
            if (annulObject != HouseObjectType.Premise && annulObject != HouseObjectType.LivingRoom)
            {
                throw new ArgumentException("annulObject");
            }

            List<HouseImportRequestPremise> importPremises = new List<HouseImportRequestPremise>();
            if (annulPremises == null || exportPremises == null)
            {
                return importPremises;
            }

            if (annulPremises.Any(ss => ss.PremisesGUID == null))
            {
                var annulPremise = annulPremises.First(ss => ss.PremisesGUID == null);
                if (annulObject == HouseObjectType.Premise && !this.ToAnnulment(annulPremise))
                {
                    return importPremises;
                }
                foreach (var exportPremise in exportPremises)
                {
                    HouseImportRequestPremise importPremise;
                    if (annulObject != HouseObjectType.Premise)
                    {
                        var importLivingRooms = this.LivingRoomsToAnnul(annulObject, annulPremise.HouseImportRequestLivingRooms, exportPremise.HouseExportResultLivingRooms, importHouse);
                        if (importLivingRooms.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importPremise = this.Premise(exportPremise, annulPremise, null);
                        foreach (var importLivingRoom in importLivingRooms)
                        {
                            importLivingRoom.HouseImportPremiseTransportGUID = importPremise.TransportGUID;
                            importPremise.HouseImportRequestLivingRooms.Add(importLivingRoom);
                        }
                    }
                    else
                    {
                        importPremise = this.Premise(exportPremise, annulPremise, annulPremise);
                    }
                    importPremise.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestPremises.Add(importPremise);
                    importPremises.Add(importPremise);
                }
            }
            else
            {
                foreach (var annulPremise in annulPremises)
                {
                    if (annulObject == HouseObjectType.Premise && !this.ToAnnulment(annulPremise))
                    {
                        continue;
                    }

                    var exportPremise = exportPremises.FirstOrDefault(ss => ss.PremisesGUID == annulPremise.PremisesGUID);
                    if (exportPremise == null)
                    {
                        // ошибка
                        continue;
                    }
                    HouseImportRequestPremise importPremise;
                    if (annulObject != HouseObjectType.Premise)
                    {
                        var importLivingRooms = this.LivingRoomsToAnnul(annulObject, annulPremise.HouseImportRequestLivingRooms, exportPremise.HouseExportResultLivingRooms, importHouse);
                        if (importLivingRooms.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importPremise = this.Premise(exportPremise, annulPremise, null);
                        foreach (var importLivingRoom in importLivingRooms)
                        {
                            importLivingRoom.HouseImportPremiseTransportGUID = importPremise.TransportGUID;
                            importPremise.HouseImportRequestLivingRooms.Add(importLivingRoom);
                        }
                    }
                    else
                    {
                        importPremise = this.Premise(exportPremise, annulPremise, annulPremise);
                    }
                    importPremise.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestPremises.Add(importPremise);
                    importPremises.Add(importPremise);
                }
            }

            return importPremises;
        }
        private IEnumerable<HouseImportRequestBlock> BlocksToAnnul(HouseObjectType annulObject
            , IEnumerable<HouseImportRequestBlock> annulBlocks
            , IEnumerable<HouseExportResultBlock> exportBlocks
            , HouseImportRequest importHouse)
        {
            if (annulObject != HouseObjectType.Premise && annulObject != HouseObjectType.LivingRoom)
            {
                throw new ArgumentException("annulObject");
            }

            List<HouseImportRequestBlock> importBlocks = new List<HouseImportRequestBlock>();
            if (annulBlocks == null || exportBlocks == null)
            {
                return importBlocks;
            }

            if (annulBlocks.Any(ss => ss.BlockGUID == null))
            {
                var annulBlock = annulBlocks.First(ss => ss.BlockGUID == null);
                if (annulObject == HouseObjectType.Block && !this.ToAnnulment(annulBlock))
                {
                    return importBlocks;
                }
                foreach (var exportBlock in exportBlocks)
                {
                    HouseImportRequestBlock importBlock;
                    if (annulObject != HouseObjectType.Block)
                    {
                        var importLivingRooms = this.LivingRoomsToAnnul(annulObject, annulBlock.HouseImportRequestLivingRooms, exportBlock.HouseExportResultLivingRooms, importHouse);
                        if (importLivingRooms.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importBlock = this.Block(exportBlock, annulBlock, null);
                        foreach (var importLivingRoom in importLivingRooms)
                        {
                            importLivingRoom.HouseImportBlockTransportGUID = importBlock.TransportGUID;
                            importBlock.HouseImportRequestLivingRooms.Add(importLivingRoom);
                        }
                    }
                    else
                    {
                        importBlock = this.Block(exportBlock, annulBlock, annulBlock);
                    }
                    importBlock.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestBlocks.Add(importBlock);
                    importBlocks.Add(importBlock);
                }
            }
            else
            {
                foreach (var annulBlock in annulBlocks)
                {
                    if (annulObject == HouseObjectType.Block && !this.ToAnnulment(annulBlock))
                    {
                        continue;
                    }

                    var exportBlock = exportBlocks.FirstOrDefault(ss => ss.BlockGUID == annulBlock.BlockGUID);
                    if (exportBlock == null)
                    {
                        // ошибка
                        continue;
                    }
                    HouseImportRequestBlock importBlock;
                    if (annulObject != HouseObjectType.Block)
                    {
                        var importLivingRooms = this.LivingRoomsToAnnul(annulObject, annulBlock.HouseImportRequestLivingRooms, exportBlock.HouseExportResultLivingRooms, importHouse);
                        if (importLivingRooms.Count() == 0)
                        {
                            // ошибка?
                            continue;
                        }
                        importBlock = this.Block(exportBlock, annulBlock, null);
                        foreach (var importLivingRoom in importLivingRooms)
                        {
                            importLivingRoom.HouseImportBlockTransportGUID = importBlock.TransportGUID;
                            importBlock.HouseImportRequestLivingRooms.Add(importLivingRoom);
                        }
                    }
                    else
                    {
                        importBlock = this.Block(exportBlock, annulBlock, annulBlock);
                    }
                    importBlock.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestBlocks.Add(importBlock);
                    importBlocks.Add(importBlock);
                }
            }

            return importBlocks;
        }
        private IEnumerable<HouseImportRequestLivingRoom> LivingRoomsToAnnul(HouseObjectType annulObject
            , IEnumerable<HouseImportRequestLivingRoom> annulLivingRooms
            , IEnumerable<HouseExportResultLivingRoom> exportLivingRooms
            , HouseImportRequest importHouse)
        {
            if (annulObject != HouseObjectType.LivingRoom)
            {
                throw new ArgumentException("annulObject");
            }

            List<HouseImportRequestLivingRoom> importLivingRooms = new List<HouseImportRequestLivingRoom>();
            if (annulLivingRooms == null || exportLivingRooms == null)
            {
                return importLivingRooms;
            }

            if (annulLivingRooms.Any(ss => ss.LivingRoomGUID == null))
            {
                var annulLivingRoom = annulLivingRooms.First(ss => ss.LivingRoomGUID == null);
                if (!this.ToAnnulment(annulLivingRoom))
                {
                    return importLivingRooms;
                }
                foreach (var exportLivingRoom in exportLivingRooms)
                {
                    HouseImportRequestLivingRoom importLivingRoom = this.LivingRoom(exportLivingRoom, annulLivingRoom, annulLivingRoom);
                    importLivingRoom.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestLivingRooms.Add(importLivingRoom);
                    importLivingRooms.Add(importLivingRoom);
                }
            }
            else
            {
                foreach (var annulLivingRoom in annulLivingRooms)
                {
                    if (!this.ToAnnulment(annulLivingRoom))
                    {
                        continue;
                    }

                    var exportLivingRoom = exportLivingRooms.FirstOrDefault(ss => ss.LivingRoomGUID == annulLivingRoom.LivingRoomGUID);
                    if (exportLivingRoom == null)
                    {
                        // ошибка
                        continue;
                    }
                    HouseImportRequestLivingRoom importLivingRoom = this.LivingRoom(exportLivingRoom, annulLivingRoom, annulLivingRoom);
                    importLivingRoom.HouseImportTransportGUID = importHouse.TransportGUID;
                    importHouse.HouseImportRequestLivingRooms.Add(importLivingRoom);
                    importLivingRooms.Add(importLivingRoom);
                }
            }

            return importLivingRooms;
        }

        private HouseImportRequestEntrance Entrance(HouseExportResultEntrance source, ITransactionObjectEntity id, IHouseAnnulment annulment)
        {
            var entrance = new HouseImportRequestEntrance
            {
                objectId = id.objectId,
                TransportGUID = Guid.NewGuid(),
                EntranceGUID = source.EntranceGUID,
                EntranceNum = source.EntranceNum,

                HouseImportRequestPremises = new HashSet<HouseImportRequestPremise>(),
            };
            if (annulment != null)
            {
                entrance.AnnulmentReasonCode = annulment.AnnulmentReasonCode;
                entrance.AnnulmentReasonGUID = annulment.AnnulmentReasonGUID;
                entrance.AnnulmentInfo = annulment.AnnulmentInfo;
            }
            return entrance;
        }
        private HouseImportRequestPremise Premise(HouseExportResultPremise source, ITransactionObjectEntity id, IHouseAnnulment annulment)
        {
            var premise = new HouseImportRequestPremise
            {
                objectId = id.objectId,
                TransportGUID = Guid.NewGuid(),
                PremisesGUID = source.PremisesGUID,
                premiseType = source.premiseType,
                PremisesNum = source.PremisesNum,

                HouseImportRequestLivingRooms = new HashSet<HouseImportRequestLivingRoom>(),
            };
            if (annulment != null)
            {
                premise.AnnulmentReasonCode = annulment.AnnulmentReasonCode;
                premise.AnnulmentReasonGUID = annulment.AnnulmentReasonGUID;
                premise.AnnulmentInfo = annulment.AnnulmentInfo;
            }
            return premise;
        }
        private HouseImportRequestBlock Block(HouseExportResultBlock source, ITransactionObjectEntity id, IHouseAnnulment annulment)
        {
            var block = new HouseImportRequestBlock
            {
                objectId = id.objectId,
                TransportGUID = Guid.NewGuid(),
                BlockGUID = source.BlockGUID,
                blockType = source.blockType,
                BlockNum = source.BlockNum,

                HouseImportRequestLivingRooms = new HashSet<HouseImportRequestLivingRoom>(),
            };
            if (annulment != null)
            {
                block.AnnulmentReasonCode = annulment.AnnulmentReasonCode;
                block.AnnulmentReasonGUID = annulment.AnnulmentReasonGUID;
                block.AnnulmentInfo = annulment.AnnulmentInfo;
            }
            return block;
        }
        private HouseImportRequestLivingRoom LivingRoom(HouseExportResultLivingRoom source, ITransactionObjectEntity id, IHouseAnnulment annulment)
        {
            var livingRoom = new HouseImportRequestLivingRoom
            {
                objectId = id.objectId,
                TransportGUID = Guid.NewGuid(),
                LivingRoomGUID = source.LivingRoomGUID,
                RoomNumber = source.RoomNumber,
            };
            if (annulment != null)
            {
                livingRoom.AnnulmentReasonCode = annulment.AnnulmentReasonCode;
                livingRoom.AnnulmentReasonGUID = annulment.AnnulmentReasonGUID;
                livingRoom.AnnulmentInfo = annulment.AnnulmentInfo;
            }
            return livingRoom;
        }

        public Task<HouseObjectType[]> NormalizeAnnulRequest(IEnumerable<HouseImportRequest> annulRequest)
        {
            if (annulRequest == null || annulRequest.Count() == 0)
            {
                throw new CommonException("Пустой набор данных для аннулирования ОЖФ.", PerformServiceError.HCS_DAT_00001.ToString());
            }
            HouseImportRequest houseData = annulRequest.First();
            if (!Enum.IsDefined(typeof(HouseType), houseData.houseType))
            {
                throw new CommonException("Неизвестный тип дома.", PerformServiceError.HCS_DAT_00001.ToString());
            }

            HashSet<HouseObjectType> annulObjects = new HashSet<HouseObjectType>();
            switch ((HouseType)houseData.houseType)
            {
                case HouseType.ApartmentHouse:
                    {
                        #region ApartmentHouse

                        #region nonResidentialPremises
                        var nonResidentialPremises = houseData.HouseImportRequestPremises
                            .Where(it => it.premiseType == (int)PremisesType.NonResidential)
                            .ToList();
                        if (nonResidentialPremises.Count > 0)
                        {
                            //все записи должны иметь признак "аннулировать"
                            if (nonResidentialPremises.Any(ss => ss.AnnulmentReasonCode == null || ss.AnnulmentReasonGUID == null))
                            {
                                throw new CommonException("У нежилых помещений для аннулирования должны быть заполнены атрибуты AnnulmentReasonCode и AnnulmentReasonGUID.", PerformServiceError.HCS_DAT_00001.ToString());
                            }
                            // если есть запись с типом "все", то она должна быть единственной
                            if (nonResidentialPremises.Any(ss => ss.PremisesGUID == null))
                            {
                                if (nonResidentialPremises.Count > 1)
                                {
                                    throw new CommonException("Нежилое помещение с атрибутом PremisesGUID равным NULL должно быть единственным в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                }
                            }

                            annulObjects.Add(HouseObjectType.Premise);
                        }
                        #endregion

                        #region residentialPremises
                        var residentialPremises = houseData.HouseImportRequestPremises
                            .Where(it => it.premiseType == (int)PremisesType.Residential && it.HouseImportEntranceTransportGUID == null)
                            .ToList();
                        // если есть запись с типом "все", то она должна быть единственной
                        if (residentialPremises.Any(ss => ss.PremisesGUID == null))
                        {
                            if (residentialPremises.Count > 1)
                            {
                                throw new CommonException("Жилое помещение с атрибутом PremisesGUID равным NULL должно быть единственным в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                            }
                        }
                        foreach (var residentialPremise in residentialPremises)
                        {
                            // если помещение не имеет признака "аннулировать", то должен быть задан список комнат для аннулирования
                            if (residentialPremise.AnnulmentReasonCode == null || residentialPremise.AnnulmentReasonGUID == null)
                            {
                                if (residentialPremise.HouseImportRequestLivingRooms.Count == 0 || residentialPremise.HouseImportRequestLivingRooms.Any(ss => ss.AnnulmentReasonCode == null || ss.AnnulmentReasonGUID == null))
                                {
                                    throw new CommonException("У жилого помещения не для аннулирования должен быть задан список комнат для аннулирования с заполненными атрибутами AnnulmentReasonCode и AnnulmentReasonGUID.", PerformServiceError.HCS_DAT_00001.ToString());
                                }
                                // если есть запись с типом "все", то она должна быть единственной
                                if (residentialPremise.HouseImportRequestLivingRooms.Any(ss => ss.LivingRoomGUID == null))
                                {
                                    if (residentialPremise.HouseImportRequestLivingRooms.Count > 1)
                                    {
                                        throw new CommonException("Комната с атрибутом LivingRoomGUID равным NULL должна быть единственной в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                    }
                                }

                                annulObjects.Add(HouseObjectType.LivingRoom);
                            }
                            // если помещение имеет признак "аннулировать", то не должно быть списка комнат
                            else
                            {
                                //if (residentialPremise.HouseImportRequestLivingRooms.Count > 0)
                                //{
                                //    throw new CommonException("У жилого помещения для аннулирования не должно быть списка комнат.", PerformServiceError.HCS_DAT_00001.ToString());
                                //}

                                annulObjects.Add(HouseObjectType.Premise);
                                annulObjects.Add(HouseObjectType.LivingRoom);
                            }
                        }
                        #endregion

                        #region entrances
                        var entrances = houseData.HouseImportRequestEntrances;
                        if (entrances.Count > 0)
                        {
                            // если есть запись с типом "все", то она должна быть единственной
                            if (entrances.Any(ss => ss.EntranceGUID == null))
                            {
                                if (entrances.Count > 1)
                                {
                                    throw new CommonException("Подъезд с атрибутом EntranceGUID равным NULL должен быть единственным в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                }
                            }
                            foreach (var entrance in entrances)
                            {
                                // если подъезд не имеет признака "аннулировать", то должен быть задан список помещений
                                if (entrance.AnnulmentReasonCode == null || entrance.AnnulmentReasonGUID == null)
                                {
                                    var premises = entrance.HouseImportRequestPremises;
                                    // если есть запись с типом "все", то она должна быть единственной
                                    if (premises.Any(ss => ss.PremisesGUID == null))
                                    {
                                        if (premises.Count > 1)
                                        {
                                            throw new CommonException("Жилое помещение с атрибутом PremisesGUID равным NULL должно быть единственным в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                        }
                                    }
                                    foreach (var premise in premises)
                                    {
                                        // если помещение не имеет признака "аннулировать", то должен быть задан список комнат для аннулирования
                                        if (premise.AnnulmentReasonCode == null || premise.AnnulmentReasonGUID == null)
                                        {
                                            if (premise.HouseImportRequestLivingRooms.Count == 0 || premise.HouseImportRequestLivingRooms.Any(ss => ss.AnnulmentReasonCode == null || ss.AnnulmentReasonGUID == null))
                                            {
                                                throw new CommonException("У жилого помещения не для аннулирования должен быть задан список комнат для аннулирования с заполненными атрибутами AnnulmentReasonCode и AnnulmentReasonGUID.", PerformServiceError.HCS_DAT_00001.ToString());
                                            }
                                            // если есть запись с типом "все", то она должна быть единственной
                                            if (premise.HouseImportRequestLivingRooms.Any(ss => ss.LivingRoomGUID == null))
                                            {
                                                if (premise.HouseImportRequestLivingRooms.Count > 1)
                                                {
                                                    throw new CommonException("Комната с атрибутом LivingRoomGUID равным NULL должна быть единственной в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                                }
                                            }

                                            annulObjects.Add(HouseObjectType.LivingRoom);
                                        }
                                        // если помещение имеет признак "аннулировать", то не должно быть списка комнат
                                        else
                                        {
                                            //if (premise.HouseImportRequestLivingRooms.Count > 0)
                                            //{
                                            //    throw new CommonException("У жилого помещения для аннулирования не должно быть списка комнат.", PerformServiceError.HCS_DAT_00001.ToString());
                                            //}

                                            annulObjects.Add(HouseObjectType.Premise);
                                            annulObjects.Add(HouseObjectType.LivingRoom);
                                        }
                                    }
                                }
                                // если подъезд имеет признак "аннулировать", то не должно быть списка помещений
                                else
                                {
                                    //if (entrance.HouseImportRequestPremises.Count > 0)
                                    //{
                                    //    throw new CommonException("У подъезда для аннулирования не должно быть списка помещений.", PerformServiceError.HCS_DAT_00001.ToString());
                                    //}

                                    annulObjects.Add(HouseObjectType.Entrance);
                                    annulObjects.Add(HouseObjectType.Premise);
                                    annulObjects.Add(HouseObjectType.LivingRoom);
                                }
                            }
                        }
                        #endregion

                        #endregion
                    }
                    break;
                case HouseType.LivingHouse:
                    {
                        #region LivingHouse

                        #region livingRooms
                        var livingRooms = houseData.HouseImportRequestLivingRooms;
                        if (livingRooms.Count > 0)
                        {
                            //все записи должны иметь признак "аннулировать"
                            if (livingRooms.Any(ss => ss.AnnulmentReasonCode == null || ss.AnnulmentReasonGUID == null))
                            {
                                throw new CommonException("У комнат для аннулирования должны быть заполнены атрибуты AnnulmentReasonCode и AnnulmentReasonGUID.", PerformServiceError.HCS_DAT_00001.ToString());
                            }
                            // если есть запись с типом "все", то она должна быть единственной
                            if (livingRooms.Any(ss => ss.LivingRoomGUID == null))
                            {
                                if (livingRooms.Count > 1)
                                {
                                    throw new CommonException("Комната с атрибутом LivingRoomGUID равным NULL должна быть единственной в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                }
                            }

                            annulObjects.Add(HouseObjectType.LivingRoom);
                        }
                        #endregion

                        #endregion
                    }
                    break;
                case HouseType.BlockHouse:
                    {
                        #region BlockHouse

                        #region blocks
                        var blocks = houseData.HouseImportRequestBlocks;
                        // если есть запись с типом "все", то она должна быть единственной
                        if (blocks.Any(ss => ss.BlockGUID == null))
                        {
                            if (blocks.Count > 1)
                            {
                                throw new CommonException("Блок с атрибутом BlockGUID равным NULL должно быть единственным в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                            }
                        }
                        foreach (var block in blocks)
                        {
                            // если блок не имеет признака "аннулировать", то должен быть задан список комнат для аннулирования
                            if (block.AnnulmentReasonCode == null || block.AnnulmentReasonGUID == null)
                            {
                                if (block.HouseImportRequestLivingRooms.Count == 0 || block.HouseImportRequestLivingRooms.Any(ss => ss.AnnulmentReasonCode == null || ss.AnnulmentReasonGUID == null))
                                {
                                    throw new CommonException("У блока не для аннулирования должен быть задан список комнат для аннулирования с заполненными атрибутами AnnulmentReasonCode и AnnulmentReasonGUID.", PerformServiceError.HCS_DAT_00001.ToString());
                                }
                                // если есть запись с типом "все", то она должна быть единственной
                                if (block.HouseImportRequestLivingRooms.Any(ss => ss.LivingRoomGUID == null))
                                {
                                    if (block.HouseImportRequestLivingRooms.Count > 1)
                                    {
                                        throw new CommonException("Комната с атрибутом LivingRoomGUID равным NULL должна быть единственной в списке.", PerformServiceError.HCS_DAT_00001.ToString());
                                    }
                                }

                                annulObjects.Add(HouseObjectType.LivingRoom);
                            }
                            // если помещение имеет признак "аннулировать", то не должно быть списка комнат
                            else
                            {
                                if (block.HouseImportRequestLivingRooms.Count > 0)
                                {
                                    throw new CommonException("У блока для аннулирования не должно быть списка комнат.", PerformServiceError.HCS_DAT_00001.ToString());
                                }

                                annulObjects.Add(HouseObjectType.Block);
                                annulObjects.Add(HouseObjectType.LivingRoom);
                            }
                        }
                        #endregion

                        #endregion
                    }
                    break;
            }

            return Task.FromResult(annulObjects.ToArray());
        }
        public Task<IEnumerable<HouseExportRequest>> CreateAnnulExportRequest(IEnumerable<HouseImportRequest> annulRequest, Guid transactionGuid, IEnumerable<ObjectInfo> objectInfo)
        {
            if (annulRequest == null || annulRequest.Count() == 0)
            {
                throw new CommonException("Пустой набор данных для аннулирования ОЖФ.", PerformServiceError.HCS_DAT_00001.ToString());
            }
            if (annulRequest.Count() > 1)
            {
                throw new CommonException("Набор данных для аннулирования ОЖФ должен содержать одну запись.", PerformServiceError.HCS_DAT_00001.ToString());
            }

            var annulRequestItem = annulRequest.First();
            var exportRequest = new HouseExportRequest
            {
                TransactionGUID = transactionGuid,
                TransportGUID = Guid.NewGuid(),
                FIASHouseGuid = annulRequestItem.FIASHouseGuid,
            };

            return Task.FromResult<IEnumerable<HouseExportRequest>>(new List<HouseExportRequest> { exportRequest });
        }
        public Task<IEnumerable<HouseImportRequest>> CreateAnnulImportRequest(IEnumerable<HouseExportResult> exportResult, IEnumerable<HouseImportRequest> annulRequest, Guid transactionGuid, HouseObjectType closeObject, IEnumerable<ObjectInfo> objectInfo)
        {
            if (annulRequest == null || annulRequest.Count() == 0)
            {
                throw new CommonException("Пустой набор данных для аннулирования ОЖФ.", PerformServiceError.HCS_DAT_00001.ToString());
            }
            HouseImportRequest houseAnnulData = annulRequest.First();
            if (!Enum.IsDefined(typeof(HouseType), houseAnnulData.houseType))
            {
                throw new CommonException("Неизвестный тип дома.", PerformServiceError.HCS_DAT_00001.ToString());
            }
            if (exportResult == null || exportResult.Count() == 0)
            {
                throw new Exception("Пустой набор данных экспорта ОЖФ.");
            }
            HouseExportResult houseExportData = exportResult.First();

            HouseImportRequest importHouse = new HouseImportRequest
            {
                TransportGUID = Guid.NewGuid(),
                HouseGUID = houseAnnulData.HouseGUID,
                houseType = houseAnnulData.houseType,
                FIASHouseGuid = houseAnnulData.FIASHouseGuid,

                InheritMissingValues = true,

                HouseImportRequestEntrances = new HashSet<HouseImportRequestEntrance>(),
                HouseImportRequestPremises = new HashSet<HouseImportRequestPremise>(),
                HouseImportRequestBlocks = new HashSet<HouseImportRequestBlock>(),
                HouseImportRequestLivingRooms = new HashSet<HouseImportRequestLivingRoom>(),
            };
            switch ((HouseType)houseAnnulData.houseType)
            {
                case HouseType.ApartmentHouse:
                    {
                        #region ApartmentHouse

                        #region nonResidentialPremises
                        if (closeObject == HouseObjectType.Premise)
                        {
                            if (houseAnnulData.HouseImportRequestPremises != null && houseExportData.HouseExportResultPremises != null)
                            {
                                var nonResidentialPremises = houseAnnulData.HouseImportRequestPremises
                                    .Where(it => it.premiseType == (int)PremisesType.NonResidential)
                                    .ToList();
                                var exportNonResidentialPremises = houseExportData.HouseExportResultPremises
                                    .Where(it => it.premiseType == (int)PremisesType.NonResidential)
                                    .ToList();
                                this.PremisesToAnnul(closeObject, nonResidentialPremises, exportNonResidentialPremises, importHouse);
                            }
                        }
                        #endregion

                        #region residentialPremises
                        if (closeObject == HouseObjectType.Premise || closeObject == HouseObjectType.LivingRoom)
                        {
                            if (houseAnnulData.HouseImportRequestPremises != null && houseExportData.HouseExportResultPremises != null)
                            {
                                var residentialPremises = houseAnnulData.HouseImportRequestPremises
                                   .Where(it => it.premiseType == (int)PremisesType.Residential && it.HouseImportEntranceTransportGUID == null)
                                   .ToList();
                                var exportResidentialPremises = houseExportData.HouseExportResultPremises
                                    .Where(it => it.premiseType == (int)PremisesType.Residential && it.HouseExportEntranceTransportGUID == null)
                                    .ToList();
                                this.PremisesToAnnul(closeObject, residentialPremises, exportResidentialPremises, importHouse);
                            }
                        }
                        #endregion

                        #region entrances
                        if (closeObject == HouseObjectType.Entrance || closeObject == HouseObjectType.Premise || closeObject == HouseObjectType.LivingRoom)
                        {
                            var entrances = houseAnnulData.HouseImportRequestEntrances;
                            var exportEntrances = houseExportData.HouseExportResultEntrances;
                            this.EntrancesToAnnul(closeObject, entrances, exportEntrances, importHouse);
                        }
                        #endregion

                        #endregion
                    }
                    break;
                case HouseType.LivingHouse:
                    {
                        #region LivingHouse

                        #region livingRooms
                        if (closeObject == HouseObjectType.LivingRoom)
                        {
                            var livingRooms = houseAnnulData.HouseImportRequestLivingRooms;
                            var exportLivingRooms = houseExportData.HouseExportResultLivingRooms;
                            this.LivingRoomsToAnnul(closeObject, livingRooms, exportLivingRooms, importHouse);
                        }
                        #endregion

                        #endregion
                    }
                    break;
                case HouseType.BlockHouse:
                    {
                        #region BlockHouse

                        #region blocks
                        if (closeObject == HouseObjectType.Block || closeObject == HouseObjectType.LivingRoom)
                        {
                            var blocks = houseAnnulData.HouseImportRequestBlocks;
                            var exportBlocks = houseExportData.HouseExportResultBlocks;
                            this.BlocksToAnnul(closeObject, blocks, exportBlocks, importHouse);
                        }
                        #endregion

                        #endregion
                    }
                    break;
            }

            return Task.FromResult<IEnumerable<HouseImportRequest>>(new HouseImportRequest[] { importHouse });
        }
        public Task<IEnumerable<HouseImportResult>> CreateAnnulResult(IDictionary<HouseObjectType, IEnumerable<HouseImportResult>> importResults, IEnumerable<HouseImportRequest> annulRequest, Guid transactionGuid, IEnumerable<ObjectInfo> objectInfo)
        {
            var annulRequestItem = annulRequest.First();
            HouseImportResult annulResult = annulRequestItem.CreateResult<HouseImportResult, HouseImportResultError>();
            foreach (var annulObject in importResults.Keys/*.OrderByDescending(ss => (int)ss)*/)
            {
                HouseImportResult importResult = importResults[annulObject].First();
                switch (annulObject)
                {
                    // todo: копировать item
                    case HouseObjectType.LivingRoom:
                        if (importResult.HouseImportResultLivingRooms != null)
                        {
                            annulResult.HouseImportResultLivingRooms = new HashSet<HouseImportResultLivingRoom>();
                            foreach (var item in importResult.HouseImportResultLivingRooms)
                            {
                                item.HouseImportTransportGUID = annulResult.TransportGUID;
                                annulResult.HouseImportResultLivingRooms.Add(item);
                            }
                        }
                        break;
                    case HouseObjectType.Block:
                        if (importResult.HouseImportResultBlocks != null)
                        {
                            annulResult.HouseImportResultBlocks = new HashSet<HouseImportResultBlock>();
                            foreach (var item in importResult.HouseImportResultBlocks)
                            {
                                item.HouseImportTransportGUID = annulResult.TransportGUID;
                                annulResult.HouseImportResultBlocks.Add(item);
                            }
                        }
                        break;
                    case HouseObjectType.Premise:
                        if (importResult.HouseImportResultPremises != null)
                        {
                            annulResult.HouseImportResultPremises = new HashSet<HouseImportResultPremise>();
                            foreach (var item in importResult.HouseImportResultPremises)
                            {
                                item.HouseImportTransportGUID = annulResult.TransportGUID;
                                annulResult.HouseImportResultPremises.Add(item);
                            }
                        }
                        break;
                    case HouseObjectType.Entrance:
                        if (importResult.HouseImportResultEntrances != null)
                        {
                            annulResult.HouseImportResultEntrances = new HashSet<HouseImportResultEntrance>();
                            foreach (var item in importResult.HouseImportResultEntrances)
                            {
                                item.HouseImportTransportGUID = annulResult.TransportGUID;
                                annulResult.HouseImportResultEntrances.Add(item);
                            }
                        }
                        break;
                }
            }

            return Task.FromResult<IEnumerable<HouseImportResult>>(new HouseImportResult[] { annulResult });
        }
        public string ExplainAnnulResult(IEnumerable<HouseImportResult> annulResult)
        {
            Func<int, string, string, string> ResultHandler = (count, countName, result) =>
            {
                if (count > 0)
                {
                    result += String.Format("\n\t{0} {1}", count, countName);
                }
                return result;
            };

            var annulResultItem = annulResult.First();
            string successResult = "", errrorResult = "";
            HouseAnnulResult houseAnnulResult = this.CheckResult(annulResult);
            if (houseAnnulResult.HasSuccessResult())
            {
                successResult = "Закрыто:";
                successResult = ResultHandler(houseAnnulResult.Entrances.Successed, "подъездов", successResult);
                successResult = ResultHandler(houseAnnulResult.Premises.Successed, "помещений", successResult);
                successResult = ResultHandler(houseAnnulResult.Blocks.Successed, "блоков", successResult);
                successResult = ResultHandler(houseAnnulResult.LivingRooms.Successed, "комнат", successResult);
            }
            if (houseAnnulResult.HasErrorResult())
            {
                errrorResult = "Не удалось закрыть:";
                errrorResult = ResultHandler(houseAnnulResult.Entrances.WithError, "подъездов", errrorResult);
                errrorResult = ResultHandler(houseAnnulResult.Premises.WithError, "помещений", errrorResult);
                errrorResult = ResultHandler(houseAnnulResult.Blocks.WithError, "блоков", errrorResult);
                errrorResult = ResultHandler(houseAnnulResult.LivingRooms.WithError, "комнат", errrorResult);
            }

            if (successResult != "" || errrorResult != "")
            {
                var result = successResult;
                if (result != "")
                {
                    result += "\n";
                }
                result += errrorResult;
                return result;
            }
            return "Нечего закрывать";
        }

        public async Task<TransactionResultInfo> ImportHouseAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<HouseImportRequest, HouseImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ExportHouseAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<HouseExportRequest, HouseExportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ImportNotificationAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<NotificationImportRequest, NotificationImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> DeleteNotificationAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<NotificationImportDeleteRequest, NotificationImportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ExportNsiAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<NsiExportRequest, NsiExportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ExportOrganizationAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<OrganizationExportRequest, OrganizationExportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        public async Task<TransactionResultInfo> ImportOrderAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<OrderImportRequest, OrderImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> CancelOrderAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<OrderImportCancellationRequest, OrderImportResult>(transactionGuid, log);
            return result;
        }
    }

    public partial class HcsService
    {
        //public async Task<TransferResult> ImportPaymentDocumentAsync(
        //    Guid transactionGuid,
        //    IEnumerable<ObjectInfo> objectInfo,
        //    PaymentImportDataSource _dataSource,
        //    IDataStoreNew2 _dataStore,
        //    Action<string, string> log)
        //{
        //    if (_dataSource is ILoggable)
        //    {
        //        ((ILoggable)_dataSource).Log = log;
        //    }
        //    if (_dataStore is ILoggable)
        //    {
        //        ((ILoggable)_dataStore).Log = log;
        //    }

        //    // создаем транзакицю, если еще не была создана
        //    if (!_dataStore.IsTransactionExists(transactionGuid))
        //    {
        //        _dataStore.CreateTransaction(transactionGuid, SysOperationCode.PaymentDocumentImport);
        //    }

        //    // получаем текущее состояние транзакции
        //    var transactionStateInfo = _dataStore.GetTransactionState(transactionGuid);

        //    // выполняем операции
        //    IEnumerable<PaymentExportRequest> requestExportData = null;
        //    IEnumerable<PaymentExportResult> resultExportData = null;
        //    IEnumerable<PaymentImportRequest> requestImportData = null;
        //    IEnumerable<PaymentImportResult> resultImportData = null;
        //    // получаем данные для импорта
        //    #region
        //    if (transactionStateInfo == null)
        //    {
        //        // получаем данные из источника и сохраняем их в БД
        //        requestImportData = await _dataSource.TakeDataAsync(transactionGuid, objectInfo);
        //        var stateGUID = Guid.NewGuid();
        //        var stateData = JsonHelper.Serialize(closeObjects);
        //        transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseAnnul, TransactionState.Composed, stateData, stateGUID);
        //        _dataStore.SaveData(requestAnnulData, stateGUID);
        //        _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //    }
        //    if (transactionStateInfo.Operation == SysOperationCode.HouseAnnul && transactionStateInfo.State == TransactionState.Composed)
        //    {
        //        // формируем запрос на экпорт и сохраняем его в БД
        //        requestExportData = await _dataSource.CreateAnnulExportRequest(requestAnnulData, transactionGuid, objectInfo);
        //        var stateGUID = Guid.NewGuid();
        //        transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseExport, TransactionState.Composed, null, stateGUID);
        //        _dataStore.SaveData(requestExportData, stateGUID);
        //        _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //    }
        //    #endregion
        //    // идет операция экспорта
        //    #region
        //    if (transactionStateInfo.Operation == SysOperationCode.HouseExport)
        //    {
        //        // запрос сформирован
        //        #region
        //        if (transactionStateInfo.State == TransactionState.Composed)
        //        {
        //            // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
        //            var stateGUID = transactionStateInfo.StateGUID;
        //            if (requestExportData == null)
        //            {
        //                requestExportData = _dataStore.ReadData<HouseExportRequest>(stateGUID);
        //            }

        //            var asyncStateGUID = await this.ExportHouseAsync(requestExportData.FirstOrDefault(), stateGUID, log);
        //            transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseExport, TransactionState.Sent, null, stateGUID, asyncStateGUID);
        //            _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //        }
        //        #endregion
        //        // запрос отправлен
        //        #region
        //        if (transactionStateInfo.State == TransactionState.Sent)
        //        {
        //            // получаем ответ от ГИС ЖКХ и сохраняем в БД
        //            var stateGUID = transactionStateInfo.StateGUID;
        //            var asyncStateGUID = (Guid)transactionStateInfo.AsyncStateGUID;

        //            try
        //            {
        //                resultExportData = await this.ExportHouseAsyncState(asyncStateGUID, log);
        //            }
        //            catch (CommonException e)
        //            {
        //                throw;
        //                //if (e.Code == ServiceError.INT002012.ToString())
        //                //{
        //                //    //
        //                //}
        //                //else
        //                //{
        //                //    throw;
        //                //}
        //            }
        //            transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseExport, TransactionState.Recieved, null, stateGUID);
        //            _dataStore.SaveData(resultExportData, stateGUID);
        //            _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //        }
        //        #endregion
        //    }
        //    #endregion
        //    // аннулирование ОЖФ
        //    #region
        //    if (transactionStateInfo.Operation == SysOperationCode.HouseExport && transactionStateInfo.State == TransactionState.Recieved || transactionStateInfo.Operation == SysOperationCode.HouseImport)
        //    {
        //        TransactionStateInfo2 annulTransactionStateInfo = _dataStore.GetTransactionState(transactionGuid, SysOperationCode.HouseAnnul, TransactionState.Composed);
        //        TransactionStateInfo2 exportTransactionStateInfo;
        //        if (transactionStateInfo.Operation == SysOperationCode.HouseExport)
        //        {
        //            exportTransactionStateInfo = transactionStateInfo;
        //        }
        //        else
        //        {
        //            exportTransactionStateInfo = _dataStore.GetTransactionState(transactionGuid, SysOperationCode.HouseExport, TransactionState.Recieved);
        //        }
        //        if (closeObjects == null)
        //        {
        //            closeObjects = JsonHelper.Deserialize<HouseObjectType[]>(annulTransactionStateInfo.Data);
        //        }
        //        IEnumerable<HouseObjectType> remainigCloseObjects = closeObjects.OrderByDescending(ss => (int)ss);
        //        if (transactionStateInfo.Operation == SysOperationCode.HouseImport)
        //        {
        //            var lastCloseObject = JsonHelper.Deserialize<HouseObjectType>(transactionStateInfo.Data);
        //            remainigCloseObjects = remainigCloseObjects.SkipWhile(ss => ss != lastCloseObject);
        //            if (transactionStateInfo.State == TransactionState.Recieved)
        //            {
        //                remainigCloseObjects = remainigCloseObjects.Skip(1);
        //            }
        //        }

        //        foreach (var closeObject in remainigCloseObjects)
        //        {
        //            //
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Recieved)
        //            {
        //                // формируем запрос на импорт и сохраняем его в БД
        //                if (requestAnnulData == null)
        //                {
        //                    requestAnnulData = _dataStore.ReadData<HouseImportRequest>(annulTransactionStateInfo.StateGUID);
        //                }
        //                if (resultExportData == null)
        //                {
        //                    resultExportData = _dataStore.ReadData<HouseExportResult>(exportTransactionStateInfo.StateGUID);
        //                }
        //                requestImportData = await _dataSource.CreateAnnulImportRequest(resultExportData, requestAnnulData, transactionGuid, closeObject, objectInfo);
        //                if (!_dataSource.ToAnnulment(requestImportData))
        //                {
        //                    continue;
        //                }
        //                var stateGUID = Guid.NewGuid();
        //                string stateData = JsonHelper.Serialize(closeObject);
        //                transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseImport, TransactionState.Composed, stateData, stateGUID);
        //                _dataStore.SaveData(requestImportData, stateGUID);
        //                _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //            }
        //            #endregion
        //            // запрос сформирован
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Composed)
        //            {
        //                // берем данные запроса из БД, отправляем в ГИС ЖКХ и сохраняем маркер запроса в БД
        //                var stateGUID = transactionStateInfo.StateGUID;
        //                if (requestImportData == null)
        //                {
        //                    requestImportData = _dataStore.ReadData<HouseImportRequest>(stateGUID);
        //                }
        //                string stateData = JsonHelper.Serialize(closeObject);
        //                var asyncState = await this.ImportHouseAsync(requestImportData, stateGUID, log);
        //                var asyncStateGUID = asyncState.State;
        //                transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseImport, TransactionState.Sent, stateData, stateGUID, asyncStateGUID);
        //                _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //            }
        //            #endregion
        //            // запрос отправлен
        //            #region
        //            if (transactionStateInfo.State == TransactionState.Sent)
        //            {
        //                // получаем ответ от ГИС ЖКХ и сохраняем в БД
        //                var stateGUID = transactionStateInfo.StateGUID;
        //                if (requestImportData == null)
        //                {
        //                    requestImportData = _dataStore.ReadData<HouseImportRequest>(stateGUID);
        //                }

        //                var asyncStateGUID = (Guid)transactionStateInfo.AsyncStateGUID;
        //                string stateData = JsonHelper.Serialize(closeObject);
        //                transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseImport, TransactionState.Recieved, stateData, stateGUID);
        //                var asyncState = new AsyncState<HouseImportRequest, HouseImportResult>
        //                {
        //                    State = asyncStateGUID,
        //                    RequestData = requestImportData,
        //                    ResultData = Enumerable.Empty<HouseImportResult>(),
        //                };
        //                resultImportData = await this.ImportHouseAsyncState(asyncState, log);
        //                _dataStore.SaveData(resultImportData, stateGUID);
        //                _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //                if (_dataSource.CheckResult(resultImportData).HasErrorResult())
        //                {
        //                    break;
        //                }
        //            }
        //            #endregion
        //        }
        //        {
        //            // берем данные из БД и передаем внешнему модулю
        //            if (requestAnnulData == null)
        //            {
        //                requestAnnulData = _dataStore.ReadData<HouseImportRequest>(annulTransactionStateInfo.StateGUID);
        //            }
        //            var resultImportDataAll = new Dictionary<HouseObjectType, IEnumerable<HouseImportResult>>();
        //            foreach (var importTransactionStateInfo in _dataStore.GetTransactionStates(transactionGuid, SysOperationCode.HouseImport, TransactionState.Recieved))
        //            {
        //                resultImportData = _dataStore.ReadData<HouseImportResult>(importTransactionStateInfo.StateGUID);
        //                var closeObject = JsonHelper.Deserialize<HouseObjectType>(importTransactionStateInfo.Data);
        //                resultImportDataAll.Add(closeObject, resultImportData);
        //            }
        //            resultAnnulData = await _dataSource.CreateAnnulResult(resultImportDataAll, requestAnnulData, transactionGuid, objectInfo);
        //            await _dataSource.PassDataAsync(resultAnnulData, transactionGuid, objectInfo);
        //            string resultString = _dataSource.ExplainAnnulResult(resultAnnulData);
        //            var stateGUID = annulTransactionStateInfo.StateGUID;
        //            string stateData = JsonHelper.Serialize(TransferResult.Create(objectInfo, resultString));
        //            transactionStateInfo = TransactionStateInfo2.Create(SysOperationCode.HouseAnnul, TransactionState.Transferred, stateData, stateGUID);
        //            _dataStore.SetTransactionState(transactionGuid, transactionStateInfo);
        //        }
        //    }
        //    #endregion

        //    // результат
        //    var transferResult = JsonHelper.Deserialize<TransferResult>(transactionStateInfo.Data);
        //    return transferResult;
        //}

        public async Task<TransactionResultInfo> ImportPaymentDocumentAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<PaymentImportRequest, PaymentImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> WithdrawPaymentDocumentAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ImportAsync<PaymentImportWithdrawRequest, PaymentImportResult>(transactionGuid, log);
            return result;
        }
        public async Task<TransactionResultInfo> ExportPaymentDocumentAsync(
            Guid transactionGuid,
            Action<string, string> log)
        {
            var result = await this.ExportAsync<PaymentExportRequest, PaymentExportResult>(transactionGuid, log);
            return result;
        }
    }

}
