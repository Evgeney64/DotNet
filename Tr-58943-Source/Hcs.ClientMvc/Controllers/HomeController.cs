using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Hcs.ClientMvc.Controllers
{
    public partial class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult<Guid>> Test()
        {
            String str = await testing();
            return View(Guid.NewGuid());
        }
        public ActionResult<String> TestStr()
        {
            var str = testing();
            return str.Result;
        }
        public async Task<ActionResult<String>> TestStr11()
        {
            String str = await testing();
            return View(str);
        }
    }
}
