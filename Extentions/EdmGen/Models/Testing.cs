using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace Model
{
    public class Test
    {
        public String Execute()
        {
            DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "MsSqlConfiguration");

            Public serv = new Public(conf);
            String str = serv.GenerateScript();
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
