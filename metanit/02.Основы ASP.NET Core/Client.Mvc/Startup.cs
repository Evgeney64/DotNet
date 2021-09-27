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
            _services = services;
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Services
            // ************************************************************************
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 2.������ ASP.NET Core
            #region 01 - Run()
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    string text = getText($"01 - Run()", $"Hello World !");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 02 - Use() + Run() (1)
            if (1 == 2)
            {
                int x = 5;
                int y = 8;
                int z = 0;
                app.Use(async (context, next) =>
                {
                    z = x * y;
                    await next.Invoke();
                });

                app.Run(async (context) =>
                {
                    string text = getText($"02 - Use() + Run() (1)", $"x * y = {z}");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 03 - () + Run() (2)
            if (1 == 2)
            {
                int x = 2;
                app.Use(async (context, next) =>
                {
                    x = x * 2;      // 2 * 2 = 4
                    await next.Invoke();    // ����� app.Run
                    x = x * 2;      // 8 * 2 = 16
                    string text = getText($"03 - () + Run() (2)", $"Result: {x}");
                    await context.Response.WriteAsync(text);
                });

                app.Run(async (context) =>
                {
                    x = x * 2;  //  4 * 2 = 8
                    await Task.FromResult(0);
                });
            }
            #endregion

            #region 04 - Map()
            if (1 == 2)
            {
                // http://localhost:58982/index
                // http://localhost:58982/Home/about
                app.Map("/index", Index);
                app.Map("/Home/about", About);
                //app.Map("/home", home =>
                //{
                //    home.Map("/index", Index);
                //    home.Map("/about", About);
                //});

                app.Run(async (context) =>
                {
                    string text = getText($"04 - Map()", $"Page Not Found");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 05 - TokenMiddleware
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=12345678
                app.UseMiddleware<TokenMiddleware>();

                app.Run(async (context) =>
                {
                    string text = getText($"05 - TokenMiddleware", $"Page Not Found");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 06 - TokenExtensions
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=12345678
                app.UseToken();

                app.Run(async (context) =>
                {
                    string text = getText($"06 - TokenExtensions", $"Page Not Found");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 07 - TokenParamExtensions
            if (1 == 2)
            {
                // http://localhost:58982/Home/about?token=555555
                app.UseTokenParam("555555");

                app.Run(async (context) =>
                {
                    string text = getText($"07 - TokenParamExtensions", $"Hello World");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 08 - Authentication
            if (1 == 2)
            {
                // http://localhost:58982/about
                // http://localhost:58982/about?token=555555
                app.UseMiddleware<AuthenticationMiddleware>();
                app.UseMiddleware<RoutingMiddleware>();
            }
            #endregion

            #region 09 - ErrorHandling
            if (1 == 2)
            {
                // http://localhost:58982/about
                // http://localhost:58982/about?token=555555
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.UseMiddleware<AuthenticationMiddleware>();
                app.UseMiddleware<RoutingMiddleware>();
            }
            #endregion

            #region 10 - IWebHostEnvironment
            if (1 == 2)
            {
                if (env.ApplicationName != null             // ��� ����������
                    && env.EnvironmentName != null          // �������� �����
                    && env.ContentRootPath != null          // ���� � �������� ����� ����������
                    && env.WebRootPath != null              // ���� � ����� wwwroot (����������� �������)
                    && env.ContentRootFileProvider != null  // ���������� ���������� Microsoft.AspNetCore.FileProviders.IFileProvider
                    && env.WebRootFileProvider != null      // ���������� ���������� Microsoft.AspNetCore.FileProviders.IFileProvider
                    )
                { }
            }
            #endregion
            #endregion
            // ************************************************************************
            #endregion
        }
    }
}
