using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

using Server.Core.Model;

namespace Server.Core.Context
{
    public partial class EntityContext : DbContext
    {
        public virtual DbSet<Server.Core.Model.NSI_STREET> NSI_STREET { get; set; }
        public virtual DbSet<NSI_STREET_TYPE> NSI_STREET_TYPE { get; set; }

        public virtual DbSet<NSI_VILLAGE> NSI_VILLAGE { get; set; }
        public virtual DbSet<NSI_VILLAGE_TYPE> NSI_VILLAGE_TYPE { get; set; }

        #region Gos
        //public virtual DbSet<street> street { get; set; }
        //public virtual DbSet<type_street> type_street { get; set; }

        //public virtual DbSet<village> village { get; set; }
        //public virtual DbSet<rgn> rgn { get; set; }
        #endregion    
    }
}