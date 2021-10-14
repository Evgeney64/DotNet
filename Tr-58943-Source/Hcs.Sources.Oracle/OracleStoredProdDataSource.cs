﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
//using System.Data.Entity;
//using System.Data.Metadata.Edm;
//using System.Data.Objects;
//using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

using Hcs;
//using Hcs.DataSource
//using Hcs.Configuration;
//using HCS.Model;
//using Oracle.ManagedDataAccess.Client;
using Hcs.Configuration;
using Hcs.Model;

namespace Hcs.DataSource
{
    public partial class OracleStoredProdDataSource : IDataSource2, IDisposable, ILoggable
    {
        #region Define
        private Dictionary<SysOperationCode, Tuple<Type, Type>> sysOperationTypes = new Dictionary<SysOperationCode, Tuple<Type, Type>>();

        protected readonly EntityRelationBuilder entityRelationBuilder = new EntityRelationBuilder();
        public StoredProcDataSourceConfiguration Configuration { get; set; }
        private string connectionString { get { return Configuration.HcsConnectionStringName; } }
        #endregion

        #region Constructors
        public OracleStoredProdDataSource(StoredProcDataSourceConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            Configuration = configuration;
            OnDataStoreCreating(entityRelationBuilder);
        }

        protected void OnDataStoreCreating(EntityRelationBuilder entityRelationBuilder)
        {
            entityRelationBuilder.EntityRelationSetAllTypes();
            //base.OnDataStoreCreating(entityRelationBuilder);

            sysOperationTypes[SysOperationCode.NsiExport] = Tuple.Create(typeof(NsiExportRequest), typeof(NsiExportResult));
            sysOperationTypes[SysOperationCode.OrganizationExport] = Tuple.Create(typeof(OrganizationExportRequest), typeof(OrganizationExportResult));
            sysOperationTypes[SysOperationCode.AccountImport] = Tuple.Create(typeof(AccountImportRequest), typeof(AccountImportResult));
            sysOperationTypes[SysOperationCode.AccountExport] = Tuple.Create(typeof(AccountExportRequest), typeof(AccountExportResult));
            sysOperationTypes[SysOperationCode.AccountClose] = Tuple.Create(typeof(AccountCloseRequest), typeof(AccountCloseResult));
            sysOperationTypes[SysOperationCode.AckImport] = Tuple.Create(typeof(AckImportRequest), typeof(AckImportResult));
            sysOperationTypes[SysOperationCode.ContractImport] = Tuple.Create(typeof(ContractImportRequest), typeof(ContractImportResult));
            sysOperationTypes[SysOperationCode.SettlementImport] = Tuple.Create(typeof(SettlementImportRequest), typeof(SettlementImportResult));
            sysOperationTypes[SysOperationCode.DeviceImport] = Tuple.Create(typeof(DeviceImportRequest), typeof(DeviceImportResult));
            sysOperationTypes[SysOperationCode.DeviceExport] = Tuple.Create(typeof(DeviceExportRequest), typeof(DeviceExportResult));
            sysOperationTypes[SysOperationCode.DeviceValueImport] = Tuple.Create(typeof(DeviceValueImportRequest), typeof(DeviceValueImportResult));
            sysOperationTypes[SysOperationCode.DeviceValueExport] = Tuple.Create(typeof(DeviceValueExportRequest), typeof(DeviceValueExportResult));
            sysOperationTypes[SysOperationCode.HouseImport] = Tuple.Create(typeof(HouseImportRequest), typeof(HouseImportResult));
            sysOperationTypes[SysOperationCode.HouseExport] = Tuple.Create(typeof(HouseExportRequest), typeof(HouseExportResult));
            sysOperationTypes[SysOperationCode.NotificationImport] = Tuple.Create(typeof(NotificationImportRequest), typeof(NotificationImportResult));
            sysOperationTypes[SysOperationCode.OrderImport] = Tuple.Create(typeof(OrderImportRequest), typeof(OrderImportResult));
            sysOperationTypes[SysOperationCode.PaymentDocumentImport] = Tuple.Create(typeof(PaymentImportRequest), typeof(PaymentImportResult));
            sysOperationTypes[SysOperationCode.PaymentDocumentExport] = Tuple.Create(typeof(PaymentExportRequest), typeof(PaymentExportResult));
        }
        #endregion

        #region CreateConnection
        protected OracleConnection CreateConnection()
        {
            var connection = new OracleConnection(connectionString);
            return connection;
        }
        #endregion

