using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
			List<scr_user> list = vmBase.UsersL;
			return View("sys_user", vmBase);
		}
	}
}