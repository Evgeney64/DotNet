using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Server.Core.Public;
using Server.Core.ViewModel;
//using Server.Core.AuthModel;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

using ru.tsb.mvc;
using Tsb.Security.Web.Models;

namespace Admin.Controllers 
{
	public class AdminController : Controller
	{
		private IConfiguration configuration { get; }
		public AdminController(IConfiguration _configuration)
		{
			configuration = _configuration;
		}

		#region OnActionExecute
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
			{ }
			base.OnActionExecuting(context);
		}
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
			{ }
			base.OnActionExecuted(context);
		}
		#endregion

		#region HttpGet
		public IActionResult GetUsers()
		{
			DataSourceConfiguration conf = getDataSourceConfiguration("config.json", "EntityDataAuthMsSql");
			using (SecurityContext context = SecurityContext.CreateContext(conf.ConnectionString))
			{
				//List<scr_principal> scr_principals = context.scr_principal.ToList();
				List<scr_user> scr_users = context.scr_user.ToList();
				{ }
				VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
				return View("sys_user", vmBase);
			}
		}
		public IActionResult GetUsers1()
		{
			if (this.HttpContext != null
				&& this.Request != null && this.Response != null
				&& this.RouteData != null
				&& this.Url != null && this.User != null
				&& this.Request.Query != null && this.Request.QueryString != null
				) { }

			#region 9.Контроллеры (01 - Передача зависимостей в контроллер)
			if (this.HttpContext.RequestServices != null)
			{
				ITimeService timeService = HttpContext.RequestServices.GetService<ITimeService>();
			}
			#endregion

			if (this.Request.Query.Keys.Count > 0)
			{
				// http://localhost:58982/Admin/GetUsers?altitude=20&height=4
				int i = 0;
				string text = $"<p><h3>RedirectToAction</h3></p>";
				foreach (string key in this.Request.Query.Keys)
				{
					string value = Request.Query.FirstOrDefault(p => p.Key == key).Value;
					text += $"<p>[{i}] {key} = {value}</p>";
					i++;
				}
				//return RedirectToAction("RedirectHtmlResult", "Admin", new { text = text });
				this.Response.WriteAsync(text);
				return null;
			}
			else
			{
				VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
				if (vmBase.UsersL != null)
				{ }
				return View("sys_user", vmBase);
			}
		}
		public IActionResult RedirectHtmlResult(string text)
        {
			return new HtmlResult(text);
		}

		public string GetUsersStr()
		{
			string text = "";
			if (this.Request.Query.Keys.Count > 0)
			{
				// http://localhost:58982/Admin/GetUsersStr?altitude=20&height=4
				int i = 0;
				foreach (string key in this.Request.Query.Keys)
				{
					string value = Request.Query.FirstOrDefault(p => p.Key == key).Value;
					text += "\n" + $"[{i}] {key} = {value}";
					i++;
				}
			}
			return text;
		}
		#endregion

		#region HttpPost
		[HttpPost]
		public string SetUser()
		{
			if (this.HttpContext != null
				&& this.Request != null && this.Response != null
				&& this.RouteData != null
				&& this.Url != null && this.User != null
				&& this.Request.Form != null
				) { }

			string text = "";
			if (this.Request.Form.Keys.Count > 0)
			{
				int i = 0;
				foreach(string key in this.Request.Form.Keys)
                {
					string value = Request.Form.FirstOrDefault(p => p.Key == key).Value;
					text += "\n" + $"[{i}] {key} = {value}";
					i++;
				}
			}

			//VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			return text;
		}
		#endregion

		#region getDataSourceConfiguration
		private DataSourceConfiguration getDataSourceConfiguration(string config_file, string name)
		{
			IConfiguration configuration = getConfiguration("Client.Mvc", "Client.Mvc", config_file);
			DataSourceConfiguration conf = new DataSourceConfiguration();
			configuration.Bind(name, conf);

			return conf;
		}

		private IConfiguration getConfiguration(string client_path, string config_path, string config_file)
		{
			string base_dir = AppDomain.CurrentDomain.BaseDirectory;
			string conf_dir = base_dir.Substring(0, base_dir.IndexOf(client_path)) + config_path + "\\";
			{ }
			var builder = new ConfigurationBuilder()
				//.SetBasePath(conf_dir).AddJsonFile(config_file)
				.AddJsonFile(conf_dir + config_file)
				;
			IConfiguration configuration = builder.Build();
			return configuration;
		}
		#endregion

	}

	public class HtmlResult : IActionResult
	{
		string htmlCode;
		public HtmlResult(string html)
		{
			htmlCode = html;
		}
		public async Task ExecuteResultAsync(ActionContext context)
		{
			string fullHtmlCode = "<!DOCTYPE html><html><head>";
			fullHtmlCode += "<title>Главная страница</title>";
			fullHtmlCode += "<meta charset=utf-8 />";
			fullHtmlCode += "</head> <body>";
			fullHtmlCode += htmlCode;
			fullHtmlCode += "</body></html>";
			await context.HttpContext.Response.WriteAsync(fullHtmlCode);
		}
	}

	public class DataSourceConfiguration
	{
		public string ConnectionString { get; set; }
		public int CommandTimeout { get; set; }
		public bool is_postgres { get; set; }
	}

}
