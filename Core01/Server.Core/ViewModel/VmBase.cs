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
using Server.Core.CoreModel;
using Server.Core.AuthModel;
using ServiceLib;

namespace Server.Core.ViewModel
{
    public partial class VmBase
    {
        public VmBase()
        { }
        private string connectionString;
        public string ConnectionString { get { return connectionString; } }
        public string HtmlString { get; set; }
        public HtmlHelper Html { get; set; }
        public VmBase(IConfiguration configuration, ConnectionType_Enum connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType_Enum.Auth:
                    connectionString = configuration["Data:auth:ConnectionString"];
                    //authServ = new AuthServ(connectionString);
                    break;
                case ConnectionType_Enum.Data:
                    connectionString = configuration["Data:renovation_web:ConnectionString"];
                    coreServ = new CoreServ(connectionString);
                    break;
                case ConnectionType_Enum.Gos:
                    connectionString = configuration["Data:gos:ConnectionString"];
                    coreServ = new CoreServ(connectionString);
                    break;
            }
            //var contextOptions = new DbContextOptionsBuilder<EntityContext>()
            //    .UseSqlServer(connectionString)
            //    .Options;

            //context = new CoreEdm(contextOptions);
        }
        //private CoreEdm context { get; }
        private AuthServ authServ { get; }
        private CoreServ coreServ { get; }

        //public IEnumerable<NSI_STREET> NsiStreets => context.NSI_STREET;
        public IEnumerable<NSI_STREET> NsiStreets => coreServ.Get_NSI_STREET();
        public List<NSI_STREET> NsiStreetsL => NsiStreets.ToList();
        public IEnumerable<VW_NSI_STREET> VwNsiStreets => coreServ.Get_VW_NSI_STREET();
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


        public IEnumerable<NSI_VILLAGE> NsiVillages => coreServ.Get_NSI_VILLAGE();
        public List<NSI_VILLAGE> NsiVillagesL => NsiVillages.ToList();
        public IEnumerable<VW_NSI_VILLAGE> VwNsiVillages => coreServ.Get_VW_NSI_VILLAGE();


        public IEnumerable<scr_user> Users => authServ.Get_USER();
        /*public Func<IEnumerable<scr_user>> Users = () => {
            List<scr_user> users = new List<scr_user>();
            return users;
            };*/

        //public List<scr_user> UsersL => Users.ToList();
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

        public IEnumerable<street> streets => coreServ.GetStreets();
        public List<street> streetsL => coreServ.GetStreets().ToList();

        public IEnumerable<type_street> type_streets => coreServ.GetTypeStreets();
        public List<type_street> type_streetsL => coreServ.GetTypeStreets().OrderBy(ss => ss.tstreet_id).ToList();
    }

    public partial class scr_user1
    {
        public int user_id { get; set; }
        public string user_name { get; set; }

    }
}
