using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using Server.Core.ViewModel;
using Server.Core.CoreModel;

namespace Home.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //[Authorize]
        //[AllowAnonymous]
        [Authorize(Roles = "admin, user")]
        public IActionResult Index()
        {
            if (this.HttpContext != null
                && this.Request != null && this.Response != null
                && this.RouteData != null
                && this.Url != null && this.User != null
                && this.Request.Query != null && this.Request.QueryString != null
                ) { }
            { }
            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Login", "Account");
                //return Content("не аутентифицирован");
            }

            return View(new VmBase(User));
        }

        [Authorize(Roles = "admin")]
        public IActionResult About()
        {
            VmBase vmBase = new VmBase(User)
            {
                Controller = "About",
            };
            return View(vmBase);
        }

        [Authorize(Policy = "OnlyForState_One")]
        public IActionResult Policy()
        {
            VmBase vmBase = new VmBase(User)
            {
                Controller = "Policy",
                Info = "state = 1",
            };
            return View("~/Views/Home/About.cshtml", vmBase);
        }

        [Authorize(Policy = "AgeLimit")]
        public IActionResult Policy18()
        {
            VmBase vmBase = new VmBase(User)
            {
                Controller = "Policy-18",
                Info = "age > 18",
            };
            return View("~/Views/Home/About.cshtml", vmBase);
        }
    }
}
