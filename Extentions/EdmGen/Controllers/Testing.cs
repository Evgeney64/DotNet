using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Tsb.Model;
using Tsb.Generate;

namespace Tsb.Controllers
{
    public partial class HomeController : Controller
    {
        private async Task<String> task1()
        {
            DataSourceConfiguration conf = Configurator.GetDataSourceConfiguration("config.json", "MsSqlConfiguration");

            ServiceResult res = EdmGenerator.CreateResultFile();
            return res.Error ? res.ErrorMessage : res.Message;
        }

        private async Task<String> task2()
        {
            DataSourceConfiguration conf = Configurator.GetDataSourceConfiguration("config.json", "MsSqlConfiguration");

            DbInfo postgr = new DbInfo(conf);
            //String str = postgr.GetSysOperation();
            String str = "";
            return str;
        }

    }
}
