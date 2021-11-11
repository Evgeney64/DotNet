using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tsb.Controllers
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
        public async Task<string> Task1()
        {
            ViewNodel vm = new ViewNodel();
            vm.ValueStr = await task1();

            return vm.ValueStr;
        }

        public async Task<string> Task2()
        {
            ViewNodel vm = new ViewNodel();
            vm.ValueStr = await task2();

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
