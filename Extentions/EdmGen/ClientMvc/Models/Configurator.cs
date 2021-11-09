using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Generate;


namespace Model
{
    public class Configurator
    {
        #region getDataSourceConfiguration
        public static DataSourceConfiguration GetDataSourceConfiguration(string config_file, string name)
        {
            IConfiguration configuration = getConfiguration("Hcs.ClientMvc", "Hcs.ClientMvc", config_file);
            DataSourceConfiguration conf = new DataSourceConfiguration();
            configuration.Bind(name, conf);

            return conf;
        }

        private static IConfiguration getConfiguration(string client_path, string config_path, string config_file)
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
