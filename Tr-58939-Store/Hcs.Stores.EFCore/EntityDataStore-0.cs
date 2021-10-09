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
using Hcs;
using Hcs.Model;
using Hcs.Stores;
using System.Linq.Expressions;
#endregion

namespace Hcs.DataSources
{
    public class EntityDataStoreNew : IDataStore2, IDisposable, ILoggable
    {
        private bool disposed = false;
        protected readonly EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();

        //protected HCSEdm Context { get; set; }
        public Action<string, string> Log { get; set; }
        public EntityDataSourceConfiguration Configuration { get; private set; }

        #region Constructors
        public EntityDataStoreNew(EntityDataSourceConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.Configuration = configuration;
            this.OnDataStoreCreating(entityRelationBuilder);
        }
        //protected EntityDataStoreNew(EntityDataSourceConfiguration configuration, string dataSourceName)
        //    : this(configuration)
        //{
        //    if (dataSourceName == null)
        //    {
        //        throw new ArgumentNullException("dataSourceName");
        //    }

        //    DataSourceElement dataSourceElement = DataSourcesSection.GetSource(dataSourceName);
        //    this.ApplyConfiguration(dataSourceElement);
        //}
        //public EntityDataStoreNew()
        //    : this(new EntityDataSourceConfiguration())
        //{
        //}
        //public EntityDataStoreNew(string dataSourceName)
        //    : this(new EntityDataSourceConfiguration(), dataSourceName)
        //{
        //}
        #endregion

