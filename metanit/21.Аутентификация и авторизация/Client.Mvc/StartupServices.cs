using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Server.Core;
using System.IO;
using System.Diagnostics;


namespace ru.tsb.mvc
{
    public partial class Startup
    {
        private static void Index(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Index");
            });
        }
        private static void About(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("About");
            });
        }
    }
    #region 9.Контроллеры
    #region 01 - Передача зависимостей в контроллер
    public class ConfigurationClass
    {
        public ConnectionStringsClass ConnectionStrings { get; set; }
    }

    public class ConnectionStringsClass
    {
        public string DefaultConnection { get; set; }
        public string AuthConnection { get; set; }
    }
    #endregion
    #endregion

}
