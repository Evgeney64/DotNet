using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Hcs.Store;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<String> testGenPostgr()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "MsSqlConfiguration");

            Public postgr = new Public(conf);
            String str = postgr.GenerateScript();
            return str;
        }

        private async Task<String> testRunPostgr()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "MsSqlConfiguration");

            Public postgr = new Public(conf);
            String str = postgr.GetData();
            return str;
        }

        private async Task<String> getContextSql()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "MsSqlConfiguration");

            Public postgr = new Public(conf);
            //String str = postgr.GetSysOperation();
            String str = postgr.GetSysTransaction();
            return str;
        }

        private async Task<String> getContextPostgres()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "PostgresConfiguration");

            Public postgr = new Public(conf);
            //String str = postgr.GetSysOperation();
            String str = postgr.GetSysTransaction();
            return str;
        }

        #region getDataSourceConfiguration
        private DataSourceConfiguration getDataSourceConfiguration(string config_file, string name)
        {
            IConfiguration configuration = getConfiguration("Hcs.ClientMvc", "Hcs.ClientMvc", config_file);
            DataSourceConfiguration conf = new DataSourceConfiguration();
            configuration.Bind(name, conf);

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