        protected void AddTrace(string header)
        {
            if (this.Log != null && this.Configuration.Log != null && this.Configuration.Log.Mode.HasFlag(LogMode.Trace))
            {
                this.Log(header, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));
            }
        }
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
                throw new DataSourceException(e);
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
                throw new DataSourceException(e);
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
                throw new DataSourceException(e);
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
                throw new DataSourceException(e);
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
                throw new DataSourceException(e);
            }
        }

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
        protected HcsContext CreateContext()
        {
            var context = HcsContext.CreateContext(this.Configuration.HcsConnectionStringName);
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
        protected virtual void OnDataStoreCreating(EntityRelationBuilder entityRelationBuilder)
        {
            if (1 == 2)
            {
                entityRelationBuilder.Entity<PaymentImportRequest>(e =>
                    {
                    //e.NavigateStr<PaymentImportRequestChargesMunicipalService>("PaymentImportRequestChargesMunicipalServices");
                    //e.NavigateStr<PaymentImportRequestChargesMunicipalServiceNorm>("PaymentImportRequestChargesMunicipalServiceNorm");
                    //e.NavigateStr<PaymentImportRequestDebtMunicipalService>("PaymentImportRequestDebtMunicipalServices");
                    { }
                        e.Navigate(a => a.PaymentImportRequestChargesMunicipalServices)
                            .Navigate(s => s.PaymentImportRequestChargesMunicipalServiceNorm);
                        e.Navigate(a => a.PaymentImportRequestPenaltyAndCourtCosts);
                        e.Navigate(a => a.PaymentImportRequestDebtMunicipalServices);
                        //onDataStoreCreating(e);
                        //onDataStoreCreating(typeof(PaymentImportRequest), e);
                        { }
                    });
            }
            else
            {
                entityRelationSet(typeof(PaymentImportRequest));
            }
        }
        List<string> entityRelations = new List<string>();
        public void entityRelationSet(Type type)
        {
            MethodInfo method = typeof(EntityRelationBuilder).GetMethod("EntitySet");
            MethodInfo methodGen = method.MakeGenericMethod(new[] { type });
            IEntityRelation item = (IEntityRelation)methodGen.Invoke(entityRelationBuilder, new object[] { });

            entityRelations = new List<string>();
            entityRelations.Add(type.Name);

            entityRelationRecurce(item, type, 0);
        }
        public void entityRelationRecurce(IEntityRelation item, Type type, int step)
        {
            foreach (PropertyInfo prop in type.GetProperties()
                //.Where(ss => ss.CustomAttributes
                //    .Where(ss1 => ss1.AttributeType.Name == "InversePropertyAttribute").Count() > 0)
                )
            {
                if (prop.CustomAttributes != null
                    && prop.CustomAttributes.Where(ss1 => ss1.AttributeType.Name == "InversePropertyAttribute").Count() > 0)
                {
                    Type type1 = prop.PropertyType;
                    if (prop.PropertyType.GetGenericArguments().Count() == 1)
                        type1 = prop.PropertyType.GetGenericArguments().Single();

                    MethodInfo method1 = item.GetType().GetMethod("NavigateSet");
                    MethodInfo methodGen1 = method1.MakeGenericMethod(new[] { type1 });
                    IEntityRelation item1 = (IEntityRelation)methodGen1.Invoke(item, new object[] { prop.Name });
                    if (entityRelations.Contains(prop.Name) == false
                        && entityRelations.Count() < 20)
                    {
                        entityRelationRecurce(item1, type1, step + 1);
                    }
                    entityRelations.Add(prop.Name);
                }
            }
        }
        protected virtual void OnDataStoreCreating_Old(EntityRelationBuilder entityRelationBuilder)
        {
            #region Nsi
            entityRelationBuilder
                .Entity<NsiExportRequest>()
                .Entity<NsiExportResult>(e =>
                {
                    e.Navigate(a => a.NsiExportResultFields);
                });
            #endregion

            #region Attachment
            entityRelationBuilder
                .Entity<AttachmentPostRequest>()
                .Entity<AttachmentPostResult>();
            #endregion

            #region Organization
            entityRelationBuilder
                .Entity<OrganizationExportRequest>(e =>
                {
                    e.Navigate(s => s.OrganizationExportRequestData);
                })
                .Entity<OrganizationExportResult>(e =>
                {
                    e.Navigate(s => s.OrganizationExportResultLegal);
                    e.Navigate(s => s.OrganizationExportResultEntp);
                    e.Navigate(s => s.OrganizationExportResultRoles);
                });
            #endregion

            #region Account
            entityRelationBuilder
                .Entity<AccountImportRequest>(e =>
                {
                    e.Navigate(a => a.AccountImportRequestReasons);
                    e.Navigate(a => a.AccountImportRequestPayer);
                    e.Navigate(a => a.AccountImportRequestPercentPremises);
                })
                .Entity<AccountImportResult>(e =>
                {
                    e.Navigate(a => a.AccountImportResultErrors);
                })
                .Entity<AccountExportRequest>()
                .Entity<AccountExportResult>(e =>
                {
                    e.Navigate(a => a.AccountExportResultPercentPremises);
                })
                .Entity<AccountCloseRequest>()
                .Entity<AccountCloseResult>(e =>
                {
                    e.Navigate(a => a.AccountCloseResultAccounts)
                        .Navigate(a => a.AccountCloseResultAccountErrors);
                });
            #endregion

            #region Ack
            entityRelationBuilder
                .Entity<AckImportRequest>()
                .Entity<AckImportCancellationRequest>()
                .Entity<AckImportResult>(e =>
                {
                    e.Navigate(a => a.AckImportResultErrors);
                });
            #endregion

            #region Contract
            entityRelationBuilder
                .Entity<ContractImportRequest>(e =>
                {
                    e.Navigate(c => c.ContractImportRequestObjectAddresses)
                        .Navigate(o => o.ContractImportRequestObjectServiceResources);
                    e.Navigate(c => c.ContractImportRequestSubjects)
                        .Navigate(s => s.ContractImportRequestSubjectQualityIndicators);
                    e.Navigate(c => c.ContractImportRequestParty);
                    e.Navigate(c => c.ContractImportRequestAttachments);
                })
                .Entity<ContractImportResult>(e =>
                {
                    e.Navigate(c => c.ContractImportResultErrors);
                });
            #endregion

            #region Settlement
            entityRelationBuilder
                .Entity<SettlementImportRequest>(e =>
                {
                    var per = e.Navigate(s => s.SettlementImportRequestPeriods);
                    per.Navigate(s => s.SettlementImportRequestPeriodInfo);
                    per.Navigate(s => s.SettlementImportRequestPeriodAnnulment);
                })
                .Entity<SettlementImportAnnulmentRequest>()
                .Entity<SettlementImportResult>(e =>
                {
                    e.Navigate(a => a.SettlementImportResultErrors);
                });
            #endregion

            #region Device
            entityRelationBuilder
                .Entity<DeviceImportRequest>(e =>
                {
                    e.Navigate(c => c.DeviceImportRequestAccounts);
                    e.Navigate(c => c.DeviceImportRequestAddresses);
                    e.Navigate(c => c.DeviceImportRequestLinkedDevices);
                    e.Navigate(c => c.DeviceImportRequestValues);
                })
                .Entity<DeviceImportArchiveRequest>()
                .Entity<DeviceImportReplaceRequest>(e =>
                {
                    e.Navigate(c => c.DeviceImportReplaceRequestValues);
                })
                .Entity<DeviceImportResult>(e =>
                {
                    e.Navigate(c => c.DeviceImportResultErrors);
                });
            #endregion

            #region DeviceValue
            entityRelationBuilder
                .Entity<DeviceValueImportRequest>()
                .Entity<DeviceValueImportResult>(e =>
                {
                    e.Navigate(c => c.DeviceValueImportResultErrors);
                })
                .Entity<DeviceValueExportRequest>(e =>
                {
                    e.Navigate(c => c.DeviceValueExportRequestDevices);
                    e.Navigate(c => c.DeviceValueExportRequestDeviceTypes);
                    e.Navigate(c => c.DeviceValueExportRequestMunicipalResources);
                })
                .Entity<DeviceValueExportResult>();
            #endregion

            #region House
            entityRelationBuilder
                .Entity<HouseImportRequest>(e =>
                {
                    e.Navigate(c => c.HouseImportRequestBlocks);
                    e.Navigate(c => c.HouseImportRequestEntrances);
                    e.Navigate(c => c.HouseImportRequestPremises);
                    e.Navigate(c => c.HouseImportRequestLivingRooms);
                })
                .Entity<HouseImportResult>(e =>
                {
                    e.Navigate(c => c.HouseImportResultErrors);
                    e.Navigate(c => c.HouseImportResultBlocks)
                        .Navigate(s => s.HouseImportResultBlockErrors);
                    e.Navigate(c => c.HouseImportResultEntrances)
                        .Navigate(o => o.HouseImportResultEntranceErrors);
                    e.Navigate(c => c.HouseImportResultPremises)
                        .Navigate(s => s.HouseImportResultPremiseErrors);
                    e.Navigate(c => c.HouseImportResultLivingRooms)
                        .Navigate(o => o.HouseImportResultLivingRoomErrors);
                })
                .Entity<HouseExportRequest>()
                .Entity<HouseExportResult>(e =>
                {
                    e.Navigate(c => c.HouseExportResultBlocks);
                    e.Navigate(c => c.HouseExportResultEntrances);
                    e.Navigate(c => c.HouseExportResultPremises);
                    e.Navigate(c => c.HouseExportResultLivingRooms);
                });
            #endregion

            #region Notification
            entityRelationBuilder
                .Entity<NotificationImportRequest>(e =>
                {
                    e.Navigate(s => s.NotificationImportRequestAccountDebts);
                })
                .Entity<NotificationImportDeleteRequest>()
                .Entity<NotificationImportResult>(e =>
                {
                    e.Navigate(c => c.NotificationImportResultErrors);
                });
            #endregion

            #region Order
            entityRelationBuilder
                .Entity<OrderImportRequest>()
                .Entity<OrderImportCancellationRequest>()
                .Entity<OrderImportResult>(e =>
                {
                    e.Navigate(a => a.OrderImportResultErrors);
                });
            #endregion

            #region Payment
            entityRelationBuilder
                .Entity<PaymentImportRequest>(e =>
                {
                    e.Navigate(a => a.PaymentImportRequestChargesMunicipalServices)
                        .Navigate(s => s.PaymentImportRequestChargesMunicipalServiceNorm);
                    e.Navigate(a => a.PaymentImportRequestPenaltyAndCourtCosts);
                    e.Navigate(a => a.PaymentImportRequestDebtMunicipalServices);
                })
                .Entity<PaymentImportWithdrawRequest>()
                .Entity<PaymentImportResult>(e =>
                {
                    e.Navigate(a => a.PaymentImportResultErrors);
                })
                .Entity<PaymentExportRequest>(e =>
                {
                    e.Navigate(a => a.PaymentExportRequestDocuments);
                    e.Navigate(a => a.PaymentExportRequestAccounts);
                })
                .Entity<PaymentExportResult>();
            #endregion
        }
        //protected virtual void ApplyConfiguration(DataSourceElement dataSource)
        //{
        //    if (dataSource == null)
        //    {
        //        throw new Exception("Не задана конфигурация источника.");
        //    }

        //    this.Configuration.HcsConnectionStringName = dataSource.HCSConnectionStringName;
        //    this.Configuration.SourceId = dataSource.UniqueId;
        //    this.Configuration.CommandTimeout = dataSource.CommandTimeout;

        //    if (dataSource.Log.ElementInformation.IsPresent)
        //    {
        //        this.Configuration.Log.Mode = dataSource.Log.Mode;
        //    }
        //    else if (dataSource.DefaultLogModeSpecified)
        //    {
        //        this.Configuration.Log.Mode = dataSource.DefaultLogMode;
        //    }
        //}
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
    }
}

