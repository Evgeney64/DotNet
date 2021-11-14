using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using ru.tsb.mvc;
using Tsb.Security.Web.Models;
using Client.Mvc.Models;
using Server.Core;
using Server.Core.Public;

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

}
