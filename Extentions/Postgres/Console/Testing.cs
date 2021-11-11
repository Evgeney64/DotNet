using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Hcs.Store
{
    public partial class HomeController
    {
        public async Task<String> GenPostgr()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "MsSqlConfiguration");

            Public postgr = new Public(conf);
            String str = postgr.GenerateScript();
            return str;
        }

        #region getDataSourceConfiguration
        private DataSourceConfiguration getDataSourceConfiguration(string config_file, string name)
        {
            IConfiguration configuration = getConfiguration("PostgresCons", "PostgresCons", config_file);
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

            IConfiguration configuration = null;
            try
            {
                configuration = builder.Build();
            }
            catch (Exception ex)
            { }
            return configuration;
        }
        #endregion

    }
}
