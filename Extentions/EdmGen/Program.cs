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
            HomeController item = new HomeController("EdmGen");
            
            //item.CreatePostgesScript();

            //item.CreateResultFile();
            
            //item.GenerateEdmClass();

            XlsxHelper.ReadXlsx();

            Console.WriteLine("");
            Console.WriteLine("Finish .......................................");
            //Console.ReadLine();
        }
    }
    public partial class HomeController
    {
        public string client_path;
        public HomeController(string _client_path)
        {
            client_path = _client_path;
        }

        public async Task<String> CreatePostgesScript()
        {
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration(client_path, "config.json", "MsSqlConfiguration");
            if (conf == null)
                return "Не найден файл конфигурации";

            DbInfo info = new DbInfo(conf);
            info.GenerateInfo();

            Postgres postgre = new Postgres(info);
            postgre.GeneratePostgresScript(client_path);
            return "";
        }

        public async Task<String> CreateResultFile()
        {
            ServiceResult res = EdmGenerator.CreateResultFile(client_path);
            return res.Error ? res.ErrorMessage : res.Message;
        }

        public async Task<String> GenerateEdmClass()
        {
            DataSourceConfiguration conf = Configuration.GetDataSourceConfiguration(client_path, "config.json", "MsSqlConfiguration");
            if (conf == null)
                return "Не найден файл конфигурации";

            ServiceResult res = EdmGenerator.GenerateEdmClass(conf, client_path);
            return res.Error ? res.ErrorMessage : res.Message;
        }

    }
}

