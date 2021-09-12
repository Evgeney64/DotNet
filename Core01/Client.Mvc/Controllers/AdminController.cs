using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;


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
			VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			//vmBase.HtmlString = "<html lang=/"en/"//>";
			//return Html.Raw(vmBase.HtmlString);
			//vmBase.HtmlString = "<html lang='en'><body>" +
			//	"<script>$(document).ready(function() {	Site_Accordion('@Model.UsersJson');});</script></body></html>";
			//List<scr_user> list = vmBase.UsersL;
			return View("sys_user", vmBase);
		}
		public string GetUsers1()
		{
			VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
			//List<scr_user> list = vmBase.UsersL;
			//return View("sys_user", vmBase);

			vmBase.HtmlString = "<html lang='en'></html><body>" +
				"<script>$(document).ready(function() {	Site_Accordion('@Model.UsersJson');});</script></body></html>";

			return vmBase.HtmlString;
		}
	}
}

namespace ru.tsb.mvc.Factories
{
	public static partial class Configurator
    {
		public static object Customize(/*HtmlHelper Html, UrlHelper Url, */VmBase vmBase)
        {
			vmBase.HtmlString = @"<html lang='en'/>";
			return vmBase.Html.Raw(vmBase.HtmlString);
		}

	}
}