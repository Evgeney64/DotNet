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
                await context.Response.WriteAsync(getText($"Index", $""));
            });
        }
        private static void About(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync(getText($"About", $""));
            });
        }
        public static string GetText(string title, string value, HttpContext context = null) { return getText(title, value, context); }
        public static string getText(string title, string value, HttpContext context = null)
        {
            if (context != null)
                context.Response.ContentType = "text/html; charset=utf-8";

            string text = $"<p><h4>{title}</h4></p>";
            text += $"<h2><p style='color:red;'>{value}</p></h2>";
            return text;
        }

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

}
