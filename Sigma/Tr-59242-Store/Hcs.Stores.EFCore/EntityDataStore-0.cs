#region
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
//using System.Data.Metadata.Edm;
//using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

using Hcs;
using Hcs.Configuration;
using Hcs.Model;
using Microsoft.Extensions.Configuration;
#endregion

namespace Hcs.Stores
{
    public partial class EntityDataStore : IDataStore, IDisposable, ILoggable
    {
        private readonly EntityDataStoreConfiguration configuration;
        protected readonly EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();
        bool is_postgres = false;

        #region Constructors
        public EntityDataStore(IConfiguration _configuration)
        {
            if (_configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = _configuration.Get<EntityDataStoreConfiguration>();
            this.OnDataStoreCreating(entityRelationBuilder);
        }
        public EntityDataStore(EntityDataStoreConfiguration _configuration, bool _is_postgres = false)
        {
            if (_configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.configuration = _configuration;
            this.is_postgres = _is_postgres;
            //this.OnDataStoreCreating(entityRelationBuilder);
            this.OnDataStoreCreating();
        }
        #endregion

        #region get / set async methods
        protected virtual IEnumerable<T> GetDataSet<T>(HcsContext context, Guid stateGuid)
            where T : class //, ITransactionEntity
        {
            IEnumerable<Type> entityRelationTypes = this.entityRelationBuilder.GetEntityRelation<T>()
                .GetRelations()
                .Select(r => r.Type)
                .Distinct()
                .ToList();
            List<T> dataSet = new List<T>();
            foreach (Type entityRelationType in entityRelationTypes)
            {
                string select = String.Format("SELECT * FROM [{0}] WHERE TransactionGUID = '{1}'", entityRelationType.Name, stateGuid);
                if (entityRelationType == typeof(T))
                {
                    dataSet = context.Set<T>()
                        .FromSqlRaw(select)
                        .ToList();
                }
                else
                {
                     context.Set(entityRelationType)
                        .FromSqlRaw1(select)
                        .OfType<object>()
                        .ToList();
                }
            }
            return dataSet;
        }
        protected virtual void SetDataSet<T>(HcsContext context, IEnumerable<T> data, Guid stateGuid)
            where T : class
        {
            context.AddRange(data);
            context.SaveChanges(stateGuid);
        }
        protected async virtual Task SetDataSetAsync<T>(HcsContext context, IEnumerable<T> data, Guid stateGuid)
            where T : class
        {
            context.AddRange(data);
            await context.SaveChangesAsync(stateGuid);
        }
        public async Task<Guid> CreateTransactionAsync(TransactionInfo transaction)
        {
            Guid transactionGuid = Guid.NewGuid();
            return await this.CreateTransactionAsync(transactionGuid, transaction);
        }
        public async Task<Guid> CreateTransactionAsync(Guid transactionGuid, TransactionInfo transaction)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("CreateTransaction Begin");

                SysTransaction existingRequest = await context.SysTransaction
                    .FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                if (existingRequest != null)
                {
                    throw new Exception("Транзакция уже существует.");
                }

                SysTransaction request = new SysTransaction
                {
                    TransactionGUID = transactionGuid,
                    ListGUID = transaction.ListGuid,
                    ClientId = transaction.ClientId,
                    OperationId = (int)transaction.OperationCode,
                    StartDate = DateTime.Now,
                    StatusId = 0,
                    NextExecutionDate = DateTime.Now,
                };
                context.SysTransaction.Add(request);
                await context.SaveChangesAsync();

                this.AddTrace("CreateTransaction End");

                return transactionGuid;
            }
        }
        public async Task<TransactionStateInfo> GetTransactionStateAsync(Guid transactionGuid)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("GetTransactionState Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var requestState = context.SysTransactionState2
                    .Where(r => r.TransactionGUID == transactionGuid)
                    .OrderByDescending(r => r.StateDate)
                    .FirstOrDefault();

                this.AddTrace("GetTransactionState End");

                if (requestState != null)
                {
                    TransactionStateInfo result = TransactionStateInfo.Create(
                        (SysOperationCode)requestState.OperationId,
                        (TransactionState)requestState.StateTypeId,
                        requestState.StateData,
                        requestState.StateGUID,
                        requestState.AsyncStateGUID);

                    return result;
                    //return Task.FromResult(result);
                }
                return null;
                //return Task.FromResult<TransactionStateInfo>(null);
            }
        }
        public async Task<TransactionStateInfo> GetTransactionStateAsync(Guid transactionGuid, SysOperationCode operation, TransactionState State)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("GetTransactionState Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var requestState = context.SysTransactionState2
                    .Where(r => r.TransactionGUID == transactionGuid && r.OperationId == (int)operation && r.StateTypeId == (byte)State)
                    .OrderByDescending(r => r.StateDate)
                    .FirstOrDefault();

                this.AddTrace("GetTransactionState End");

                if (requestState != null)
                {
                    TransactionStateInfo result = TransactionStateInfo.Create(
                        (SysOperationCode)requestState.OperationId,
                        (TransactionState)requestState.StateTypeId,
                        requestState.StateData,
                        requestState.StateGUID,
                        requestState.AsyncStateGUID);

                    return result;
                    //return Task.FromResult(result);
                }
                return null;
                //return Task.FromResult<TransactionStateInfo>(null);
            }
        }

        public async Task<bool> IsTransactionExistsAsync(Guid transactionGuid)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("IsTransactionExists Begin");

                var existingRequest = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var existingRequest = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);

                this.AddTrace("IsTransactionExists End");

                return (existingRequest != null);
                //return Task.FromResult(existingRequest != null);
            }
        }

        public async Task SetTransactionResultAsync(Guid transactionGuid, TransactionResultInfo result)
        {
            if (result.Status != TransactionStatus.Completed || result.Result == TransactionResult.None)
            {
                throw new Exception("Недопустимое значение результата транзакции.");
            }
            if (result.Result == TransactionResult.TransactionError && result.Error == null)
            {
                throw new Exception("Отсутствует значение ошибки результата транзакции.");
            }

            using (var context = this.CreateContext())
            {
                this.AddTrace("SetTransactionResult Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                request.ResultId = (int)result.Result;
                if (result.Result == TransactionResult.TransactionError)
                {
                    request.ErrorCode = result.Error.ErrorCode;
                    request.ErrorDescription = result.Error.ErrorDescription;
                }
                context.SaveChanges();

                this.AddTrace("SetTransactionResult End");

                return;
                //return Task.FromResult(0);
            }
        }
        public async Task<TransactionResultInfo> GetTransactionResultAsync(Guid transactionGuid)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("GetTransactionResult Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                TransactionResultInfo transactionResult;
                switch ((TransactionResult)(request.ResultId ?? 0))
                {

                    case TransactionResult.TransactionError:
                        transactionResult = TransactionResultInfo.TransactionError(ErrorInfo.Create(request.ErrorCode, request.ErrorDescription), true);
                        break;
                    case TransactionResult.Success:
                    case TransactionResult.ObjectError:
                        var transactionObjects = await this.getTransactionObjectsAsync(context, transactionGuid);
                        transactionResult = TransactionResultInfo.Create(transactionObjects);
                        break;
                    case TransactionResult.None:
                    default:
                        transactionResult = TransactionResultInfo.None((TransactionStatus)request.StatusId == TransactionStatus.InProgress);
                        break;
                }

                this.AddTrace("GetTransactionResult End");

                return transactionResult;
            }
        }

        public async Task SetTransactionStateAsync(Guid transactionGuid, TransactionStateInfo transactionState)
        {
            if (!Enum.IsDefined(typeof(TransactionState), transactionState.State))
            {
                throw new Exception("Неизвестный статус.");
            }

            using (var context = this.CreateContext())
            {
                this.AddTrace("SetTransactionState Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var requestState = new SysTransactionState2
                {
                    StateGUID = transactionState.StateGUID,
                    AsyncStateGUID = transactionState.AsyncStateGUID,
                    TransactionGUID = transactionGuid,
                    OperationId = (int)transactionState.OperationCode,
                    StateTypeId = (byte)transactionState.State,
                    StateDate = DateTime.Now,
                    StateData = transactionState.Data,
                };
                context.SysTransactionState2.Add(requestState);
                await context.SaveChangesAsync();

                this.AddTrace("SetTransactionState End");

                return;
                //return Task.FromResult(0);
            }
        }
        // todo: индексы проверить!
        public async Task<IEnumerable<TransactionStateInfo>> GetTransactionStatesAsync(Guid transactionGuid)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("GetTransactionState Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var requestStates = context.SysTransactionState2.Where(r => r.TransactionGUID == transactionGuid).ToList();

                this.AddTrace("GetTransactionState End");

                var result = requestStates.Select(requestState =>
                    TransactionStateInfo.Create(
                        (SysOperationCode)requestState.OperationId,
                        (TransactionState)requestState.StateTypeId,
                        requestState.StateData,
                        requestState.StateGUID,
                        requestState.AsyncStateGUID));

                return result;
                //return Task.FromResult(result);
            }
        }
        public async Task<IEnumerable<TransactionStateInfo>> GetTransactionStatesAsync(Guid transactionGuid, SysOperationCode operation, TransactionState State)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("GetTransactionState Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var requestStates = context.SysTransactionState2.Where(r => r.TransactionGUID == transactionGuid && r.OperationId == (int)operation && r.StateTypeId == (byte)State).ToList();

                this.AddTrace("GetTransactionState End");

                var result = requestStates.Select(requestState =>
                    TransactionStateInfo.Create(
                        (SysOperationCode)requestState.OperationId,
                        (TransactionState)requestState.StateTypeId,
                        requestState.StateData,
                        requestState.StateGUID,
                        requestState.AsyncStateGUID));
                
                return result;
                //return Task.FromResult(result);
            }
        }

        public async Task SetTransactionLogAsync(Guid transactionGuid, TransactionLogInfo logInfo)
        {
            using (var context = this.CreateContext())
            {
                this.AddTrace("SetTransactionLog Begin");

                var request = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                //var request = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (request == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                var transactionLog = new SysTransactionLog
                {
                    TransactionGUID = request.TransactionGUID,
                    LogDate = DateTime.Now,
                    LogData = logInfo.Data,
                };
                if (logInfo.Error != null)
                {
                    transactionLog.ErrorCode = logInfo.Error.ErrorCode;
                    transactionLog.ErrorDescription = logInfo.Error.ErrorDescription;
                    transactionLog.ErrorStackTrace = logInfo.Error.ErrorStackTrace;
                }
                context.SysTransactionLog.Add(transactionLog);
                context.SaveChanges();

                this.AddTrace("SetTransactionLog End");

                return;
                //return Task.FromResult(0);
            }
        }

        private async Task<IEnumerable<ObjectInfo>> getTransactionObjectsAsync(HcsContext context, Guid transactionGuid)
        {
            var transactionObjects = await context.SysTransactionObject
                .Where(ss => ss.TransactionGUID == transactionGuid)
                .Select(ss => new ObjectInfo
                {
                    ObjectId = ss.objectId,
                    Comment = ss.comment,
                    Group = ss.group ?? 0,
                    Operation = ss.operation,
                    Param = ss.param,
                })
                .ToListAsync();
            var transactionObjectErrors = await context.SysTransactionObjectError
                .Where(ss => ss.TransactionGUID == transactionGuid)
                .Select(ss => new ObjectInfoError
                {
                    ObjectId = ss.objectId,
                    ErrorCode = ss.errorCode,
                    ErrorDescription = ss.errorDescription,
                })
                .ToListAsync();
            foreach (var transactionObjectErrorsGroup in transactionObjectErrors.GroupBy(ss => ss.ObjectId))
            {
                var transactionObject = transactionObjects
                    .Where(ss => ss.ObjectId == transactionObjectErrorsGroup.Key)
                    .FirstOrDefault();
                if (transactionObject != null)
                {
                    transactionObject.Errors = transactionObjectErrorsGroup.ToArray();
                }
            }

            return transactionObjects;
            //return Task.FromResult<IEnumerable<ObjectInfo>>(transactionObjects);
        }
        //public async Task SetTransactionObjectsAsync(Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
        //{
        //    if (objectList == null)
        //    {
        //        throw new ArgumentNullException(nameof(objectList));
        //    }

        //    try
        //    {
        //        using (var context = this.CreateContext())
        //        {
        //            this.AddTrace("SetTransactionObjects Begin");

        //            var transactionObjects = objectList
        //                .Select(ss => new SysTransactionObject
        //                {
        //                    TransactionGUID = transactionGuid,
        //                    objectId = ss.ObjectId,
        //                    comment = ss.Comment,
        //                    group = ss.Group,
        //                    operation = ss.Operation,
        //                    param = ss.Param,
        //                })
        //                .ToList();
        //            var transactionObjectErrors = objectList
        //                .Where(ss => ss.Errors != null)
        //                .SelectMany(ss => ss.Errors)
        //                .Select(ss => new SysTransactionObjectError
        //                {
        //                    TransactionGUID = transactionGuid,
        //                    objectId = ss.ObjectId,
        //                    errorCode = ss.ErrorCode,
        //                    errorDescription = ss.ErrorDescription,
        //                })
        //                .ToList();
        //            context.AddRange(transactionObjects);
        //            context.AddRange(transactionObjectErrors);
        //            await context.SaveChangesAsync();
        //            //context.SaveChanges();

        //            this.AddTrace("SetTransactionObjects End");

        //            return;
        //            //return Task.FromResult(0);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DataStoreException(e);
        //    }
        //}
        public async Task SetTransactionObjectErrorsAsync(Guid transactionGuid, IEnumerable<ObjectInfoError> objectErrorList)
        {
            if (objectErrorList == null)
            {
                throw new ArgumentNullException(nameof(objectErrorList));
            }

            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("SetTransactionObjectErrors Begin");

                    var transactionObjectErrors = objectErrorList
                        .Select(ss => new SysTransactionObjectError
                        {
                            TransactionGUID = transactionGuid,
                            objectId = ss.ObjectId,
                            errorCode = ss.ErrorCode,
                            errorDescription = ss.ErrorDescription,
                        })
                        .ToList();
                    context.AddRange(transactionObjectErrors);
                    await context.SaveChangesAsync();
                    //context.SaveChanges();

                    this.AddTrace("SetTransactionObjectErrors End");

                    return;
                    //return Task.FromResult(0);
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }
        //public async Task<IEnumerable<ObjectInfo>> GetTransactionObjectsAsync(Guid transactionGuid)
        //{
        //    try
        //    {
        //        using (var context = this.CreateContext())
        //        {
        //            this.AddTrace("GetTransactionObjects Begin");

        //            var transactionObjects = await this.getTransactionObjectsAsync(context, transactionGuid);

        //            this.AddTrace("GetTransactionObjects End");

        //            return transactionObjects;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new DataStoreException(e);
        //    }
        //}

        public virtual Task<IEnumerable<T>> ReadDataAsync<T>(Guid stateGuid)
            where T : class
        {
            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("Read Request Begin");
                    var data = this.GetDataSet<T>(context, stateGuid);
                    this.AddTrace("Read Request End");
                    return Task.FromResult(data);
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }
        public async virtual Task SaveDataAsync<T>(IEnumerable<T> data, Guid stateGuid)
            where T : class
        {
            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("Save Request Begin");
                    await this.SetDataSetAsync<T>(context, data, stateGuid);
                    //this.SetDataSet<T>(context, data, stateGuid);
                    this.AddTrace("Save Request End");

                    return;
                    //return Task.FromResult(0);
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }

        public async Task<IEnumerable<Guid>> CreateTransactionsAsync(TransactionInfo transactionInfo)
        {
            if (transactionInfo.Objects == null || !transactionInfo.Objects.Any())
            {
                throw new ArgumentException($"Не задан список объектов");
            }
            
            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("CreateTransactions Begin");

                    IList<Guid> transactionGuids = new List<Guid>();
                    SysOperation sysOperation = await context.SysOperation
                        .Where(ss => ss.OperationId == (int)transactionInfo.OperationCode)
                        .FirstOrDefaultAsync();
                    if (sysOperation == null)
                    {
                        throw new ArgumentException($"Неизвестный код операции {(int)transactionInfo.OperationCode}");
                    }

                    int packetSize = sysOperation.PacketSize;
                    IEnumerable<IEnumerable<ObjectInfo>> transactionObjectGroups;
                    if (packetSize > 0)
                    {
                        transactionObjectGroups = TransactionPartitioner.Partition(transactionInfo.Objects, packetSize);
                    }
                    else
                    {
                        transactionObjectGroups = new List<IEnumerable<ObjectInfo>>()
                        {
                            transactionInfo.Objects,
                        };
                    }
                    IEnumerable<ParamInfo> transactionParams = transactionInfo.Params;

                    foreach (var transactionObjects in transactionObjectGroups)
                    {
                        Guid transactionGuid = Guid.NewGuid();
                        SysTransaction sysTransaction = new SysTransaction
                        {
                            TransactionGUID = transactionGuid,
                            ListGUID = transactionInfo.ListGuid,
                            ClientId = transactionInfo.ClientId,
                            OperationId = (int)transactionInfo.OperationCode,
                            StartDate = DateTime.Now,
                            StatusId = (int)transactionInfo.InitialStatus,
                            NextExecutionDate = DateTime.Now,
                        };
                        await context.SysTransaction.AddAsync(sysTransaction);

                        IEnumerable<SysTransactionObject> sysTransactionObjects = transactionObjects
                            .Select(ss => new SysTransactionObject
                            {
                                TransactionGUID = transactionGuid,
                                objectId = ss.ObjectId,
                                comment = ss.Comment,
                                group = ss.Group,
                                operation = ss.Operation,
                                param = ss.Param,
                            })
                            .ToList();
                        await context.SysTransactionObject.AddRangeAsync(sysTransactionObjects);

                        if (transactionParams != null && transactionParams.Any())
                        {
                            IEnumerable<SysTransactionParam> sysTransactionParams = transactionParams
                                .Select(ss => new SysTransactionParam
                                {
                                    TransactionGUID = transactionGuid,
                                    ParamName = ss.Name,
                                    ParamValue = ss.Value,
                                })
                                .ToList();
                            await context.SysTransactionParam.AddRangeAsync(sysTransactionParams);
                        }
                        transactionGuids.Add(transactionGuid);
                    }
                    await context.SaveChangesAsync();

                    this.AddTrace("CreateTransactions End");

                    return transactionGuids;
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }

        private async Task<IEnumerable<ParamInfo>> getTransactionParamsAsync(HcsContext context, Guid transactionGuid)
        {
            var transactionParams = await context.SysTransactionParam
                .Where(ss => ss.TransactionGUID == transactionGuid)
                .Select(ss => new ParamInfo
                {
                    Name = ss.ParamName,
                    Value = ss.ParamValue,
                })
                .ToListAsync();

            return transactionParams;
        }
        public async Task<TransactionInfo> GetTransactionAsync(Guid transactionGuid)
        {
            using (var context = this.CreateContext())
            {
                SysTransaction transaction = await context.SysTransaction
                    .Where(ss => ss.TransactionGUID == transactionGuid)
                    .FirstOrDefaultAsync();
                if (transaction == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }


                IEnumerable<ObjectInfo> transactionObjects = await this.getTransactionObjectsAsync(context, transactionGuid);
                IEnumerable<ParamInfo> transactionParams = await this.getTransactionParamsAsync(context, transactionGuid);
                TransactionInfo transactionInfo = new TransactionInfo
                {
                    ListGuid = transaction.ListGUID,
                    ClientId = transaction.ClientId,
                    OperationCode = (SysOperationCode)transaction.OperationId,
                    Objects = transactionObjects,
                    Params = transactionParams,
                };

                return transactionInfo;
            }
        }

        private static object transactionLock = new object();
        // todo: TransactionInfo не подходит в качестве параметра
        // todo: lock
        public async Task<TransactionInfo2> AcquireTransactionAsync(TransactionInfo transactionInfo)
        {
            using (var context = this.CreateContext())
            {
                TransactionInfo2 transactionInfo2 = null;
                var transactionQuery = context.SysTransaction
                    .Where(ss => ss.StatusId == (int)TransactionStatus.Ready)
                    .Where(ss => (ss.ResultId ?? 0) == (int)TransactionResult.None)
                    .Where(ss => ss.NextExecutionDate <= DateTime.Now)
                    .Where(ss => ss.ClientId == transactionInfo.ClientId);
                if (transactionInfo.OperationCode != SysOperationCode.Unknown)
                {
                    transactionQuery = transactionQuery
                        .Where(ss => ss.OperationId == (int)transactionInfo.OperationCode);
                };
                SysTransaction sysTransaction;
                lock (transactionLock)
                {
                    sysTransaction = transactionQuery
                        .OrderBy(ss => ss.NextExecutionDate)
                        .FirstOrDefault();
                    if (sysTransaction != null)
                    {
                        sysTransaction.StatusId = (int)TransactionStatus.InProgress;
                        sysTransaction.LastExecutionDate = DateTime.Now;
                        sysTransaction.NextExecutionDate = null;
                        context.SaveChanges();
                    }
                }
                if (sysTransaction != null)
                {
                    transactionInfo2 = new TransactionInfo2
                    {
                        TransactionGuid = sysTransaction.TransactionGUID,
                        ClientId = sysTransaction.ClientId,
                        ListGuid = sysTransaction.ListGUID,
                        OperationCode = (SysOperationCode)sysTransaction.OperationId,
                    };
                }

                return transactionInfo2;
            }
        }

        public async Task ReleaseTransactionAsync(Guid transactionGuid, TransactionStatus transactionStatus)
        {
            using (var context = this.CreateContext())
            {
                SysTransaction sysTransaction = context.SysTransaction
                    .Where(ss => ss.TransactionGUID == transactionGuid)
                    .FirstOrDefault();
                if (sysTransaction != null)
                {
                    sysTransaction.StatusId = (int)transactionStatus;
                    if (transactionStatus != TransactionStatus.Completed)
                    {
                        sysTransaction.NextExecutionDate = DateTime.Now.AddSeconds(15);
                    }
                    else
                    {
                        sysTransaction.NextExecutionDate = null;
                    }
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task SetTransactionStatusAsync(Guid transactionGuid, TransactionStatus transactionStatus)
        {
            if (!Enum.IsDefined(typeof(TransactionStatus), transactionStatus))
            {
                throw new Exception("Неизвестный статус.");
            }

            using (var context = this.CreateContext())
            {
                this.AddTrace("SetTransactionStatus Begin");

                var sysTransaction = await context.SysTransaction.FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid);
                if (sysTransaction == null)
                {
                    throw new Exception("Транзакция не найдена.");
                }

                sysTransaction.StatusId = (int)transactionStatus;
                if (transactionStatus == TransactionStatus.Ready)
                {
                    sysTransaction.NextExecutionDate = DateTime.Now.AddSeconds(15);
                }
                else if (transactionStatus == TransactionStatus.Completed)
                {
                    sysTransaction.NextExecutionDate = null;
                }
                await context.SaveChangesAsync();

                this.AddTrace("SetTransactionStatus End");

                return;
            }
        }
        #endregion

        #region old
        /*
        protected ICollection GetNavigationCollection(object entity, string navigationProperty, bool load)
        {
            var navigationEntry = this.Context.Entry(entity).Member(navigationProperty);
            if (navigationEntry is DbCollectionEntry)
            {
                if (load)
                {
                    ((DbCollectionEntry)navigationEntry).Load();
                }
                return ((IEnumerable)navigationEntry.CurrentValue).OfType<object>().ToList();
            }
            else if (navigationEntry is DbReferenceEntry)
            {
                if (load)
                {
                    ((DbReferenceEntry)navigationEntry).Load();
                }
                return navigationEntry.CurrentValue != null ? new object[] { navigationEntry.CurrentValue } : new object[0];
            }

            throw new Exception("navigationProperty");
        }
        protected void GetEntitySet(IDictionary<Type, IList> entitySet, IEntityRelation entityRelation, object entity)
        {
            foreach (var navigation in entityRelation.Navigation)
            {
                IList list;
                if (!entitySet.TryGetValue(navigation.Value.Type, out list))
                {
                    list = new ArrayList();
                    entitySet.Add(navigation.Value.Type, list);
                }

                var collection = this.GetNavigationCollection(entity, navigation.Key, false);
                foreach (object navigationEntity in collection)
                {
                    list.Add(navigationEntity);
                    this.GetEntitySet(entitySet, navigation.Value, navigationEntity);
                }
            }
        }
        */
        #endregion
        // todo: async

        #region Context
        protected HcsContext CreateContext()
        {
            var context = HcsContext.CreateContext(this.configuration.ConnectionStringName, is_postgres);
            //$$$
            //context.ObjectContext.Connection.Open();
            //context.ObjectContext.CommandTimeout = this.Configuration.CommandTimeout;
            AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            string versionNumber = assemblyName.Version.ToString();
            var sysParams = context.SysParams
                .AsNoTracking()
                .FirstOrDefault();
            //$$$
            //if (sysParams == null || !String.Equals(sysParams.VersionNumber, versionNumber, StringComparison.Ordinal))
            //{
            //    throw new Exception(String.Format("Текущая версия промежуточной БД не поддерживается. Требуется версия {0}", versionNumber));
            //}

            return context;
        }
        protected void OnDataStoreCreating(EntityRelationBuilder entityRelationBuilder)
        {
            //entityRelationBuilder.EntityRelationSetAllTypes();
        }
        public String OnDataStoreCreating()
        {
            EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();
            entityRelationBuilder.EntityRelationSet(typeof(PaymentImportRequest));

            String str = "";
            if (entityRelationBuilder.EntityRelations != null)
            {
                //str = "<ul>";
                foreach (String _str in entityRelationBuilder.EntityRelations)
                {
                    //str += "<li>" + _str + "</li>";
                    str += $"\n" + _str;
                }
                //str += "</ul>";
            }
            return str;
        }
        #endregion

        //public Expression<Func<X, Z>> ExpressionProperty<X, Y, Z>(
        //    Expression<Func<X, Y>> fObj, 
        //    Expression<Func<Y, Z>> fProp
        //    )
        //{
        //    return Expression.Lambda<Func<X, Z>>(
        //        Expression.Property(fObj.Body, (fProp.Body as MemberExpression).Member as PropertyInfo), 
        //        fObj.Parameters
        //        );
        //}
        //public String OnDataStoreCreating()
        //{
        //    EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();
        //    entityRelationBuilder.EntityRelationSet(typeof(PaymentImportRequest));

        //    String str = "";
        //    if (entityRelationBuilder.EntityRelations != null)
        //    {
        //        //str = "<ul>";
        //        foreach (String _str in entityRelationBuilder.EntityRelations)
        //        {
        //            //str += "<li>" + _str + "</li>";
        //            str += $"\n" + _str;
        //        }
        //        //str += "</ul>";
        //    }
        //    return str;
        //}

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (this.Context != null)
                //{
                //    this.Context.Dispose();
                //}
            }
        }
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.Dispose(true);
                this.disposed = true;
            }
            GC.SuppressFinalize(this);
        }
        #endregion

        #region ILoggable
        public Action<string, string> Log { get; set; }

        protected void AddTrace(string header)
        {
            if (this.Log != null && this.configuration.Log != null && this.configuration.Log.Mode.HasFlag(LogMode.Trace))
            {
                this.Log(header, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));
            }
        }
        #endregion
    }
}

