using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Hcs.Model;

namespace Hcs.DataSource
{
    public partial class OracleStoredProdDataSource : IDataSource2, IDisposable, ILoggable
    {
        public async Task TestAsync0()
        {
            if (entityRelationBuilder.entities != null && entityRelationBuilder.EntityRelations != null) ;
            { }
            IEnumerable<ObjectInfo> result = await ListAsync(SysOperationCode.OrganizationExport);
            { }
        }

        public async Task TestAsync1()
        {
            using (var connection = this.CreateConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = 300;
                    command.InitialLOBFetchSize = -1;
                    command.CommandText = "SELECT * FROM HSC_IDS_MAP";
                    await connection.OpenAsync();

                    int count = 0;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string col0 = await reader.GetFieldValueAsync<string>(0);
                            string col1 = await reader.GetFieldValueAsync<string>(1);
                            string col2 = await reader.GetFieldValueAsync<string>(2);
                            { }
                            //                //    var objectInfoError = new ObjectInfoError
                            //                //    {
                            //                //        ObjectId = await reader.GetFieldValueAsync<string>(0),
                            //                //        ErrorCode = await reader.GetFieldValueAsync<string>(1),
                            //                //        ErrorDescription = await reader.GetFieldValueAsync<string>(2),
                            //                //    };
                            count++;
                        }
                    }
                }
            }
            return;
        }

        public async Task<string> TestAsync()
        {
            Guid transactionGuid = Guid.NewGuid();
            IEnumerable<ObjectInfo> objectInfos = await ListAsync(SysOperationCode.OrganizationExport);
            IEnumerable<HouseImportRequest> items = await TakeDataAsync<HouseImportRequest>(transactionGuid, objectInfos);
            string str = "";
            foreach (ObjectInfo item in objectInfos)
            {
                str += "<p> - " + item.Comment + "</p>";
            }
            return str;
        }
    }
}
