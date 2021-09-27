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
        public static string GetText(string title, string value) { return getText(title, value); }
        public static string getText(string title, string value)
        {
            string text = $"<p><h4>{title}</h4></p>";
            text += $"<h2><p style='color:red;'>{value}</p></h2>";
            return text;

        }
    }

    #region 2.Основы ASP.NET Core
    #region 05 / 06 - Token
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
                string text = Startup.GetText($"05/06 - TokenMiddleware", $"Token is invalid");
                await context.Response.WriteAsync(text);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
    #endregion

    #region 07 - TokenParamExtensions
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
                string text = Startup.GetText($"07 - TokenParamExtensions", $"Token is invalid");
                await context.Response.WriteAsync(text);
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
                string text = Startup.GetText($"08 - Authentication", $"/index");
                await context.Response.WriteAsync(text);
            }
            else if (path == "/about")
            {
                string text = Startup.GetText($"08 - Authentication", $"/about");
                await context.Response.WriteAsync(text);
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
                string text = Startup.GetText($"09 - ErrorHandling", $"Access Denied");
                await context.Response.WriteAsync(text);
            }
            else if (context.Response.StatusCode == 404)
            {
                string text = Startup.GetText($"09 - ErrorHandling", $"Not Found");
                await context.Response.WriteAsync(text);
            }
        }
    }
    #endregion
    #endregion

}
