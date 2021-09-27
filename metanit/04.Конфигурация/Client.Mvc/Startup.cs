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

            #region Services
            #region 3.Dependency Injection
            #region 05 - DI (Создание своих сервисов)
            if (1 == 2)
            {
                services.AddTransient<IMessageSender, EmailMessageSender>();
            }
            #endregion

            #region 06 - DI (Расширения для добавления сервисов)
            if (1 == 2)
            {
                //services.AddTransient<TimeService>();
                services.AddTimeService();
            }
            #endregion

            #region 07 - DI (Передача зависимостей - Конструкторы)
            if (1 == 2)
            {
                services.AddTransient<IMessageSender, EmailMessageSender>();
                services.AddTransient<MessageService>();
            }
            #endregion

            #region 08 / 09
            if (1 == 1)
            {
                services.AddTransient<IMessageSender, EmailMessageSender>();
            }
            #endregion
            #endregion
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
        #region
            //, IMessageSender messageSender  // 05 - DI (Создание своих сервисов)
            //, TimeService timeService  // 06 - DI (Расширения для добавления сервисов)
            //, MessageService messageService // 07 - DI (Передача зависимостей - Конструкторы)
        #endregion

            )
        {
            #region Services
            // ************************************************************************
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 3.Dependency Injection
            #region 01 - UseDeveloperExceptionPage
            if (1 == 2)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.Run(async (context) =>
                {
                    int x = 0;
                    int y = 8 / x;
                    //y = 8 * x;
                    string text = getText("01 - UseDeveloperExceptionPage", $"Result = {y}");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 02 - UseExceptionHandler
            if (1 == 2)
            {
                env.EnvironmentName = "Production";
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/error");
                }
                app.Map("/error", ap => ap.Run(async context =>
                {
                    string text = getText("02 - UseExceptionHandler", $"DivideByZeroException occured !");
                    await context.Response.WriteAsync(text);
                }));
                app.Run(async (context) =>
                {
                    int x = 0;
                    int y = 8 / x;
                    //y = 8 * x;
                    string text = getText("02 - UseExceptionHandler", $"Result = {y}");
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 03 - StatusCodePagesMiddleware 
            if (1 == 2)
            {
                // http://localhost:58982/hello
                // http://localhost:58982/about
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                // обработка ошибок HTTP
                app.UseStatusCodePages();

                app.Map("/hello", ap => ap.Run(async (context) =>
                {
                    string text = getText("03 - StatusCodePagesMiddleware", $"/hello");
                    await context.Response.WriteAsync(text);
                }));
            }
            #endregion

            #region 04 - Dependency Injection
            if (1 == 2)
            {
                // Все сервисы
                app.Run(async context =>
                {
                    var sb = new StringBuilder();
                    sb.Append("<h1>Все сервисы</h1>");
                    sb.Append("<table>");
                    sb.Append("<tr><th>Тип</th><th>Lifetime</th><th>Реализация</th></tr>");
                    foreach (var svc in _services)
                    {
                        sb.Append("<tr>");
                        sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                        sb.Append($"<td>{svc.Lifetime}</td>");
                        sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("</table>");
                    context.Response.ContentType = "text/html;charset=utf-8";
                    await context.Response.WriteAsync(sb.ToString());
                });
            }
            #endregion

            #region 05 - DI (Создание своих сервисов)
            if (1 == 2)
            {
                //app.Run(async (context) =>
                //{
                //    context.Response.ContentType = "text/html; charset=utf-8";
                //    await context.Response.WriteAsync(messageSender.Send());
                //});
            }
            #endregion

            #region 06 - DI (Расширения для добавления сервисов)
            if (1 == 2)
            {
                //app.Run(async (context) =>
                //{
                //    context.Response.ContentType = "text/html; charset=utf-8";
                //    string text = getText("06 - DI (Расширения для добавления сервисов)", $"Текущее время: {timeService?.GetTime()}");
                //    await context.Response.WriteAsync(text);
                //});
            }
            #endregion

            #region 07 - DI (Передача зависимостей - Конструкторы)
            if (1 == 2)
            {
                //app.Run(async (context) =>
                //{
                //    context.Response.ContentType = "text/html; charset=utf-8";
                //    await context.Response.WriteAsync(messageService.Send());
                //});
            }
            #endregion

            #region 08 - DI (Передача зависимостей - HttpContext.RequestServices / ApplicationServices)
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    //IMessageSender messageSender = context.RequestServices.GetService<IMessageSender>();
                    IMessageSender messageSender = app.ApplicationServices.GetService<IMessageSender>();
                    { }
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(messageSender.Send());
                });
            }
            #endregion

            #region 09 - DI (Передача зависимостей - Invoke / InvokeAsync)
            if (1 == 1)
            {
                app.UseMiddleware<MessageMiddleware>();
            }
            #endregion
            #endregion
            // ************************************************************************
            #endregion
        }
    }
}
