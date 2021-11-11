using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Tsb.Model;
using Tsb.Generate;
using Tsb;

namespace EdmGen
{
    class Program
    {
        static void Main(string[] args)
        {
            HomeController item = new HomeController();
            
            item.CreatePostgesScript("EdmGen");

            //item.CreateResultFile();
            //item.GenerateEdmClass("EdmGen");

            Console.WriteLine("");
            Console.WriteLine("Finish .......................................");
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

            Postgres postgre = new Postgres(info.tables);
            postgre.GeneratePostgresScript(client_path);
            return "";
        }

        public async Task<String> CreateResultFile()
        {
            ServiceResult res = EdmGenerator.CreateResultFile();
            return res.Error ? res.ErrorMessage : res.Message;
        }

        public async Task<String> GenerateEdmClass(string client_path)
        {
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration(client_path, "config.json", "MsSqlConfiguration");

            ServiceResult res = EdmGenerator.GenerateEdmClass(conf);
            return res.Error ? res.ErrorMessage : res.Message;
        }

    }
}
