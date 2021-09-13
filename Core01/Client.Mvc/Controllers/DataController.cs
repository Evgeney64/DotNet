using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.CoreModel;

namespace Data.Controllers
{
    public class DataController : Controller
    {
        private IConfiguration configuration { get; }
        public DataController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public ViewResult GetVillages()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Data);
            return View("nsi_village", vmBase);
        }
        public ViewResult GetStreets()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Data);
            List<NSI_STREET> list = vmBase.NsiStreetsL;
            return View("nsi_street", vmBase);
        }
    }
}
