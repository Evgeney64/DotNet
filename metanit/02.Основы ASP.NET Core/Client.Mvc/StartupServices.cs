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

}
