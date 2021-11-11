using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Tsb.Model;
using Tsb;

namespace PostgresCons
{
    class Program
    {
        static void Main(string[] args)
        {
            HomeController item = new HomeController();
            item.CreatePostgesScript("PostgresCons");

            Console.WriteLine("-------------------------------------------");
            Console.ReadLine();
        }
    }
    public partial class HomeController
    {
        public async Task<String> CreatePostgesScript(string client_path)
        {
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration(client_path, "config.json", "MsSqlConfiguration");

            DbInfo info = new DbInfo(conf);
            info.GenerateInfo();
            info.GeneratePostgresScript(client_path);
            return "";
        }
    }
}
