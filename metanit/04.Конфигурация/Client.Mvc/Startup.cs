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

            #region 4.������������
            #region 04 - �������� ���������� ������������ (������������ � JSON / XML / INI)
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

            #region Start 01 / 05 / 06 / 07 / 09 / 10
            if (1 == 1)
            {
                string path_conf = "";
                path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"]; ;

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
                // ������� ������������
                CustomConfiguration = builder.Build();
            }
            #endregion

            #region 08 - ������ � ������������� (������ ����� ������������)
            if (1 == 2)
            {
                string path_conf = AppConfiguration["ASPNETCORE_IIS_PHYSICAL_PATH"];

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(path_conf + "project.json")
                    ;
                // ������� ������������

                CustomConfiguration = builder.Build();
            }
            #endregion
            #endregion
        }

        private IServiceCollection _services;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _services = services;
            services.AddMvc();

            #region 4.������������
            #region 06 - ����������� ���������� ������������ (UseMiddleware)
            if (1 == 2)
            {
                services.AddTransient<IConfiguration>(provider => CustomConfiguration);
            }
            #endregion

            #region 10 - �������� ������������ ����� IOptions
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // https://metanit.com/sharp/aspnet5/1.1.php
            #region 4.������������
            #region 01 - ������ ������������
            if (1 == 2)
            {
                #region 1
                if (1 == 2)
                {
                    app.Run(async (context) =>
                    {
                        string text = getText("01 - ������ ������������", CustomConfiguration["name"], context);
                        await context.Response.WriteAsync(text);
                    });
                }
                #endregion

                #region 2 - (�� ����� ����������� �������� ��� ��������� ��������� ��� ���������� �����)
                if (1 == 2)
                {
                    CustomConfiguration["name"] = "�����";
                    CustomConfiguration["lastname"] = "�����";
                    CustomConfiguration["age"] = "6.5";
                    app.Run(async (context) =>
                    {
                        string text1 = $"{CustomConfiguration["name"]} {CustomConfiguration["lastname"]} - {CustomConfiguration["age"]}";
                        string text = getText("01 - ������ ������������", text1, context);
                        await context.Response.WriteAsync(text);
                    });
                }
                #endregion
            }
            #endregion

            #region 02 - ������������ �� ���������
            if (1 == 2)
            {
                app.Run(async (context) =>
                {
                    string text = getText("02 - ������������ �� ���������", "Hello world", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 03 - ���������� ����� ��������� ��� �������� ������������
            if (1 == 2)
            {
                string envVars = "";
                envVars += $"<p>";
                envVars += $"JAVA_HOME - ";
                if (AppConfiguration["JAVA_HOME"] != null) envVars += AppConfiguration["JAVA_HOME"];
                else envVars += "not set";
                envVars += $"</p>";

                envVars += $"<p>";
                envVars += $"\nTEMP - ";
                if (AppConfiguration["TEMP"] != null) envVars += AppConfiguration["TEMP"];
                else envVars += "not set";
                envVars += $"</p>";

                envVars += $"<p>";
                envVars += $"\nProgram Files - ";
                if (AppConfiguration["ProgramFiles"] != null) envVars += AppConfiguration["ProgramFiles"];
                else envVars += "not set";
                envVars += $"</p>";

                { }
                app.Run(async (context) =>
                {
                    string text = getText("03 - ���������� ����� ��������� ��� �������� ������������", envVars, context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 04 - �������� ���������� ������������ (������������ � JSON / XML / INI)
            if (1 == 2)
            {
                var color = CustomConfiguration["color"];
                var text1 = CustomConfiguration["text"];
                app.Run(async (context) =>
                {
                    string text = getText("04 - �������� ���������� ������������ (������������ � JSON / XML / INI)", $"<p style='color:{color};'>{text1}</p>", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 05 - ����������� ���������� ������������
            if (1 == 2)
            {
                // ��������� � ����� conf.json
                string color = CustomConfiguration["color"];

                // ��������� � ������
                string name = CustomConfiguration["name"] + " - " + CustomConfiguration["age"];

                // ��������� � ���������� ����� ���������
                string path = "TEMP - " + CustomConfiguration["TEMP"];

                string text1 = $"<p style='color:{color};'>{name}</p>";
                text1 += $"<p>{path}</p>";
                { }
                app.Run(async (context) =>
                {
                    string text = getText("05 - ����������� ���������� ������������", $"<p style='color:{color};'>{text1}</p>", context);
                    await context.Response.WriteAsync(text);
                });
            }
            #endregion

            #region 06 - ����������� ���������� ������������ (UseMiddleware)
            if (1 == 2)
            {
                app.UseMiddleware<ConfigMiddleware>();
            }
            #endregion

            #region 07 - ������ � ������������� (GetSection)
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
                    string text1 = getText("07 - ������ � ������������� (GetSection)", $"<p style='color:{color};'>{text}</p>", context);
                    await context.Response.WriteAsync(text1);
                });
            }
            #endregion

            #region 08 - ������ � ������������� (������ ����� ������������)
            if (1 == 2)
            {
                string projectJsonContent = GetSectionContent(CustomConfiguration);
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("{\n" + projectJsonContent + "}");
                });
            }
            #endregion

            #region 09 - �������� ������������ �� ������
            if (1 == 1)
            {
                ConfigurationClass conf = new ConfigurationClass();
                CustomConfiguration.Bind(conf);

                // �������� ������ ������������ 
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
                    string text1 = getText("09 - �������� ������������ �� ������", text, context);
                    await context.Response.WriteAsync(text1);
                });
            }
            #endregion

            #region 10 - �������� ������������ ����� IOptions
            if (1 == 2)
            {
                app.UseMiddleware<ConfigurationClassMiddleware>();
            }
            #endregion
            #endregion
        }
    }
}
