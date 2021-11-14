using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Tsb.Model
{
    public class Configuration
    {
        #region getDataSourceConfiguration
        public static DataSourceConfiguration GetDataSourceConfiguration(string client_path, string config_file, string name)
        {
            IConfiguration configuration = getConfiguration(client_path, client_path, config_file);
            if (configuration == null)
                return null;

            DataSourceConfiguration conf = new DataSourceConfiguration();
            configuration.Bind(name, conf);

            return conf;
        }

        private static IConfiguration getConfiguration(string client_path, string config_path, string config_file)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string conf_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + config_path + "\\";
            if (Directory.Exists(conf_dir) == false)
            {
                Console.WriteLine("Не найден каталог - [" + conf_dir + "]");
                return null;
            }
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
