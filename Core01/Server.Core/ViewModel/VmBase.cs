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
        #region Define
        public VmBase()
        { }
        public VmBase(string _connectionString)
        {
            connectionString = _connectionString;
        }
        private string connectionString;
        public string ConnectionString { get { return connectionString; } }
        public string HtmlString { get; set; }
        public HtmlHelper Html { get; set; }
        private EntityServ serv { get; }
        public VmBase(IConfiguration configuration, ConnectionType_Enum connectionType)
        {
            
            //switch (connectionType)
            //{
            //    case ConnectionType_Enum.Auth:
            //        connectionString = configuration["Data:auth:ConnectionString"];
            //        //authServ = new AuthServ(connectionString);
            //        break;
            //    case ConnectionType_Enum.Data:
            //        connectionString = configuration["Data:renovation_web:ConnectionString"];
            //        coreServ = new CoreServ(connectionString);
            //        break;
            //    case ConnectionType_Enum.Gos:
            //        connectionString = configuration["Data:gos:ConnectionString"];
            //        coreServ = new CoreServ(connectionString);
            //        break;
            //}
            //var contextOptions = new DbContextOptionsBuilder<EntityContext>()
            //    .UseSqlServer(connectionString)
            //    .Options;

            //context = new CoreEdm(contextOptions);
        }
        //private CoreEdm context { get; }
        //private AuthServ authServ { get; }
        #endregion

        #region Data
        //public IEnumerable<NSI_STREET> NsiStreets => context.NSI_STREET;
        public IEnumerable<NSI_STREET> NsiStreets => serv.Get_NSI_STREET();
        public List<NSI_STREET> NsiStreetsL => NsiStreets.ToList();
        //public IEnumerable<VW_NSI_STREET> VwNsiStreets => serv.Get_VW_NSI_STREET();
        public List<VW_NSI_STREET> VwNsiStreetsL
        {
            get
            {
                List<VW_NSI_STREET> vwNsiStreetsL = new List<VW_NSI_STREET>
                {
                    new VW_NSI_STREET
                    {
                        NSTREET_ID = 1,
                        NSTREET_NAME = "Test",
                    }
                };
                return vwNsiStreetsL;
            }
        }


        public IEnumerable<NSI_VILLAGE> NsiVillages => serv.Get_NSI_VILLAGE();
        public List<NSI_VILLAGE> GetNsiVillages()
        {
            using (EntityServ _serv = new EntityServ(connectionString))
            {
                nsiVillagesL = _serv.Get_NSI_VILLAGE().ToList();
                return nsiVillagesL;
            }
        }
        private List<NSI_VILLAGE> nsiVillagesL;
        public List<NSI_VILLAGE> NsiVillagesL => nsiVillagesL;
        public IEnumerable<VW_NSI_VILLAGE> VwNsiVillages => serv.Get_VW_NSI_VILLAGE();


        public List<scr_user1> UsersL
        {
            get
            {
                List<scr_user1> users = new List<scr_user1>
                {
                    new scr_user1{ user_id = 1964, user_name = "Евгений"},
                    new scr_user1{ user_id = 1986, user_name = "Андрей"},
                    new scr_user1{ user_id = 1990, user_name = "Рита"},
                    new scr_user1{ user_id = 2014, user_name = "Алиса"},
                };
                return users;
            }
        }

        public scr_user1[] UsersAr { get { return UsersL.ToArray(); } }

        public string UsersJson { get { return JsonSerializer.Serialize(UsersAr); } }
        #endregion

        #region Gos
        //public void DoSmth()
        //{
        //    using (EntityServ _serv = new EntityServ(connectionString))
        //    {
        //        rgns = _serv.Get_rgn().ToList();
        //        villages = _serv.Get_village().ToList();
        //    }
        //}
        //public List<rgn> rgns;
        //public List<village> villages;
        //public IEnumerable<street> streets => serv.GetStreets();
        //public List<street> streetsL => serv.GetStreets().ToList();

        //public IEnumerable<type_street> type_streets => serv.GetTypeStreets();
        //public List<type_street> type_streetsL => serv.GetTypeStreets().OrderBy(ss => ss.tstreet_id).ToList();
        #endregion

    }

    public partial class scr_user1
    {
        public int user_id { get; set; }
        public string user_name { get; set; }

    }
}
