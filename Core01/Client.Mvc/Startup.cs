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
        public IConfiguration AppConfiguration { get; }
        public IConfiguration CustomConfiguration { get; set; }
        public Startup(IConfiguration configuration)
        {
            AppConfiguration = configuration;

            #region Services
            #region 4.Конфигурация
            #region 01 - Основы конфигурации
            if (1 == 2)
            {
                // строитель конфигурации
                var builder = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                    {"firstname", "Tom"},
                    {"age", "31"}
                    });
                // создаем конфигурацию
                CustomConfiguration = builder.Build();
            }
            #endregion

            #region 04 - Файловые провайдеры конфигурации (Конфигурация в JSON / XML / INI)
            if (1 == 2)
            {
                string path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"] + "config.json";
                var builder = new ConfigurationBuilder().AddJsonFile(path_conf);
                //string path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"] + "config.xml";
                //var builder = new ConfigurationBuilder().AddXmlFile(path_conf);
                //string path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"] + "config.ini";
                //var builder = new ConfigurationBuilder().AddIniFile(path_conf);

                CustomConfiguration = builder.Build();
            }
            #endregion

            #region 05 / 06 / 07 / 09 / 10
            if (1 == 2)
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

            #region 08 - Работа с конфигурацией (анализ файла конфигурации)
            if (1 == 2)
            {
                string path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"];

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(path_conf + "project.json")
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
            #region Default
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
            #endregion

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

            #region 4.Конфигурация
            #region 06 - Объединение источников конфигурации (UseMiddleware)
            if (1 == 2)
            {
                services.AddTransient<IConfiguration>(provider => CustomConfiguration);
            }
            #endregion

            #region 10 - Передача конфигурации через IOptions
            if (1 == 2)
            {
                services.Configure<ConfigurationClass>(CustomConfiguration);
                services.Configure<ConfigurationClass>(opt =>
                {
                    opt.color = "blue";
                });
            }
            #endregion
            #endregion

            #region 5.Состояние приложения
            #region 03 - Сессии
            if (1 == 2)
            {
                services.AddDistributedMemoryCache();
                //services.AddSession();
                services.AddSession(options =>
                {
                    options.Cookie.Name = ".MyApp.Session";
                    options.IdleTimeout = TimeSpan.FromSeconds(3600);
                    options.Cookie.IsEssential = true;
                });
            }
            #endregion

            #region 04 - Сессии (Хранение сложных объектов)
            if (1 == 2)
            {
                services.AddDistributedMemoryCache();
                services.AddSession();
            }
            #endregion
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
            #region Default
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

            #region 4.Конфигурация
            #region 01 - Основы конфигурации
            if (1 == 2)
            {
                #region 1
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(CustomConfiguration["firstname"]);
                });
                #endregion

                #region 2 - (мы можем динамически изменять уже имеющиеся настройки или определять новые)
                CustomConfiguration["firstname"] = "alice";
                CustomConfiguration["lastname"] = "simpson";
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync($"{CustomConfiguration["firstname"]} {CustomConfiguration["lastname"]} - {CustomConfiguration["age"]}");
                });
                #endregion
            }
            #endregion

            #region 02 - Конфигурация по умолчанию
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello world");
                });
            }
            #endregion

            #region 03 - Переменные среды окружения как источник конфигурации
            if (1 == 2)
            {
                string envVars = "";
                envVars += "JAVA_HOME - ";
                if (AppConfiguration["JAVA_HOME"] != null) envVars += AppConfiguration["JAVA_HOME"];
                else envVars += "not set";

                envVars += "\nTEMP - ";
                if (AppConfiguration["TEMP"] != null) envVars += AppConfiguration["TEMP"];
                else envVars += "not set";

                envVars += "\nProgram Files - ";
                if (AppConfiguration["ProgramFiles"] != null) envVars += AppConfiguration["ProgramFiles"];
                else envVars += "not set";

                { }
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(envVars);
                });
            }
            #endregion

            #region 04 - Файловые провайдеры конфигурации (Конфигурация в JSON / XML / INI)
            if (1 == 2)
            {
                var color = CustomConfiguration["color"];
                var text = CustomConfiguration["text"];
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync($"<p style='color:{color};'>{text}</p>");
                });
            }
            #endregion

            #region 05 - Объединение источников конфигурации
            if (1 == 2)
            {
                // определен в файле conf.json
                string color = CustomConfiguration["color"];

                // определен в памяти
                string name = CustomConfiguration["name"] + " - " + CustomConfiguration["age"];

                // определен в переменных среды окружения
                string path = "TEMP - " + CustomConfiguration["TEMP"];

                string text = $"<p style='color:{color};'>{name}</p>";
                text += $"<p>{path}</p>";
                { }
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 06 - Объединение источников конфигурации (UseMiddleware)
            if (1 == 2)
            {
                app.UseMiddleware<ConfigMiddleware>();
            }
            #endregion

            #region 07 - Работа с конфигурацией (GetSection)
            if (1 == 2)
            {
                string color = CustomConfiguration["color"];

                IConfigurationSection connStrings = CustomConfiguration.GetSection("ConnectionStrings");
                string def_con = connStrings.GetSection("DefaultConnection").Value;
                
                string tko_con = CustomConfiguration.GetSection("ConnectionStrings:TKOConnection").Value;
                string resk_con = CustomConfiguration["ConnectionStrings:RESKConnection"];
                string nesk_con = CustomConfiguration.GetConnectionString("NESKConnection");

                string text = "";
                text += $"<table><tr>";
                text += $"<td>default - </td><td style='color:{color};'>{def_con}</td>";
                text += $"</tr></table>";
                text += $"<p>RESK - {resk_con}</p>";
                text += $"<p>NESK - {nesk_con}</p>";
                { }
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 08 - Работа с конфигурацией (анализ файла конфигурации)
            if (1 == 2)
            {
                string projectJsonContent = getSectionContent(CustomConfiguration);
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("{\n" + projectJsonContent + "}");
                });
            }
            #endregion

            #region 09 - Проекция конфигурации на классы
            if (1 == 2)
            {
                ConfigurationClass conf = new ConfigurationClass();
                CustomConfiguration.Bind(conf);

                // Привязка секций конфигурации 
                ConnectionStringsClass conf_conn = CustomConfiguration
                    .GetSection("ConnectionStrings")
                    .Get<ConnectionStringsClass>()
                    ;

                string text = "";
                text += $"<table><tr>";
                text += $"<td>default - </td><td style='color:{conf.color};'>{conf.ConnectionStrings.DefaultConnection}</td>";
                text += $"</tr></table>";
                text += $"<p>TKO - {conf.ConnectionStrings.TKOConnection}</p>";
                text += $"<p>RESK - {conf_conn.RESKConnection}</p>";
                text += $"<p>NESK - {conf_conn.NESKConnection}</p>";
                { }
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 10 - Передача конфигурации через IOptions
            if (1 == 2)
            {
                app.UseMiddleware<ConfigurationClassMiddleware>();
            }
            #endregion
            #endregion

            #region 5.Состояние приложения
            #region 01 - HttpContext.Items
            if (1 == 2)
            {
                app.Use(async (context, next) =>
                {
                    context.Items["text1"] = "Text from HttpContext.Items";
                    context.Items.Add("text2", "Привет мир !!!");
                    await next.Invoke();
                });

                app.Run(async (context) =>
                {
                    string text = "";
                    if (context.Items.ContainsKey("text1")) text += $"<p>text1 - {context.Items["text1"]}</p>";
                    if (context.Items.ContainsKey("text2")) text += $"<p>text2 - {context.Items["text2"]}</p>";
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 02 - Куки
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    if (context.Request.Cookies.ContainsKey("name"))
                    {
                        // второй запрос
                        string name = context.Request.Cookies["name"];
                        await context.Response.WriteAsync($"Hello {name}!");
                    }
                    else
                    {
                        // первый запрос
                        context.Response.Cookies.Append("name", "Tom");
                        await context.Response.WriteAsync("Hello World!");
                    }
                });
            }
            #endregion

            #region 03 - Сессии
            if (1 == 2)
            {
                app.UseSession();

                app.Run(async (context) =>
                {
                    // второй запрос
                    if (context.Session.Keys.Contains("name"))
                        await context.Response.WriteAsync($"Hello {context.Session.GetString("name")}!");
                    else
                    {
                        // первый запрос
                        context.Session.SetString("name", "Tom");
                        await context.Response.WriteAsync("Hello World!");
                    }
                });
            }
            #endregion

            #region 04 - Сессии (Хранение сложных объектов)
            if (1 == 2)
            {
                app.UseSession();

                app.Run(async (context) =>
                {
                    if (context.Session.Keys.Contains("configuration"))
                    {
                        ConfigurationClass conf = context.Session.Get<ConfigurationClass>("configuration");
                        string text = "";
                        text += $"<table><tr>";
                        text += $"<td>default - </td><td style='color:{conf.color};'>{conf.ConnectionStrings.DefaultConnection}</td>";
                        text += $"</tr></table>";
                        text += $"<p>TKO - {conf.ConnectionStrings.TKOConnection}</p>";
                        text += $"<p>RESK - {conf.ConnectionStrings.RESKConnection}</p>";
                        text += $"<p>NESK - {conf.ConnectionStrings.NESKConnection}</p>";
                        { }

                        await context.Response.WriteAsync(text);
                    }
                    else
                    {
                        ConfigurationClass conf = new ConfigurationClass 
                        { 
                            color = "blue",
                            ConnectionStrings = new ConnectionStringsClass
                            {
                                DefaultConnection = "renowation_web",
                                TKOConnection = "renowation_web_tko",
                                RESKConnection = "renowation_web_resk",
                                NESKConnection = "renowation_web_nesk",
                            }
                        };
                        context.Session.Set<ConfigurationClass>("configuration", conf);
                        await context.Response.WriteAsync("Hello World!");
                    }
                });
            }
            #endregion
            #endregion
            // ************************************************************************
            #endregion
        }
    }
}
