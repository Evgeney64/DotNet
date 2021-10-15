using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Hcs.Configuration;
using Hcs.DataSource;
using Hcs.Model;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<string> testing()
        {
            StoredProcDataSourceConfiguration conf = getDataSourceConfiguration("config.json");
            OracleStoredProdDataSource source = new OracleStoredProdDataSource(conf);

            string str = "";
            {
                //string str = await source.TestAsync();
                IEnumerable<ObjectInfo> objectInfos = await source.ListAsync(SysOperationCode.OrganizationExport);
                str += "<p>*** ObjectInfo --------------------------------------</p>";
                if (objectInfos != null)
                {
                    foreach (ObjectInfo item in objectInfos)
                    {
                        str += "<p> - " + item.Comment + "</p>";
                    }
                }

                Guid transactionGuid = Guid.NewGuid();
                IEnumerable<OrganizationExportRequest> items = await source.TakeDataAsync<OrganizationExportRequest>(transactionGuid, objectInfos);
                if (items != null)
                {
                    str += "<p>*** OrganizationExportRequest --------------------------------------</p>";
                    foreach (OrganizationExportRequest item in items)
                    {
                        str += "<p>      - " + item.uniqueId + " : " + item.TransactionGUID + " : " + item.TransportGUID + "</p>";
                        foreach (OrganizationExportRequestData item1 in item.OrganizationExportRequestData)
                        {
                            str += "<p>      - " + item1.uniqueId + " : " + item1.TransactionGUID + " : " + item1.TransportGUID + "</p>";
                        }
                    }
                }
            }
            return str;
        }

        #region Configuration
        private StoredProcDataSourceConfiguration getDataSourceConfiguration(string config_file)
        {
            IConfiguration configuration = getConfiguration("Hcs.ClientMvc", "Hcs.Sources.Oracle", config_file);

            //EntityDataSourceConfiguration conf1 = configuration.GetSection("EntityDataSourceConfiguration").Get<EntityDataSourceConfiguration>();
            StoredProcDataSourceConfiguration conf = new StoredProcDataSourceConfiguration();
            configuration.Bind("StoredProcDataSourceConfiguration", conf);

            return conf;
        }

        private IConfiguration getConfiguration(string client_path, string config_path, string config_file)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string conf_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + config_path + "\\";
            { }
            var builder = new ConfigurationBuilder()
                //.SetBasePath(conf_dir).AddJsonFile(config_file)
                .AddJsonFile(conf_dir + config_file)
                ;
            IConfiguration configuration = builder.Build();
            return configuration;
        }
        #endregion

    }
}