        #region SysOperationCode
        private SysOperationCode getSysOperationCodeByRequestType(Type type)
        {
            var sysItems = this.sysOperationTypes
                .Where(ss => ss.Value.Item1 == type)
                .ToList();
            if (sysItems.Count == 0)
            {
                throw new Exception($"Не найдено соответствие кода операции для типа {type.FullName}.");
            }
            if (sysItems.Count > 1)
            {
                throw new Exception($"Найдено несколько соответствий кода операции для типа {type.FullName}.");
            }

            return sysItems.First().Key;
        }
        private SysOperationCode getSysOperationCodeByResultType(Type type)
        {
            var sysItems = this.sysOperationTypes
                .Where(ss => ss.Value.Item2 == type)
                .ToList();
            if (sysItems.Count == 0)
            {
                throw new Exception($"Не найдено соответствие кода операции для типа {type.FullName}.");
            }
            if (sysItems.Count > 1)
            {
                throw new Exception($"Найдено несколько соответствий кода операции для типа {type.FullName}.");
            }

            return sysItems.First().Key;
        }
        #endregion

        #region old
        //protected SqlConnection CreateConnection(string connectionStringName)
        //{
        //    if (connectionStringName == null)
        //    {
        //        throw new ArgumentNullException("connectionStringName");
        //    }

        //    ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        //    if (connectionStringSettings == null)
        //    {
        //        throw new SettingsPropertyNotFoundException();
        //    }

        //    SqlConnection connection = new SqlConnection(connectionStringSettings.ConnectionString);
        //    return connection;
        //}
        //protected override void ApplyConfiguration(DataSourceElement dataSource)
        //{
        //    base.ApplyConfiguration(dataSource);
        //    this.Configuration.ExternalConnectionStringName = dataSource.ExternalConnectionStringName;
        //    if (dataSource.StoredProcedures != null)
        //    {
        //        foreach (StoredProcedureElement storedProcElement in dataSource.StoredProcedures)
        //        {
        //            this.Configuration[storedProcElement.Operation] = new StoredProcConfiguration
        //            {
        //                ListProcedureName = storedProcElement.ListProcedureName,
        //                PrepareProcedureName = storedProcElement.PrepareProcedureName,
        //                ResultProcedureName = storedProcElement.ResultProcedureName,
        //            };
        //        }
        //    }
        //}

        //protected HcsContext CreateContext()
        //{
        //    var context = HcsContext.CreateContext(this.Configuration.HcsConnectionStringName);
        //    //$$$
        //    //context.ObjectContext.Connection.Open();
        //    //context.ObjectContext.CommandTimeout = this.Configuration.CommandTimeout;
        //    AssemblyName assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
        //    string versionNumber = assemblyName.Version.ToString();
        //    //var sysParams = context.SysParams
        //    //    .AsNoTracking()
        //    //    .FirstOrDefault();

        //    return context;
        //}

        //private async Task<IEnumerable<ObjectInfo>> GetTemporaryListDataSetAsync(HcsContext context, Guid transactionGuid)
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        this.AddTrace("GetTemporaryListDataSetAsync Begin");

        //        command.CommandTimeout = this.Configuration.CommandTimeout;
        //        command.InitialLOBFetchSize = -1;

        //        IList<ObjectInfo> result = new List<ObjectInfo>();
        //        IList<ObjectInfoError> resultError = new List<ObjectInfoError>();
        //        using (var context = HCSEdm.CreateContext(this.Configuration.HcsConnectionStringName))
        //        {
        //            command.CommandText = "SELECT OBJECTID, to_clob(\"comment\") AS \"comment\", OPERATION, NVL(\"group\",0) AS \"group\", to_clob(PARAM) AS PARAM FROM cisgkh.SYSTRANSACTIONOBJECT";
        //            using (var reader = command.ExecuteReader())
        //            {
        //                result = context.ObjectContext
        //                    .Translate<ObjectInfo>(reader)
        //                    .ToList();
        //                //while (await reader.ReadAsync())
        //                //{
        //                //    var objectInfo = new ObjectInfo
        //                //    {
        //                //        ObjectId = await reader.GetFieldValueAsync<string>(0),
        //                //        Comment = !await reader.IsDBNullAsync(1) ? await reader.GetFieldValueAsync<string>(1) : null,
        //                //        Operation = (int)await reader.GetFieldValueAsync<decimal>(2),
        //                //        Group = !await reader.IsDBNullAsync(3) ? await reader.GetFieldValueAsync<int>(3) : 0,
        //                //        Param = !await reader.IsDBNullAsync(4) ? await reader.GetFieldValueAsync<string>(4) : null,
        //                //    };
        //                //    result.Add(objectInfo);
        //                //}
        //            }
        //            command.CommandText = "SELECT OBJECTID, ERRORCODE, ERRORDESCRIPTION FROM cisgkh.SYSTRANSACTIONOBJECTERROR";
        //            using (var reader = await command.ExecuteReaderAsync())
        //            {
        //                resultError = context.ObjectContext
        //                    .Translate<ObjectInfoError>(reader)
        //                    .ToList();
        //                //while (await reader.ReadAsync())
        //                //{
        //                //    var objectInfoError = new ObjectInfoError
        //                //    {
        //                //        ObjectId = await reader.GetFieldValueAsync<string>(0),
        //                //        ErrorCode = await reader.GetFieldValueAsync<string>(1),
        //                //        ErrorDescription = await reader.GetFieldValueAsync<string>(2),
        //                //    };
        //                //    resultError.Add(objectInfoError);
        //                //}
        //            }
        //        }

