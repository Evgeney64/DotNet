using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Hcs.Configuration;
using Hcs.DataSource;
//using Hcs.Store;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<String> testing()
        {
            String str = "";
            //StoredProcDataSourceConfiguration conf = new StoredProcDataSourceConfiguration();
            string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=ebuled1-scan1.spb-dc.interrao.ru)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=DBCCB)));User Id=cisgkh;Password=cisgkh;";
            OracleStoredProdDataSource source = new OracleStoredProdDataSource(connectionString);
            await source.TestAsync();
            { }
            return str;
        }

        #region Configuration
        private EntityDataSourceConfiguration getDataSourceConfiguration(string config_file)
        {
            IConfiguration configuration = getConfiguration("Hcs.ClientMvc", "Hcs.Stores.EFCore", config_file);

            //EntityDataSourceConfiguration conf1 = configuration.GetSection("EntityDataSourceConfiguration").Get<EntityDataSourceConfiguration>();
            EntityDataSourceConfiguration conf = new EntityDataSourceConfiguration();
            configuration.Bind("EntityDataSourceConfiguration", conf);
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
