using System;
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
        #region objectList
        private async Task<IEnumerable<ObjectInfo>> GetTemporaryListDataSetAsync(OracleConnection connection, Guid transactionGuid)
        {
            using (var command = connection.CreateCommand())
            {
                this.AddTrace("GetTemporaryListDataSetAsync Begin");

                command.CommandTimeout = this.Configuration.CommandTimeout;
                command.InitialLOBFetchSize = -1;

                IList<ObjectInfo> result = new List<ObjectInfo>();
                IList<ObjectInfoError> resultError = new List<ObjectInfoError>();

                #region temp
                //using (var context = HCSEdm.CreateContext(this.Configuration.HcsConnectionStringName))
                //{
                //    command.CommandText = "SELECT OBJECTID, to_clob(\"comment\") AS \"comment\", OPERATION, NVL(\"group\",0) AS \"group\", to_clob(PARAM) AS PARAM FROM cisgkh.SYSTRANSACTIONOBJECT";
                //    using (var reader = command.ExecuteReader())
                //    {
                //        result = context.ObjectContext
                //            .Translate<ObjectInfo>(reader)
                //            .ToList();
                //        //while (await reader.ReadAsync())
                //        //{
                //        //    var objectInfo = new ObjectInfo
                //        //    {
                //        //        ObjectId = await reader.GetFieldValueAsync<string>(0),
                //        //        Comment = !await reader.IsDBNullAsync(1) ? await reader.GetFieldValueAsync<string>(1) : null,
                //        //        Operation = (int)await reader.GetFieldValueAsync<decimal>(2),
                //        //        Group = !await reader.IsDBNullAsync(3) ? await reader.GetFieldValueAsync<int>(3) : 0,
                //        //        Param = !await reader.IsDBNullAsync(4) ? await reader.GetFieldValueAsync<string>(4) : null,
                //        //    };
                //        //    result.Add(objectInfo);
                //        //}
                //    }
                //    command.CommandText = "SELECT OBJECTID, ERRORCODE, ERRORDESCRIPTION FROM cisgkh.SYSTRANSACTIONOBJECTERROR";
                //    using (var reader = await command.ExecuteReaderAsync())
                //    {
                //        resultError = context.ObjectContext
                //            .Translate<ObjectInfoError>(reader)
                //            .ToList();
                //        //while (await reader.ReadAsync())
                //        //{
                //        //    var objectInfoError = new ObjectInfoError
                //        //    {
                //        //        ObjectId = await reader.GetFieldValueAsync<string>(0),
                //        //        ErrorCode = await reader.GetFieldValueAsync<string>(1),
                //        //        ErrorDescription = await reader.GetFieldValueAsync<string>(2),
                //        //    };
                //        //    resultError.Add(objectInfoError);
                //        //}
                //    }
                //}
                #endregion

                foreach (var objectError in resultError.GroupBy(e => e.ObjectId))
                {
                    var obj = result.FirstOrDefault(o => o.ObjectId == objectError.Key);
                    if (obj != null)
                    {
                        obj.Errors = objectError.ToArray();
                    }
                }

                this.AddTrace("GetTemporaryListDataSetAsync End");

                return result;
            }
        }
        private async Task<IEnumerable<ObjectInfoError>> GetTemporaryListErrorDataSetAsync(OracleConnection connection, Guid transactionGuid)
        {
            using (var command = new OracleCommand())
            {
                this.AddTrace("GetTemporaryListErrorDataSetAsync Begin");

                IList<ObjectInfoError> resultError = new List<ObjectInfoError>();

                command.Connection = connection;
                command.CommandText = "SELECT OBJECTID, ERRORCODE, ERRORDESCRIPTION FROM cisgkh.SYSTRANSACTIONOBJECTERROR";
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var objectInfoError = new ObjectInfoError
                        {
                            ObjectId = await reader.GetFieldValueAsync<string>(0),
                            ErrorCode = await reader.GetFieldValueAsync<string>(1),
                            ErrorDescription = await reader.GetFieldValueAsync<string>(2),
                        };
                        resultError.Add(objectInfoError);
                    }
                }

                this.AddTrace("GetTemporaryListErrorDataSetAsync End");

                return resultError;
            }
        }
        private async Task SetTemporaryListDataSetAsync(OracleConnection connection, IEnumerable<ObjectInfo> objectList, Guid transactionGuid)
        {
            if (objectList.Count() < 1)
            {
                throw new Exception("Пустой список.");
            }

            using (var command = new OracleCommand())
            {
                this.AddTrace("SetTemporaryListDataSetAsync Begin");

                command.Connection = connection;
                command.CommandTimeout = this.Configuration.CommandTimeout;

                string insert =
                    "INSERT INTO cisgkh.SYSTRANSACTIONOBJECT (TRANSACTIONGUID, OBJECTID, \"comment\", OPERATION, \"group\", PARAM) " +
                    String.Join(" UNION ALL ", objectList.Select(o => $"SELECT '{transactionGuid}', '{o.ObjectId}', '{o.Comment?.Replace("'", "''")}', {o.Operation}, {o.Group}, '{o.Param?.Replace("'", "''")}' FROM DUAL"));
                command.CommandText = insert;
                await command.ExecuteNonQueryAsync();

                var objectListError = objectList.SelectMany(o => o.Errors ?? Enumerable.Empty<ObjectInfoError>()).ToList();
                if (objectListError.Count() > 0)
                {
                    string insertError =
                        "INSERT INTO cisgkh.SYSTRANSACTIONOBJECTERROR (TRANSACTIONGUID, OBJECTID, ERRORCODE, ERRORDESCRIPTION) " +
                        String.Join(" UNION ALL ", objectListError.Select(o => $"SELECT '{transactionGuid}', '{o.ObjectId}', '{o.ErrorCode}', '{o.ErrorDescription?.Replace("'", "''")}' FROM DUAL"));
                    command.CommandText = insertError;
                    await command.ExecuteNonQueryAsync();
                }

                this.AddTrace("SetTemporaryListDataSetAsync End");
            }
        }
        private async Task PrepareTemporaryListDataSetAsync(OracleConnection connection, Guid transactionGUID, SysOperationCode operationCode)
        {
            var procedures = this.Configuration[operationCode];
            if (procedures == null || procedures.ListProcedureName == null)
            {
                throw new Exception("Не задано имя процедуры для получения списка данных.");
            }

            this.AddTrace("PrepareTemporaryListDataSetAsync Begin");

            var transactionGUIDParameter = new OracleParameter("TransactionGUID", OracleDbType.Char, transactionGUID.ToString(), ParameterDirection.Input);
            //try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = this.Configuration.CommandTimeout;
                    command.CommandText = String.Format("BEGIN cisgkh.HCS.{0} (:TransactionGUID); END;", procedures.ListProcedureName);
                    command.Parameters.Add(transactionGUIDParameter);
                    await command.ExecuteNonQueryAsync();
                }
            }
            //catch (System.Data.SqlClient.SqlException e)
            //{
            //    throw new ExecSPException(procedures.ListProcedureName, e);
            //}

            this.AddTrace("PrepareTemporaryListDataSetAsync End");
        }
        private async Task TruncateTemporaryListDataSetAsync(OracleConnection connection, Guid transactionGuid)
        {
            this.AddTrace("TruncateTemporaryListDataSetAsync Begin");

            using (var command = connection.CreateCommand())
            {
                command.CommandTimeout = this.Configuration.CommandTimeout;
                foreach (string table in new[] { "SYSTRANSACTIONOBJECT", "SYSTRANSACTIONOBJECTERROR" })
                {
                    command.CommandText = $"TRUNCATE TABLE cisgkh.{table}";
                    await command.ExecuteNonQueryAsync();
                }
            }

            this.AddTrace("TruncateTemporaryListDataSetAsync End");
        }
        #endregion

        #region data
        private readonly static ConcurrentDictionary<Type, IEnumerable<string>> entityFields = new ConcurrentDictionary<Type, IEnumerable<string>>();

        private async Task PrepareTemporaryDataSetAsync(OracleConnection connection, Guid transactionGUID, SysOperationCode operationCode)
        {
            var procedures = this.Configuration[operationCode];
            if (procedures == null || procedures.PrepareProcedureName == null)
            {
                throw new Exception("Не задано имя процедуры для подготовки данных.");
            }

            this.AddTrace("PrepareTemporaryDataSetAsync Begin");

            var transactionGUIDParameter = new OracleParameter("TransactionGUID", OracleDbType.Char, transactionGUID.ToString(), ParameterDirection.Input);
            var transferDateParameter = new OracleParameter("TransferDate", OracleDbType.Date) { Direction = ParameterDirection.Output };
            //try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = this.Configuration.CommandTimeout;
                    command.CommandText = String.Format("BEGIN cisgkh.HCS.{0} (:TransactionGUID, :TransferDate); END;", procedures.PrepareProcedureName);
                    command.Parameters.Add(transactionGUIDParameter);
                    command.Parameters.Add(transferDateParameter);
                    await command.ExecuteNonQueryAsync();
                }
            }
            //catch (System.Data.SqlClient.SqlException e)
            //{
            //    throw new ExecSPException(procedures.PrepareProcedureName, e);
            //}
            //param.Date = (DateTime)sourceDateParameter.Value;

            this.AddTrace("PrepareTemporaryDataSetAsync End");
        }
        private async Task<IEnumerable<T>> GetTemporaryDataSetAsync<T>(OracleConnection connection, Guid transactionGUID)
            where T : class
        {
            return null;
            
            #region temp
            //this.AddTrace("GetTemporaryDataSetAsync Begin");

            //List<T> data = new List<T>();
            //IEnumerable<Type> entityRelationTypes = this.entityRelationBuilder.GetEntityRelation<T>()
            //    .GetRelations()
            //    .Select(r => r.Type)
            //    .Distinct()
            //    .ToList();
            //using (var context = HCSEdm.CreateContext(this.Configuration.HcsConnectionStringName))
            //{
            //    foreach (Type type in entityRelationTypes)
            //    {
            //        using (var command = connection.CreateCommand())
            //        {
            //            command.CommandText = String.Format("SELECT * FROM cisgkh.{0} WHERE TransactionGUID = :TransactionGUID", type.Name);
            //            command.Parameters.Add("TransactionGUID", OracleDbType.Char, transactionGUID.ToString(), ParameterDirection.Input);
            //            using (var reader = await command.ExecuteReaderAsync())
            //            {
            //                if (type == typeof(T))
            //                {
            //                    data = (await context.Set(type).TranslateAsync(reader))
            //                        .OfType<T>()
            //                        .ToList();
            //                    //data = context.ObjectContext
            //                    //    .Translate<T>(reader)
            //                    //    .ToList();
            //                }
            //                else
            //                {
            //                    var test = (await context.Set(type).TranslateAsync(reader))
            //                        .OfType<object>()
            //                        .ToList();
            //                    //var test = context.ObjectContext
            //                    //    .Translate(type, reader)
            //                    //    .OfType<object>()
            //                    //    .ToList();
            //                }
            //            }
            //        }
            //    }
            //}

            //this.AddTrace("GetTemporaryDataSetAsync End");

            //return data;
            #endregion
        }
        private async Task SetTemporaryDataSetAsync<T>(OracleConnection connection, IEnumerable<T> data)
            where T : class
        {
            return;

            this.AddTrace("SetTemporaryDataSetAsync Begin");

            #region temp
            //using (var context = HCSEdm.CreateContext(this.Configuration.HcsConnectionStringName))
            //{
            //    IEnumerable<Type> entityRelationTypes = this.entityRelationBuilder.GetEntityRelation<T>()
            //        .GetRelations()
            //        .Select(r => r.Type)
            //        .Distinct()
            //        .ToList();
            //    context.AddRange(data);
            //    foreach (Type type in entityRelationTypes)
            //    {
            //        //IEnumerable<string> fieldNames = entityFields.GetOrAdd(type, t =>
            //        //{
            //        //    context.Database.ExecuteSqlCommand("SELECT 1");
            //        //    IEnumerable<EntityType> entityTypes = context.ObjectContext
            //        //        .MetadataWorkspace
            //        //        .GetItemCollection(DataSpace.SSpace)
            //        //        .GetItems<EntityType>();
            //        //    EntityType entityType = entityTypes.FirstOrDefault(et => et.Name == type.Name);
            //        //    if (entityType == null)
            //        //    {
            //        //        throw new Exception(String.Format("Не найдена таблица [{0}]", type.Name));
            //        //    }

            //        //    return entityType.Properties
            //        //        .Where(p => p.Name != "uniqueId")
            //        //        .Select(p => p.Name)
            //        //        .ToList();
            //        //});

            //        var simpleProperties = OracleTypeUtil.GetSimpleProperties(type)
            //            .Where(p => p.EntityName != "uniqueId")
            //            .ToArray();
            //        string insert = String.Format("INSERT INTO cisgkh.{0} ({1})\nVALUES ({2})",
            //            type.Name,
            //            String.Join(",", simpleProperties.Select(f => f.DbName)),
            //            String.Join(",", simpleProperties.Select(f => ":" + f.DbName)));
            //        using (var command = connection.CreateCommand())
            //        {
            //            command.CommandText = insert;
            //            var entities = context.ChangeTracker
            //                .Entries()
            //                //.Where(ss => ss.State == EntityState.Added)
            //                .Where(ss => ss.Entity.GetType() == type)
            //                .Select(ss => ss.Entity);
            //            foreach (var entity in entities)
            //            {
            //                command.Parameters.Clear();
            //                foreach (var simpleProperty in simpleProperties)
            //                {
            //                    PropertyInfo property = simpleProperty.EntityProperty;
            //                    object value = property.GetValue(entity, null);
            //                    value = OracleTypeUtil.ToOracleValue(value);
            //                    command.Parameters.Add(simpleProperty.DbName, simpleProperty.DbType, value ?? DBNull.Value, ParameterDirection.Input);
            //                }
            //                //try
            //                //{
            //                await command.ExecuteNonQueryAsync();
            //                //}
            //                //catch (Exception e)
            //                //{
            //                //    this.AddTrace(insert);
            //                //    foreach (var simpleProperty in simpleProperties)
            //                //    {
            //                //        this.AddTrace(simpleProperty.ToString());
            //                //    }
            //                //    throw;
            //                //}
            //            }
            //        }
            //    }
            //}
            #endregion

            this.AddTrace("SetTemporaryDataSetAsync End");
        }
        private async Task ProcessTemporaryDataSetAsync(OracleConnection connection, Guid transactionGUID, SysOperationCode operationCode)
        {
            var procedures = this.Configuration[operationCode];
            if (procedures == null || procedures.ResultProcedureName == null)
            {
                throw new Exception("Не задано имя процедуры для обработки данных.");
            }

            this.AddTrace("ProcessTemporaryDataSetAsync Begin");

            var transactionGUIDParameter = new OracleParameter("TransactionGUID", OracleDbType.Char, transactionGUID.ToString(), ParameterDirection.Input);
            var transferDateParameter = new OracleParameter("TransferDate", OracleDbType.Date, DateTime.Now /*param.Date*/, ParameterDirection.Input);
            //try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = this.Configuration.CommandTimeout;
                    command.CommandText = String.Format("BEGIN cisgkh.HCS.{0} (:TransactionGUID, :TransferDate); END;", procedures.ResultProcedureName);
                    command.Parameters.Add(transactionGUIDParameter);
                    command.Parameters.Add(transferDateParameter);
                    await command.ExecuteNonQueryAsync();
                }
            }
            //catch (System.Data.SqlClient.SqlException e)
            //{
            //    throw new ExecSPException(procedures.ResultProcedureName, e);
            //}

            this.AddTrace("ProcessTemporaryDataSetAsync End");
        }
        private async Task TruncateTemporaryDataSetAsync<T>(OracleConnection connection, Guid transactionGUID)
            where T : class
        {
            this.AddTrace("TruncateTemporaryDataSetAsync Begin");

            #region temp
            //IEnumerable<Type> entityRelationTypes = this.entityRelationBuilder.GetEntityRelation<T>()
            //    .GetRelations()
            //    .Select(r => r.Type)
            //    .Distinct()
            //    .ToList();
            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandTimeout = this.Configuration.CommandTimeout;
            //    foreach (var entityType in entityRelationTypes)
            //    {
            //        command.CommandText = $"TRUNCATE TABLE cisgkh.{entityType.Name}";
            //        await command.ExecuteNonQueryAsync();
            //    }
            //}
            #endregion

            this.AddTrace("TruncateTemporaryDataSetAsync End");
        }
        #endregion
    }
}
