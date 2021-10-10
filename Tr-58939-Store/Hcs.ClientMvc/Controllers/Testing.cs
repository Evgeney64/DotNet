using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Hcs.DataSources;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<Guid> testing()
        {
            EntityDataSourceConfiguration conf = getDataSourceConfiguration("config.json");
            EntityDataStoreNew store = new EntityDataStoreNew(conf);
            return Guid.NewGuid();

            TransactionInfo info = TransactionInfo.Create(SysOperationCode.AccountImport);

            Guid guid = await store.CreateTransactionAsync(info);
            return guid;
        }

        #region Configuration
        private EntityDataSourceConfiguration getDataSourceConfiguration(string config_file)
        {
            IConfiguration configuration = getConfiguration(config_file);

            //EntityDataSourceConfiguration conf1 = configuration.GetSection("EntityDataSourceConfiguration").Get<EntityDataSourceConfiguration>();
            EntityDataSourceConfiguration conf = new EntityDataSourceConfiguration();
            configuration.Bind("EntityDataSourceConfiguration", conf);
            return conf;
        }

        private IConfiguration getConfiguration(string config_file)
        {
            string base_dir = AppDomain.CurrentDomain.BaseDirectory;
            string conf_dir = base_dir.Substring(0, base_dir.IndexOf("Hcs.Client")) + "Hcs.Client\\";
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
