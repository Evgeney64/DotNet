using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.EntityFrameworkCore;

using Server.Core;
using System.Text;

namespace ru.tsb.mvc
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private IServiceCollection _services;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            //services.AddDbContext<EntityContext>(options =>
            //    options.UseSqlServer(Configuration["Data:renovation_web:ConnectionString"]));
            //services.AddTransient<ICoreEdm, CoreEdm>();

            services.AddControllersWithViews();

            //services.AddDbContext<AppidentityDbContext>(options => 
            //    options.UseSqlServer(Configuration["Data:auth:ConnectionString"]));
            
            //services.AddIdentity<scr_user, IdentityRole>()
            //    .AddEntityFrameworkStores<AppidentityDbContext>()
            //    .AddDefaultTokenProviders();

            services.AddMvc();

            #region Services
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

            #region 08 - DI (Передача зависимостей - HttpContext.RequestServices / ApplicationServices)
            if (1 == 2)
            {
                services.AddTransient<IMessageSender, EmailMessageSender>();
            }
            #endregion

            #region 09 - DI (Передача зависимостей - Invoke / InvokeAsync)
            if (1 == 2)
            {
                services.AddTransient<IMessageSender, EmailMessageSender>();
            }
            #endregion
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
        #region
            //, IMessageSender messageSender  // 15 - DI (Создание своих сервисов)
            //, TimeService timeService  // 16 - DI (Расширения для добавления сервисов)
            //, MessageService messageService // 17 - DI (Передача зависимостей - Конструкторы)
        #endregion
            )
        {
            #region Common
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            #endregion

            #region Services
            // ************************************************************************
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 2.Основы ASP.NET Core
            #region 01 - Run()
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World !");
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
                    await context.Response.WriteAsync($"x * y = {z}");
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
                    await next.Invoke();    // вызов app.Run
                    x = x * 2;      // 8 * 2 = 16
                    await context.Response.WriteAsync($"Result: {x}");
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
                app.Map("/index", Index);
                app.Map("/Home/about", About);
                //app.Map("/home", home =>
                //{
                //    home.Map("/index", Index);
                //    home.Map("/about", About);
                //});

                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Page Not Found");
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
                    await context.Response.WriteAsync("Page Not Found");
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
                    await context.Response.WriteAsync("Page Not Found");
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
                    await context.Response.WriteAsync("Hello World");
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
                if (env.ApplicationName != null             // имя приложения
                    && env.EnvironmentName != null          // описание среды
                    && env.ContentRootPath != null          // путь к корневой папке приложения
                    && env.WebRootPath != null              // путь к папке wwwroot (статический контент)
                    && env.ContentRootFileProvider != null  // реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider
                    && env.WebRootFileProvider != null      // реализацию интерфейса Microsoft.AspNetCore.FileProviders.IFileProvider
                    )
                { }
            }
            #endregion
            #endregion

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
                    await context.Response.WriteAsync($"Result = {y}");
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
                    await context.Response.WriteAsync("DivideByZeroException occured!");
                }));
                app.Run(async (context) =>
                {
                    int x = 0;
                    int y = 8 / x;
                    await context.Response.WriteAsync($"Result = {y}");
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
                    await context.Response.WriteAsync($"Hello ASP.NET Core");
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
                //    await context.Response.WriteAsync($"Текущее время: {timeService?.GetTime()}");
                //});
            }
            #endregion

            #region 07 - DI (Передача зависимостей - Конструкторы)
            if (1 == 2)
            {
                //app.Run(async (context) =>
                //{
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
                    context.Response.ContentType = "text/html;charset=utf-8";
                    await context.Response.WriteAsync(messageSender.Send());
                });
            }
            #endregion

            #region 09 - DI (Передача зависимостей - Invoke / InvokeAsync)
            if (1 == 2)
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