        //        foreach (var objectError in resultError.GroupBy(e => e.ObjectId))
        //        {
        //            var obj = result.FirstOrDefault(o => o.ObjectId == objectError.Key);
        //            if (obj != null)
        //            {
        //                obj.Errors = objectError.ToArray();
        //            }
        //        }

        //        this.AddTrace("GetTemporaryListDataSetAsync End");

        //        return result;
        //    }
        //}
        #endregion

        #region IDataSource2
        public virtual async Task<IEnumerable<T>> TakeDataAsync<T>(Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
            where T : class
        {
            using (var connection = this.CreateConnection())
            {
                this.AddTrace("TakeDataAsync Begin");

                await connection.OpenAsync();
                IEnumerable<T> data;
                data = null;
                try
                {
                    await this.SetTemporaryListDataSetAsync(connection, objectList, transactionGuid);
                    
                    SysOperationCode sysOperationCode = this.getSysOperationCodeByRequestType(typeof(T));
                    await this.PrepareTemporaryDataSetAsync(connection, transactionGuid, sysOperationCode);
                    data = await this.GetTemporaryDataSetAsync<T>(connection, transactionGuid);
                    
                    IEnumerable<ObjectInfoError> objectListError = await this.GetTemporaryListErrorDataSetAsync(connection, transactionGuid);
                    { }
                    foreach (var objectError in objectListError.GroupBy(e => e.ObjectId))
                    {
                        var obj = objectList.FirstOrDefault(o => o.ObjectId == objectError.Key);
                        if (obj != null)
                        {
                            obj.Errors = objectError.ToArray();
                        }
                    }
                    // здесь надо проверить, по всем ли запрошенным сущностям вернулась информация
                    // вообще надо все, что возможно проверить по запрашиваемому списку
                    //if (typeof(ITransactionObjectEntity).IsAssignableFrom(typeof(TRequestData)))
                    //{
                    //
                    //}
                    
                }
                finally
                {
                    await this.TruncateTemporaryListDataSetAsync(connection, transactionGuid);
                    await this.TruncateTemporaryDataSetAsync<T>(connection, transactionGuid);
                }

                this.AddTrace("TakeDataAsync End");

                return data;
            }
        }
        public virtual async Task PassDataAsync<T>(IEnumerable<T> data, Guid transactionGuid, IEnumerable<ObjectInfo> objectList)
            where T : class
        {
            using (var connection = this.CreateConnection())
            {
                this.AddTrace("PassDataAsync Begin");

                await connection.OpenAsync();

                //if (typeof(IResultEntity).IsAssignableFrom(typeof(T)))
                //{
                //    foreach (IResultEntity dataItem in data)
                //    {
                //        var obj = objectList.FirstOrDefault(o => o.ObjectId == dataItem.objectId);
                //        if (obj != null)
                //        {
                //            obj.Errors = dataItem.ResultErrors.Select(e => new ObjectInfoError
                //            {
                //                ObjectId = obj.ObjectId,
                //                ErrorCode = e.ErrorCode,
                //                ErrorDescription = e.ErrorDescription,
                //            }).ToArray();
                //        }
                //    }
                //}
                try
                {
                    await this.SetTemporaryListDataSetAsync(connection, objectList, transactionGuid);

                    SysOperationCode sysOperationCode = this.getSysOperationCodeByResultType(typeof(T));
                    await this.SetTemporaryDataSetAsync(connection, data);
                    await this.ProcessTemporaryDataSetAsync(connection, transactionGuid, sysOperationCode);
                }
                finally
                {
                    await this.TruncateTemporaryListDataSetAsync(connection, transactionGuid);
                    await this.TruncateTemporaryDataSetAsync<T>(connection, transactionGuid);
                }

                this.AddTrace("PassDataAsync End");
            }
        }
        public virtual async Task<IEnumerable<ObjectInfo>> ListAsync(SysOperationCode operationCode)
        {
            using (var connection = this.CreateConnection())
            {
                this.AddTrace("ListAsync Begin");

                await connection.OpenAsync();
                IEnumerable<ObjectInfo> listData;
                try
                {
                    Guid transactionGuid = Guid.NewGuid();
                    await this.PrepareTemporaryListDataSetAsync(connection, transactionGuid, operationCode);

                    listData = await this.GetTemporaryListDataSetAsync(connection, transactionGuid);
                }
                finally
                {
                    await this.TruncateTemporaryListDataSetAsync(connection, Guid.Empty);
                }

                this.AddTrace("ListAsync End");

                return listData;
            }
        }
        #endregion

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
            if (this.Log != null && this.Configuration.Log != null && this.Configuration.Log.Mode.HasFlag(LogMode.Trace))
            {
                this.Log(header, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff"));
            }
        }
        #endregion
    }
}


