using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ru.tsb.mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {

            #region AppDomain
            if (1 == 2)
            {
                AppDomain domain = AppDomain.CurrentDomain;
                if (domain.FriendlyName != null
                    && domain.BaseDirectory != null)
                { }
                Assembly[] assemblies = domain.GetAssemblies();
            }
            #endregion

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
