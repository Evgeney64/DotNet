using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

using Server.Core;
using System.Text;
using System.IO;
using System.Diagnostics;

using ru.tsb.mvc.Models.Users;
using ServiceLib;
using Server.Core.CoreModel;
using Server.Core.AuthModel;

namespace ru.tsb.mvc
{
    public partial class Startup
    {
        public IConfiguration AppConfiguration { get; }
        public IConfiguration CustomConfiguration { get; set; }
        public Startup(IConfiguration configuration)
        {
            AppConfiguration = configuration;

            #region Services
            #region 4.Конфигурация
            #region Start / 05 / 06 / 07 / 09 / 10
            if (1 == 1)
            {
                string path_conf = "";
                //path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"]; ;

                var builder = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile(path_conf + "config.json")
                    .AddEnvironmentVariables()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"name", "Tom"},
                        {"age", "31"}
                    })
                    .AddConfiguration(configuration)
                    ;
                // создаем конфигурацию

                CustomConfiguration = builder.Build();
            }
            #endregion
            #endregion
            #endregion
        }

        private IServiceCollection _services;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            services.Configure<ConfigurationClass>(CustomConfiguration);

            #region Services
            #region 21.Аутентификация и авторизация
            #region 01 - Аутентификация на основе куки
            if (1 == 1)
            {
                // установка конфигурации подключения
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => //CookieAuthenticationOptions
                    {
                        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    });
                services.AddControllersWithViews();
            }
            #endregion
            #endregion
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Services
            // ************************************************************************
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 21.Аутентификация и авторизация
            #region 01 - Аутентификация на основе куки
            if (1 == 1)
            {
                app.UseDeveloperExceptionPage();

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();    // аутентификация
                app.UseAuthorization();     // авторизация

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            }
            #endregion
            #endregion
            // ************************************************************************
            #endregion
        }
    }
}
