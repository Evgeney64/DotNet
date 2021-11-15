using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration;

using Server.Core.Public;
using Server.Core.Model;

namespace Server.Core
{
    public partial class VmBase
    {
        #region Data
        public void DoSmth()
        {
            using (EntityServ _serv = new EntityServ(connectionString))
            {
                List<Partners> items = _serv.Get_Partners().ToList();
                List<rgn> items1 = _serv.Get_rgn().ToList();
            }
        }
        #endregion
    }
}
