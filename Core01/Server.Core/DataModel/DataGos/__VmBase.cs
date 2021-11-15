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
                List<Partners> pars = _serv.Get_Partners().Where(ss => ss.par_id <= 10).ToList();
                List<payerlive> plvs = _serv.Get_payerlive().Where(ss => ss.reciever_id <= 10).ToList();
                //List<rgn> rgns = _serv.Get_rgn().Where(ss => ss.rgn_id <= 10).ToList();
            }
        }
        #endregion
    }
}
