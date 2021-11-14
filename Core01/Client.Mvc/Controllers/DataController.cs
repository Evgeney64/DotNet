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
        private DataConfiguration conf;
        public DataController()
        {
            conf = ConfigurateHelper.GetConfiguration("config.json", "EntityDataMsSql");
        }

        public ViewResult DoSmth()
        {
            VmBase vmBase = new VmBase(conf.ConnectionString);
            vmBase.DoSmth();
            return View("nsi_village", vmBase);
        }
        public ViewResult GetVillages()
        {
            VmBase vmBase = new VmBase(conf.ConnectionString);
            //List<VW_NSI_VILLAGE> items = vmBase.VwNsiVillages.ToList();
            return View("nsi_village", vmBase);
        }
        public ViewResult GetStreets()
        {
            VmBase vmBase = new VmBase(conf.ConnectionString);
            //List<NSI_STREET> list = vmBase.NsiStreetsL;
            return View("nsi_street", vmBase);
        }

    }
}
