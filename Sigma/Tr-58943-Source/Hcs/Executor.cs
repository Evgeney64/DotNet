using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Transactions;

namespace Hcs
{
    using Model;

    public class HCSExecutor
    {
        public static Task<IList<IGrouping<int, ObjectInfo>>> Partition(IEnumerable<ObjectInfo> objectInfo, int maxListCount)
        {
            if (maxListCount < 1)
            {
                maxListCount = 1;
            }

            var groupedObjectInfo = objectInfo
                .GroupBy(o => new { o.Operation, o.Group })
                .SelectMany(l =>
                {
                    int groupCount = l.Count();
                    int chunkSize = groupCount / maxListCount + (groupCount % maxListCount > 0 ? 1 : 0);
                    return l
                        .Select((x, i) => new { Index = i, Value = x })
                        .GroupBy(x => x.Index % chunkSize, x => x.Value, (k, g) => new Grouping<int, ObjectInfo>(l.Key.Operation, g));
                })
                .ToList<IGrouping<int, ObjectInfo>>();

            return Task.FromResult<IList<IGrouping<int, ObjectInfo>>>(groupedObjectInfo);
        }
    }

    public class HCSExecutor<TRequestData, TResultData>
        where TRequestData : class, ITransactionEntity
        where TResultData : class, ITransactionEntity
    {
        private readonly IDataSource dataSource;
        private readonly IDataStore dataStore;
        private readonly IDataService2 dataService;
        private readonly Action<string, string> log;
        private SysOperationCode operationCode;
        private Guid transactionGuid;
        public IEnumerable<ObjectInfo> ObjectInfo { get; private set; }
        public IEnumerable<TRequestData> RequestData { get; private set; }
        public IEnumerable<TResultData> ResultData { get; private set; }
        public TransactionStateInfo CurrentTransactionStateInfo { get; private set; }

        private TransactionScope BeginTransaction()
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TransactionManager.MaximumTimeout
            };
            return new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        }
        private TransactionScope SupressTransaction()
        {
            return new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled);
        }
        //private async Task<IEnumerable<ObjectInfo>> getObjectInfo(Guid stateGuid)
        //{
        //    if (this.ObjectInfo == null)
        //    {
        //        this.ObjectInfo = await dataStore.GetTransactionObjectsAsync(stateGuid);
        //    }
        //    return this.ObjectInfo;
        //}
        private async Task<IEnumerable<TRequestData>> getRequestData(Guid stateGuid)
        {
            if (this.RequestData == null)
            {
                this.RequestData = await dataStore.ReadDataAsync<TRequestData>(stateGuid);
            }
            return this.RequestData;
        }
        private async Task<IEnumerable<TResultData>> getResultData(Guid stateGuid)
        {
            if (this.ResultData == null)
            {
                this.ResultData = await dataStore.ReadDataAsync<TResultData>(stateGuid);
            }
            return this.ResultData;
        }
        private IEnumerable<ObjectInfoError> createObjectInfoError(IEnumerable<ObjectInfo> objectInfo, IEnumerable<TResultData> resultData)
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

        public HCSExecutor(IDataSource dataSource, IDataStore dataStore, IDataService2 dataService, Action<string, string> log)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }
            if (dataStore == null)
            {
                throw new ArgumentNullException(nameof(dataStore));
            }
            if (dataService == null)
            {
                throw new ArgumentNullException(nameof(dataService));
            }

            this.dataSource = dataSource;
            this.dataStore = dataStore;
            this.dataService = dataService;
            this.log = log;

            if (this.dataSource is ILoggable)
            {
                ((ILoggable)this.dataSource).Log = this.log;
            }
            if (this.dataStore is ILoggable)
            {
                ((ILoggable)this.dataStore).Log = this.log;
            }
        }

        public void Init(TransactionInfo transactionInfo, Guid transactionGuid, IEnumerable<TRequestData> requestData = null, TransactionStateInfo transactionState = null)
        {
            this.operationCode = transactionInfo.OperationCode;
            this.transactionGuid = transactionGuid;
            this.RequestData = requestData;
            this.ResultData = null;
            this.ObjectInfo = transactionInfo.Objects;
            this.CurrentTransactionStateInfo = transactionState;
        }
        public async Task<TransactionState> TakeDataAsync(Guid stateGuid, string stateData)
        {
            this.RequestData = await dataSource.TakeDataAsync<TRequestData>(stateGuid, this.ObjectInfo);
            using (var transactionScope = this.BeginTransaction())
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(this.operationCode, TransactionState.Composed, stateData, stateGuid);
                await dataStore.SaveDataAsync(this.RequestData, stateGuid);
                //await dataStore.SetTransactionObjectsAsync(this.transactionGuid, this.ObjectInfo);
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            this.ObjectInfo = this.ObjectInfo;
            return TransactionState.Composed;
        }
        public async Task<TransactionState> PassDataAsync(Guid stateGuid, string stateData)
        {
            await getResultData(stateGuid);
            //await getObjectInfo(stateGuid);
            IEnumerable<ObjectInfoError> objectInfoError = this.createObjectInfoError(this.ObjectInfo, this.ResultData);
            await dataSource.PassDataAsync(this.ResultData, stateGuid, this.ObjectInfo);
            using (var transactionScope = this.BeginTransaction())
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(this.operationCode, TransactionState.Transferred, stateData, stateGuid);
                await dataStore.SetTransactionObjectErrorsAsync(this.transactionGuid, objectInfoError);
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            return TransactionState.Transferred;
        }
        public async Task<TransactionState> SendImportDataAsync(Guid stateGuid, string stateData)
        {
            await getRequestData(stateGuid);
            if (this.RequestData.Count() == 0)
            {
                return TransactionState.Recieved;
            }
            var asyncState = await this.dataService.ImportAsync<TRequestData, TResultData>(this.RequestData, stateGuid, this.log);
            if (asyncState.RequestState != null)
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(this.operationCode, TransactionState.Sent, stateData, stateGuid, asyncState.RequestState.State);
            }
            else
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(this.operationCode, TransactionState.Recieved, stateData, stateGuid);
            }
            this.ResultData = asyncState.ResultData;
            using (var transactionScope = this.BeginTransaction())
            {
                await dataStore.SaveDataAsync(this.ResultData, stateGuid);
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            return this.CurrentTransactionStateInfo.State;
        }
        public async Task<TransactionState> RecieveImportDataAsync(Guid stateGuid, string stateData)
        {
            await getRequestData(stateGuid);
            await getResultData(stateGuid);
            var requestStateData = this.RequestData
                .Where(ss => !this.ResultData
                    .Select(ss1 => ss1.TransportGUID)
                    .Contains(ss.TransportGUID))
                .ToList();
            var requestAsyncState = new AsyncImportState<TRequestData>
            {
                Data = requestStateData,
                State = (Guid)this.CurrentTransactionStateInfo.AsyncStateGUID,
            };

            var resultStateData = await this.dataService.ImportStateAsync<TRequestData, TResultData>(requestAsyncState, log);
            using (var transactionScope = this.BeginTransaction())
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(this.operationCode, TransactionState.Recieved, stateData, stateGuid);
                await dataStore.SaveDataAsync(resultStateData, stateGuid);
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            this.ResultData = this.ResultData
                .Concat(resultStateData)
                .ToList();
            return TransactionState.Recieved;
        }
        public async Task<TransactionState> SendExportDataAsync(Guid stateGuid, string stateData)
        {
            await getRequestData(stateGuid);
            if (this.RequestData.Count() == 0)
            {
                return TransactionState.Recieved;
            }
            if (this.RequestData.Count() > 1)
            {
                throw new Exception("Invalid export data count");
            }
            var asyncState = await this.dataService.ExportAsync<TRequestData, TResultData>(this.RequestData.First(), stateGuid, this.log);
            this.CurrentTransactionStateInfo = TransactionStateInfo.Create(operationCode, TransactionState.Sent, stateData, stateGuid, asyncState);
            using (var transactionScope = this.BeginTransaction())
            {
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            return this.CurrentTransactionStateInfo.State;
        }
        public async Task<TransactionState> RecieveExportDataAsync(Guid stateGuid, string stateData)
        {
            await getRequestData(stateGuid);
            if (this.RequestData.Count() == 0)
            {
                return TransactionState.Recieved;
            }
            if (this.RequestData.Count() > 1)
            {
                throw new Exception("Invalid export data count");
            }
            var requestAsyncState = new AsyncExportState<TRequestData>
            {
                Data = this.RequestData.First(),
                State = (Guid)this.CurrentTransactionStateInfo.AsyncStateGUID,
            };
            this.ResultData = await this.dataService.ExportStateAsync<TRequestData, TResultData>(requestAsyncState, log);
            using (var transactionScope = this.BeginTransaction())
            {
                this.CurrentTransactionStateInfo = TransactionStateInfo.Create(operationCode, TransactionState.Recieved, stateData, stateGuid);
                await dataStore.SaveDataAsync(this.ResultData, stateGuid);
                await dataStore.SetTransactionStateAsync(this.transactionGuid, this.CurrentTransactionStateInfo);

                transactionScope.Complete();
            }
            return TransactionState.Recieved;
        }

        // note: убрать
        public async Task<TransactionResultInfo> ImportAsync(
            Guid transactionGuid)
        {
            TransactionInfo transactionInfo = await this.dataStore.GetTransactionAsync(transactionGuid);
            if (transactionInfo == null)
            {
                return TransactionResultInfo.None();
            }
            TransactionResultInfo transactionResultCheck = await this.dataStore.GetTransactionResultAsync(transactionGuid);
            if (transactionResultCheck.IsCompleted)
            {
                return transactionResultCheck;
            }

            this.Init(transactionInfo, transactionGuid);
            // получаем текущее состояние транзакции
            this.CurrentTransactionStateInfo = await dataStore.GetTransactionStateAsync(transactionGuid);
            TransactionState transactionState = this.CurrentTransactionStateInfo?.State ?? TransactionState.None;
            TransactionLogInfo transactionLog = TransactionLogInfo.Create();
            TransactionResultInfo transactionResult;
            try
            {
                while (transactionState != TransactionState.Transferred)
                {
                    switch (transactionState)
                    {
                        case TransactionState.None:
                            transactionState = await this.TakeDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Composed:
                            transactionState = await this.SendImportDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Sent:
                            transactionState = await this.RecieveImportDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Recieved:
                            transactionState = await this.PassDataAsync(transactionGuid, null);
                            continue;
                    }
                }
                //await getObjectInfo(transactionGuid);
                transactionResult = TransactionResultInfo.Create(this.ObjectInfo);
                
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
                await dataStore.SetTransactionLogAsync(transactionGuid, transactionLog);
            }

            if (transactionResult.IsCompleted)
            {
                await dataStore.SetTransactionResultAsync(transactionGuid, transactionResult);
            }
            return transactionResult;
        }
        // note: убрать
        public async Task<TransactionResultInfo> ExportAsync(
            Guid transactionGuid)
        {
            TransactionInfo transactionInfo = await this.dataStore.GetTransactionAsync(transactionGuid);
            if (transactionInfo == null)
            {
                return TransactionResultInfo.None();
            }
            TransactionResultInfo transactionResultCheck = await this.dataStore.GetTransactionResultAsync(transactionGuid);
            if (transactionResultCheck.IsCompleted)
            {
                return transactionResultCheck;
            }

            this.Init(transactionInfo, transactionGuid);
            // получаем текущее состояние транзакции
            this.CurrentTransactionStateInfo = await dataStore.GetTransactionStateAsync(transactionGuid);
            TransactionState transactionState = this.CurrentTransactionStateInfo?.State ?? TransactionState.None;
            TransactionLogInfo transactionLog = TransactionLogInfo.Create();
            TransactionResultInfo transactionResult;
            try
            {
                while (transactionState != TransactionState.Transferred)
                {
                    switch (transactionState)
                    {
                        case TransactionState.None:
                            transactionState = await this.TakeDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Composed:
                            transactionState = await this.SendExportDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Sent:
                            transactionState = await this.RecieveExportDataAsync(transactionGuid, null);
                            continue;
                        case TransactionState.Recieved:
                            transactionState = await this.PassDataAsync(transactionGuid, null);
                            continue;
                    }
                }
                //await getObjectInfo(transactionGuid);
                transactionResult = TransactionResultInfo.Create(this.ObjectInfo);
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
                await dataStore.SetTransactionLogAsync(transactionGuid, transactionLog);
            }

            if (transactionResult.IsCompleted)
            {
                await dataStore.SetTransactionResultAsync(transactionGuid, transactionResult);
            }
            return transactionResult;
        }
    }
}
