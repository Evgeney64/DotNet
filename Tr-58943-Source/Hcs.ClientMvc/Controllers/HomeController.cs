using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<ActionResult<ViewNodel>> Test()
        {
            ViewNodel vm = new ViewNodel();
            vm.ValueStr = await testing();
            //vm.ValueStr = "await testing();";
            vm.ValueGuid = Guid.NewGuid();

            return View(vm);
        }
        public async Task<string> TestStr()
        {
            ViewNodel vm = new ViewNodel();
            vm.ValueStr = await testing();
            vm.ValueGuid = Guid.NewGuid();

            return vm.ValueStr;
        }
    }

    public class ViewNodel
    {
        public string ValueStr { get; set; }
        public Guid ValueGuid { get; set; }
        [JsonIgnore]
        public string ValueJson { get { return JsonSerializer.Serialize(this); } }
    }
}
