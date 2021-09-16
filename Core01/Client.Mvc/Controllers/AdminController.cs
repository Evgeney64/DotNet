using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
				&& this.Url != null && this.User != null) { }
			{ }

			VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			return View("sys_user", vmBase);
		}
	}
}
