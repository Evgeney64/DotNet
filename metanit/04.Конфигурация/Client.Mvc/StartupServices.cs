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
    }

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
            string text1 = Startup.GetText("06 - Объединение источников конфигурации (UseMiddleware)", $"<p style='color:{color};'>{text}</p>", context);
            await context.Response.WriteAsync(text1);
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

            string text1 = Startup.GetText("10 - Передача конфигурации через IOptions", text, context);
            await context.Response.WriteAsync(text1);
        }
    }
    #endregion
    #endregion

}
