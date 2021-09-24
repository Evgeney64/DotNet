using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.AuthModel;

namespace Admin.Controllers 
{
	public class AdminController : Controller
	{
		private IConfiguration configuration { get; }
		public AdminController(IConfiguration _configuration)
		{
			configuration = _configuration;
		}

		public ViewResult GetUsers()
		{
			if (this.HttpContext != null 
				&& this.Request != null && this.Response != null 
				&& this.RouteData != null
				&& this.Url != null && this.User != null
				&& this.Request.Query != null
				//&& this.Request.Form != null
				&& this.Request.QueryString != null
				) { }

			if (1 == 2)
			{
				// http://localhost:58982/Admin/GetUsers?altitude=20&height=4
				string altitudeString = Request.Query.FirstOrDefault(p => p.Key == "altitude").Value;
				int altitude = Int32.Parse(altitudeString);

				string heightString = Request.Query.FirstOrDefault(p => p.Key == "height").Value;
				int height = Int32.Parse(heightString);
			}

			VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			return View("sys_user", vmBase);
		}

		#region HttpPost
		[HttpPost]
		public string SetUser()
		{
			if (this.HttpContext != null
				&& this.Request != null && this.Response != null
				&& this.RouteData != null
				&& this.Url != null && this.User != null
				&& this.Request.Query != null
				&& this.Request.Form != null
				&& this.Request.QueryString != null
				) { }

			string text = "";
			if (1 == 2)
			{
				// http://localhost:58982/Admin/SetUsers?altitude=20&height=4
				string altitudeString = Request.Form.FirstOrDefault(p => p.Key == "altitude").Value;
				int altitude = Int32.Parse(altitudeString);
				text += $"<p>altitude = {altitude}</p>";

				string heightString = Request.Form.FirstOrDefault(p => p.Key == "height").Value;
				int height = Int32.Parse(heightString);
				text += $"<p>height = {height}</p>";
			}

			//VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			return text;
		}

		[HttpPost]
		public string GetUsers(int? a)
		{
			// http://localhost:58982/Admin/Buy
			return "Спасибо  за покупку!";
		}
		#endregion
	}
}
