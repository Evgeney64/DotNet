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
            return Startup.GetText("05 - DI (Создание своих сервисов)", "Sent by Email");
        }
    }
    public class SmsMessageSender : IMessageSender
    {
        public string Send()
        {
            return Startup.GetText("05 - DI (Создание своих сервисов)", "Sent by SMS");
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
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(messageSender.Send());
        }
    }
    #endregion
    #endregion

}
