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

using System.Text;
using System.IO;
using System.Diagnostics;


namespace ru.tsb.mvc
{
    public partial class Startup
    {
        public IConfiguration AppConfiguration { get; }
        public IConfiguration CustomConfiguration { get; set; }
        public Startup(IConfiguration configuration)
        {
            AppConfiguration = configuration;
        }

        private IServiceCollection _services;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region Default
            _services = services;

            #endregion

            #region Services
            #region 19.WEB API
            #region 01 / 02
            if (1 == 1)
            {
                //services.AddControllers();
                services.AddMvc();
            }
            #endregion
            #endregion
            #endregion

            //services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            #region Services
            // ************************************************************************
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 19.WEB API
            #region 01 - Создание контроллера
            if (1 == 2)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers(); // подключаем маршрутизацию на контроллеры
                });
            }
            #endregion

            #region 02 - Создание клиента для WEB API
            if (1 == 1)
            {
                app.UseDeveloperExceptionPage();

                app.UseDefaultFiles();
                app.UseStaticFiles();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            #endregion
            #endregion
            // ************************************************************************
            #endregion
        }
    }
}
