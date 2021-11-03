using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Hcs.Configuration;
using Hcs.Stores;
//using Hcs.StoreP;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<String> testStore()
        {
            String str = "";
            EntityDataStoreConfiguration conf = getDataSourceConfiguration("config.json", "EntityDataStoreConfigurationMsSql");
            EntityDataStore store = new EntityDataStore(conf);
            TransactionInfo info = TransactionInfo.Create("111", SysOperationCode.AccountImport);

            //Guid guid = await store.CreateTransactionAsync(info);
            //str = guid.ToString();

            str = store.GetSysOperation();
            //str = store.OnDataStoreCreating();
            return str;
        }

        private async Task<String> testStoreP()
        {
            String str = "";
            EntityDataStoreConfiguration conf = getDataSourceConfiguration("config.json", "EntityDataStoreConfigurationPostgres");
            EntityDataStore store = new EntityDataStore(conf, true);
            TransactionInfo info = TransactionInfo.Create("111", SysOperationCode.AccountImport);

            //str = store.GetSysOperation();
            //str = store.GetSysTransaction();
            Guid guid = await store.CreateTransactionAsync(info);

            //Guid guid1 = Guid.Parse("424d2636-fe18-4c6b-a529-b19a816fbc58");
            //bool res = await store.IsTransactionExistsAsync(guid1);
            str = guid.ToString();

            return str;
        }


        #region Configuration
        private EntityDataStoreConfiguration getDataSourceConfiguration(string config_file, string section_name)
        {
            IConfiguration configuration = getConfiguration("Hcs.ClientMvc", "Hcs.Stores.EFCore", config_file);

            //EntityDataSourceConfiguration conf1 = configuration.GetSection("EntityDataSourceConfiguration").Get<EntityDataSourceConfiguration>();
            EntityDataStoreConfiguration conf = new EntityDataStoreConfiguration();
            configuration.Bind(section_name, conf);
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
