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
    public class EntityDataStore : IDataStore, IDisposable, ILoggable
    {
        private readonly EntityDataStoreConfiguration _configuration;
        protected readonly EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();

        #region Constructors
        public EntityDataStore(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this._configuration = configuration.Get<EntityDataStoreConfiguration>();
        }
        public EntityDataStore(EntityDataStoreConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this._configuration = configuration;
            //this.OnDataStoreCreating(entityRelationBuilder);
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
                //$$$
                if (entityRelationType == typeof(T))
                {
                    //dataSet = context.Set<T>()
                    //    .SqlQuery(select)
                    //    .ToList();
                    dataSet = context.Set<T>()
                        .FromSqlRaw(select)
                        .ToList();
                }
                else
                {
                    dataSet = context.Set<T>()
                        .FromSqlRaw(select)
                        .OfType<T>()
                        .ToList();
                    //context.Set(entityRelationType)
                    //    .SqlQuery(select)
                    //    .OfType<object>()
                    //    .ToList();
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
                    .FirstOrDefaultAsync(r => r.TransactionGUID == transactionGuid)
                    ;
                //var existingRequest = context.SysTransaction.FirstOrDefault(r => r.TransactionGUID == transactionGuid);
                if (existingRequest != null)
                {
                    throw new Exception("Транзакция уже существует.");
                }

                SysTransaction request = new SysTransaction
                {
                    TransactionGUID = transactionGuid,
                    OperationId = (int)transaction.OperationCode,
                    TransactionDate = DateTime.Now,
                    TransactionData = transaction.Data,
                };
                context.SysTransaction.Add(request);
                context.SaveChanges();

                this.AddTrace("CreateTransaction End");

                return transactionGuid;
                //return Task.FromResult(transactionGuid);
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
            if (!result.IsCompleted || result.Result == TransactionResult.None)
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

                request.TransactionResult = (int)result.Result;
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
                switch ((TransactionResult)(request.TransactionResult ?? 0))
                {

                    case TransactionResult.TransactionError:
                        transactionResult = TransactionResultInfo.TransactionError(ErrorInfo.Create(request.ErrorCode, request.ErrorDescription), true);
                        break;
                    case TransactionResult.Success:
                    case TransactionResult.ObjectError:
                        var transactionObjects = await this.getTransactionObjectsAsync(transactionGuid, context);
                        transactionResult = TransactionResultInfo.Create(transactionObjects);
                        break;
                    case TransactionResult.None:
                    default:
                        transactionResult = TransactionResultInfo.None();
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

        private async Task<IEnumerable<ObjectInfo>> getTransactionObjectsAsync(Guid transactionGuid, HcsContext context)
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
        public async Task SetTransactionObjectsAsync(Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
        {
            if (objectList == null)
            {
                throw new ArgumentNullException(nameof(objectList));
            }

            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("SetTransactionObjects Begin");

                    var transactionObjects = objectList
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
                    var transactionObjectErrors = objectList
                        .Where(ss => ss.Errors != null)
                        .SelectMany(ss => ss.Errors)
                        .Select(ss => new SysTransactionObjectError
                        {
                            TransactionGUID = transactionGuid,
                            objectId = ss.ObjectId,
                            errorCode = ss.ErrorCode,
                            errorDescription = ss.ErrorDescription,
                        })
                        .ToList();
                    context.AddRange(transactionObjects);
                    context.AddRange(transactionObjectErrors);
                    await context.SaveChangesAsync();
                    //context.SaveChanges();

                    this.AddTrace("SetTransactionObjects End");

                    return;
                    //return Task.FromResult(0);
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }
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
        public async Task<IEnumerable<ObjectInfo>> GetTransactionObjectsAsync(Guid transactionGuid)
        {
            try
            {
                using (var context = this.CreateContext())
                {
                    this.AddTrace("GetTransactionObjects Begin");

                    var transactionObjects = await this.getTransactionObjectsAsync(transactionGuid, context);

                    this.AddTrace("GetTransactionObjects End");

                    return transactionObjects;
                }
            }
            catch (Exception e)
            {
                throw new DataStoreException(e);
            }
        }

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

        public Task<IEnumerable<Guid>> SetTransactionObjectsAsync(TransactionInfo transactionInfo, IEnumerable<ObjectInfo> objectList)
        {
            throw new NotImplementedException();
        }
        public Task<TransactionInfo> GetTransactionAsync(Guid transactionGuid)
        {
            throw new NotImplementedException();
        }
        public Task<Guid?> AcquireTransactionAsync(TransactionInfo transactionInfo)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseTransactionAsync(Guid transactionGuid)
        {
            throw new NotImplementedException();
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
        protected HcsContext CreateContext()
        {
            var context = HcsContext.CreateContext(this._configuration.ConnectionStringName);
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

        public Expression<Func<X, Z>> ExpressionProperty<X, Y, Z>(
            Expression<Func<X, Y>> fObj, 
            Expression<Func<Y, Z>> fProp
            )
        {
            return Expression.Lambda<Func<X, Z>>(
                Expression.Property(fObj.Body, (fProp.Body as MemberExpression).Member as PropertyInfo), 
                fObj.Parameters
                );
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
            if (this.Log != null && this._configuration.Log != null && this._configuration.Log.Mode.HasFlag(LogMode.Trace))
            {
                this.Log(header, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));
            }
        }
        #endregion
    }
}

