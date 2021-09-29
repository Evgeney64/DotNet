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

            #region 7.�������������
            #region 05 - �������� ������ ��������
            if (1 == 2)
            {
                services.AddRouting();
            }
            #endregion
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 7.�������������
            #region 01 - ������ ������������� � ASP.NET Core
            if (1 == 2)
            {
                app.Use(async (context, next) =>
                {
                    // �������� �������� �����
                    Endpoint endpoint = context.GetEndpoint();

                    if (endpoint != null)
                    {
                        // �������� ������ ��������, ������� ������������ � �������� ������
                        var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText;

                        Debug.WriteLine($"Endpoint Name: {endpoint.DisplayName}");
                        Debug.WriteLine($"Route Pattern: {routePattern}");

                        // ���� �������� ����� ����������, �������� ��������� ������
                        await next();
                    }
                    else
                    {
                        Debug.WriteLine("Endpoint: null");
                        // ���� �������� ����� �� ����������, ��������� ���������
                        // http://localhost:58982/About
                        string text = getText("01 - ������ ������������� � ASP.NET Core", "Endpoint is not defined", context);
                        await context.Response.WriteAsync(text);
                    }
                });
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/index", async context =>
                    {
                        // http://localhost:58982/index
                        string text = getText("01 - ������ ������������� � ASP.NET Core", "Hello Index !", context);
                        await context.Response.WriteAsync(text);
                    });
                    endpoints.MapGet("/", async context =>
                    {
                        // http://localhost:58982
                        string text = getText("01 - ������ ������������� � ASP.NET Core", "Hello World !", context);
                        await context.Response.WriteAsync(text);
                    });
                });
            }
            #endregion

            #region 02 - RouterMiddleware
            if (1 == 1)
            {
                // ���������� ���������� ��������
                var myRouteHandler = new RouteHandler(Handle);

                // ������� �������, ��������� ����������
                var routeBuilder = new RouteBuilder(app, myRouteHandler);

                // ���� ����������� �������� - �� ������ ��������������� ������� {controller}/{action}
                // http://localhost:58982/1
                routeBuilder.MapRoute("default", "{controller}/");
                // http://localhost:58982/1/2
                routeBuilder.MapRoute("two_param", "{controller}/{action}");
                // http://localhost:58982/1/2/3
                routeBuilder.MapRoute("three_param", "{controller}/{action}/{id}");

                // ������ �������
                app.UseRouter(routeBuilder.Build());

                app.Run(async (context) =>
                {
                    string text = getText("02 - RouterMiddleware", "Hello World !", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 03 - ����������� ���������
            if (1 == 2)
            {
                var routeBuilder = new RouteBuilder(app);

                // http://localhost:58982/1/2
                routeBuilder.MapRoute("{controller}/{action}",
                    async context => {
                        context.Response.ContentType = "text/html; charset=utf-8";
                        await context.Response.WriteAsync("�������������� ������");
                    });

                // http://localhost:58982/1/2/3
                routeBuilder.MapRoute("{controller}/{action}/{id}", Handle3);

                // ����������� ��������
                //routeBuilder.MapRoute("default", "store/{action}");

                // �������������� ���������
                //routeBuilder.MapRoute("default", "{controller}/{action?}/{id?}");

                // �������� ��� ���������� �� ���������
                //routeBuilder.MapRoute("default", "{controller}/{action}/{id?}", 
                //    new 
                //    { 
                //        controller = "home", 
                //        action = "index" 
                //    });
                //routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");

                // �������� ������������� ���������� ���������� � �������
                // http://localhost:58982/Home/Index/1/name/book/order
                //routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}/{*catchall}");

                // ������������� ���������
                //http://localhost:58982/RuHome/Index-en/1
                //routeBuilder.MapRoute("default", "Ru{controller=Home}/{action=Index}-en/{id?}");

                // ��������� ���������� � ��������
                //http://localhost:58982/Store/Order/lumia-2015
                //routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{name}-{year}");

                app.UseRouter(routeBuilder.Build());

                app.Run(async (context) =>
                {
                    string text = getText("03 - ����������� ���������", "Hello World !", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 04 - ������ � ����������
            if (1 == 2)
            {
                //http://localhost:58982/ind/lumia-2015
                var myRouteHandler = new RouteHandler(Handle4);

                var routeBuilder = new RouteBuilder(app, myRouteHandler);

                routeBuilder.MapRoute("default", "{action=Index}/{name}-{year}");

                app.UseRouter(routeBuilder.Build());

                app.Run(async (context) =>
                {
                    string text = getText("04 - ������ � ����������", "Hello World !", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 05 - �������� ������ ��������
            if (1 == 2)
            {
                // http://localhost:58982/admin/index
                var routeBuilder = new RouteBuilder(app);

                routeBuilder.Routes.Add(new AdminRoute());

                routeBuilder.MapRoute("{controller}/{action}",
                    async context => {
                        context.Response.ContentType = "text/html;charset=utf-8";
                        await context.Response.WriteAsync("�������������� ������");
                    });

                app.UseRouter(routeBuilder.Build());

                app.Run(async (context) =>
                {
                    string text = getText("05 - �������� ������ ��������", "Hello World !", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion
            #endregion
        }
    }
}
