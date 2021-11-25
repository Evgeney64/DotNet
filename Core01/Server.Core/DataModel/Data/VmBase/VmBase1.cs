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
                List<BUILD> items = _serv.Get_BUILD().ToList();
                List<NSI_VILLAGE> items1 = _serv.Get_NSI_VILLAGE().ToList();
            }
        }

        public IEnumerable<VW_NSI_VILLAGE> VwNsiVillages
        {
            get
            {
                using (EntityServ _serv = new EntityServ(connectionString))
                {
                    //List<BUILD> itemsL = _serv.Get_BUILD().ToList();
                    { }
                    vwNsiVillages = _serv.Get_VW_NSI_VILLAGE().ToList();
                }
                //string query_sql = (vwNsiVillages as DbQuery<VW_NSI_VILLAGE>).ToString();
                //string query_sql1 = (vwNsiVillages as DbQuery<VW_NSI_VILLAGE>).Sql;
                return vwNsiVillages;
            }
        }
        private IEnumerable<VW_NSI_VILLAGE> vwNsiVillages;

        public IEnumerable<VW_NSI_STREET> VwNsiStreets
        {
            get
            {
                using (EntityServ _serv = new EntityServ(connectionString))
                {
                    List<NSI_VILLAGE> itemsL = _serv.Get_NSI_VILLAGE().ToList();
                    { }
                    vwNsiStreets = _serv.Get_VW_NSI_STREET().ToList();
                }
                return vwNsiStreets;
            }
        }
        private IEnumerable<VW_NSI_STREET> vwNsiStreets;
        #endregion

    }
}
