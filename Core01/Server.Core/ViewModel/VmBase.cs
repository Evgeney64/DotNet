using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        public VmBase(IConfiguration configuration, ConnectionType_Enum connectionType)
        {
            switch (connectionType)
            {
                case ConnectionType_Enum.Auth:
                    connectionString = configuration["Data:auth:ConnectionString"];
                    authServ = new AuthServ(connectionString);
                    break;
                case ConnectionType_Enum.Data:
                    connectionString = configuration["Data:renovation_web:ConnectionString"];
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


        public IEnumerable<NSI_VILLAGE> NsiVillages => coreServ.Get_NSI_VILLAGE();
        public List<NSI_VILLAGE> NsiVillagesL => NsiVillages.ToList();
        public IEnumerable<VW_NSI_VILLAGE> VwNsiVillages => coreServ.Get_VW_NSI_VILLAGE();


        public IEnumerable<scr_user> Users => authServ.Get_USER();
        public List<scr_user> UsersL => Users.ToList();
    }
}
