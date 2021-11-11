using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Client.Mvc.Models;
using System.Linq;

using Server.Core;
using Server.Core.Public;
using Server.Core.Model;

namespace Data.Controllers
{
    public class DataController : Controller
    {
        private IConfiguration configuration { get; }
        public DataController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public ViewResult GetVillages0()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Data);
            return View("nsi_village", vmBase);
        }
        public ViewResult GetVillages()
        {
            DataConfiguration conf = ConfigurateHelper.GetConfiguration("config.json", "EntityDataMsSql");
            VmBase vmBase = new VmBase(conf.ConnectionString);

            //vmBase.DoSmth();
            //List<rgn> rgns = vmBase.rgns;
            //List<village> villages = vmBase.villages;
            //{ }
            return View("nsi_village", vmBase);
        }
        public ViewResult GetStreets()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Data);
            List<NSI_STREET> list = vmBase.NsiStreetsL;
            return View("nsi_street", vmBase);
        }

        public ViewResult GetGosStreets()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Gos);
            //List<type_street> type_streets = vmBase.type_streetsL;
            return View("type_street", vmBase);
            //return View(vmBase.type_streetsL);
        }
    }
}
