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

        #region StartHandle
        private async Task StartHandle1(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var controller = routeValues["controller"].ToString();
            var action = routeValues["action"].ToString();
            var id = routeValues["id"].ToString();
            //await context.Response.WriteAsync($"controller: {controller} | action: {action}");
        }
        private async Task StartHandle2(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var controller = routeValues["controller"].ToString();
            var action = routeValues["action"].ToString();
            //await context.Response.WriteAsync($"controller: {controller} | action: {action}");
        }
        #endregion

        #region 4.Конфигурация
        #region 08 - Работа с конфигурацией (анализ файла конфигурации)
        public static string GetSectionContent(IConfiguration configSection)
        {
            string sectionContent = "";
            foreach (var section in configSection.GetChildren())
            {
                sectionContent += "\"" + section.Key + "\":";
                if (section.Value == null)
                {
                    string subSectionContent = GetSectionContent(section);
                    sectionContent += "{\n" + subSectionContent + "},\n";
                }
                else
                {
                    sectionContent += "\"" + section.Value + "\",\n";
                }
            }
            return sectionContent;
        }
        #endregion
        #endregion

        #region 6.Логгирование
        private void logging(HttpContext context
            //, ILogger<Startup> logger
            , ILogger logger
            )
        {
            logger.LogInformation("==================================================");
            // пишем на консоль информацию
            logger.LogInformation("--- LogCritical ----------------------");
            logger.LogCritical("{0}", context.Request.Path);
            logger.LogInformation("--- LogDebug ----------------------");
            logger.LogDebug("{0}", context.Request.Path);
            logger.LogInformation("--- LogError ----------------------");
            logger.LogError("{0}", context.Request.Path);
            logger.LogInformation("--- LogInformation ----------------------");
            logger.LogInformation("{0}", context.Request.Path);
            //logger.LogInformation($"Путь запроса {context.Request.Path}");
            logger.LogInformation("--- LogWarning ----------------------");
            logger.LogWarning("{0}", context.Request.Path);
        }
        #endregion

        #region 7.Маршрутизация
        #region 02 - RouterMiddleware
        private async Task Handle(HttpContext context)
        {
            // собственно обработчик маршрута
            await context.Response.WriteAsync("Hello ASP.NET Core!");
        }
        #endregion

        #region 03 - Определение маршрутов
        private async Task Handle3(HttpContext context)
        {
            // собственно обработчик маршрута
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync("трехсегментный запрос");
        }
        #endregion

        #region 04 - Работа с маршрутами
        private async Task Handle4(HttpContext context)
        {
            var routeValues = context.GetRouteData().Values;
            var action = routeValues["action"].ToString();
            var name = routeValues["name"].ToString();
            var year = routeValues["year"].ToString();
            await context.Response.WriteAsync($"action: {action} | name: {name} | year:{year}");
        }
        #endregion
        #endregion
    }
    #region Start
    #region StartMiddleware
    public class StartMiddleware
    {
        private readonly RequestDelegate _next;

        public StartMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            CustomConfiguration = config;
        }
        public IConfiguration CustomConfiguration { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            string[] paths = context.Request.Path.Value.Split($"/");
            if (paths[paths.Length - 1] == "about")
            {
                string color = CustomConfiguration["color"];
                string name = CustomConfiguration["name"] + " - " + CustomConfiguration["age"];
                string path = "TEMP - " + CustomConfiguration["TEMP"];
                string text = $"<p><h1>Middleware</h1></p>";
                text += $"<p style='color:{color};'>{name}</p>";
                text += $"<p>{path}</p>";
                await context.Response.WriteAsync(text);
            }
            else if (paths[paths.Length - 1] == "config")
            {
                string projectJsonContent = Startup.GetSectionContent(CustomConfiguration);
                await context.Response.WriteAsync(projectJsonContent);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
    #endregion

    #region StartEndpointMiddleware
    public class StartEndpointVerifyMiddleware
    {
        private readonly RequestDelegate _next;

        public StartEndpointVerifyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            CustomConfiguration = config;
        }
        public IConfiguration CustomConfiguration { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            Endpoint endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                // получаем шаблон маршрута, который ассоциирован с конечной точкой
                var routePattern = (endpoint as Microsoft.AspNetCore.Routing.RouteEndpoint)?.RoutePattern?.RawText;

                Debug.WriteLine($"Endpoint Name: {endpoint.DisplayName}");
                Debug.WriteLine($"Route Pattern: {routePattern}");

                // если конечная точка определена, передаем обработку дальше
                await _next.Invoke(context);
            }
            else
            {
                Debug.WriteLine("Endpoint: null");
                // если конечная точка не определена, завершаем обработку
                await context.Response.WriteAsync("Endpoint is not defined");
            }
        }
    }
    #endregion

    #region StartEndpointMiddleware1
    public class StartEndpointVerifyMiddleware1
    {
        private readonly RequestDelegate _next;

        public StartEndpointVerifyMiddleware1(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            CustomConfiguration = config;
        }
        public IConfiguration CustomConfiguration { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            Endpoint endpoint = context.GetEndpoint();

            //endpoints.MapGet("/index", async context =>
            //{
            //    await context.Response.WriteAsync("Hello Index!");
            //});
            //endpoints.MapGet("/", async context =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
    #endregion
    #endregion

    #region 2.Основы ASP.NET Core
    #region 06 - Token
    public static class TokenExtensions
    {
        public static IApplicationBuilder UseToken(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if (token != "12345678")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
    #endregion

    #region 07 - TokenParam
    public static class TokenParamExtensions
    {
        public static IApplicationBuilder UseTokenParam(this IApplicationBuilder builder, string pattern)
        {
            return builder.UseMiddleware<TokenParamMiddleware>(pattern);
        }
    }

    public class TokenParamMiddleware
    {
        private readonly RequestDelegate _next;
        string pattern;
        public TokenParamMiddleware(RequestDelegate next, string pattern)
        {
            this._next = next;
            this.pattern = pattern;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if (token != pattern)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
    #endregion

    #region 08 - Authentication
    public class AuthenticationMiddleware
    {
        private RequestDelegate _next;
        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = 403;
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }

    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        public RoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value.ToLower();
            if (path == "/index")
            {
                await context.Response.WriteAsync("Home Page");
            }
            else if (path == "/about")
            {
                await context.Response.WriteAsync("About");
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            //await _next.Invoke(context);
        }
    }
    #endregion

    #region 09 - ErrorHandling
    public class ErrorHandlingMiddleware
    {
        private RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await _next.Invoke(context);
            if (context.Response.StatusCode == 403)
            {
                await context.Response.WriteAsync("Access Denied");
            }
            else if (context.Response.StatusCode == 404)
            {
                await context.Response.WriteAsync("Not Found");
            }
        }
    }
    #endregion
    #endregion

    #region 3.Dependency Injection
    #region 05 - DI (Создание своих сервисов)
    public interface IMessageSender
    {
        string Send();
    }
    public class EmailMessageSender : IMessageSender
    {
        public string Send()
        {
            return "Sent by Email";
        }
    }
    public class SmsMessageSender : IMessageSender
    {
        public string Send()
        {
            return "Sent by SMS";
        }
    }
    #endregion

    #region 06 - DI (Расширения для добавления сервисов)
    public class TimeService
    {
        public string GetTime() => System.DateTime.Now.ToString("hh:mm:ss");
    }
    public static class ServiceProviderExtensions
    {
        public static void AddTimeService(this IServiceCollection services)
        {
            services.AddTransient<TimeService>();
        }
    }
    #endregion

    #region 07 - DI (Передача зависимостей - Конструкторы)
    public class MessageService
    {
        IMessageSender _sender;
        public MessageService(IMessageSender sender)
        {
            _sender = sender;
        }
        public string Send()
        {
            return _sender.Send();
        }
    }
    #endregion

    #region 09 - DI (Передача зависимостей - Invoke / InvokeAsync)
    public class MessageMiddleware
    {
        private readonly RequestDelegate _next;

        public MessageMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context, IMessageSender messageSender)
        {
            context.Response.ContentType = "text/html;charset=utf-8";
            await context.Response.WriteAsync(messageSender.Send());
        }
    }
    #endregion
    #endregion

    #region 4.Конфигурация
    #region 06 - Объединение источников конфигурации (UseMiddleware)
    public class ConfigMiddleware
    {
        private readonly RequestDelegate _next;

        public ConfigMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            CustomConfiguration = config;
        }
        public IConfiguration CustomConfiguration { get; set; }

        public async Task InvokeAsync(HttpContext context)
        {
            // определен в файле conf.json
            string color = CustomConfiguration["color"];

            // определен в памяти
            string name = CustomConfiguration["name"] + " - " + CustomConfiguration["age"];

            // определен в переменных среды окружения
            string path = "TEMP - " + CustomConfiguration["TEMP"];

            string text = $"<p><h1>Middleware</h1></p>";
            text += $"<p style='color:{color};'>{name}</p>";
            text += $"<p>{path}</p>";
            { }

            await context.Response.WriteAsync(text);
        }
    }
    #endregion

    #region 09 - Проекция конфигурации на классы
    public class ConfigurationClass
    {
        public string color { get; set; }
        public string text { get; set; }
        public ConnectionStringsClass ConnectionStrings { get; set; }
        public List<string> Languages { get; set; }
        public ConfigurationCompanyClass Company { get; set; }
    }
    public class ConfigurationCompanyClass
    {
        public string Title { get; set; }
        public string Country { get; set; }
    }
    public class ConnectionStringsClass
    {
        public string DefaultConnection { get; set; }
        public string TKOConnection { get; set; }
        public string RESKConnection { get; set; }
        public string NESKConnection { get; set; }
    }
    #endregion

    #region 10 - Передача конфигурации через IOptions
    public class ConfigurationClassMiddleware
    {
        private readonly RequestDelegate _next;

        public ConfigurationClassMiddleware(RequestDelegate next, IOptions<ConfigurationClass> options)
        {
            _next = next;
            conf = options.Value;
        }

        public ConfigurationClass conf { get; }

        public async Task InvokeAsync(HttpContext context)
        {
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
    }
    #endregion
    #endregion

    #region 5.Состояние приложения
    #region 04 - Сессии (Хранение сложных объектов)
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
    #endregion
    #endregion

    #region 6.Логгирование
    #region 03 - Создание провайдера логгирования
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();
        public FileLogger(string path)
        {
            filePath = path;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //return logLevel == LogLevel.Trace;
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    File.AppendAllText(filePath, formatter(state, exception) + Environment.NewLine);
                }
            }
        }
    }

    public class FileLoggerProvider : ILoggerProvider
    {
        private string path;
        public FileLoggerProvider(string _path)
        {
            path = _path;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }

        public void Dispose()
        {
        }
    }

    public static class FileLoggerExtensions
    {
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
        {
            factory.AddProvider(new FileLoggerProvider(filePath));
            return factory;
        }
    }
    #endregion
    #endregion

    #region 7.Маршрутизация
    #region 05 - Создание своего маршрута
    public class AdminRoute : IRouter
    {
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            throw new NotImplementedException();
        }

        public async Task RouteAsync(RouteContext context)
        {
            string url = context.HttpContext.Request.Path.Value.TrimEnd('/');
            if (url.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Handler = async ctx =>
                {
                    ctx.Response.ContentType = "text/html;charset=utf-8";
                    await ctx.Response.WriteAsync("Привет admin!");
                };
            }
            await Task.CompletedTask;
        }
    }
    #endregion
    #endregion

    #region 9.Контроллеры
    #region 01 - Контроллеры и их действия
    #endregion
    #endregion

}
