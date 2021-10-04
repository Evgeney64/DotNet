using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using ServiceLib;

namespace Server.Core.CoreModel
{
    public partial class CoreEdm : EntityContext
    {
        public CoreEdm()
        { }
        public CoreEdm(DbContextOptions<EntityContext> options)
            : base(options)
        { }

        public virtual DbSet<NSI_STREET> NSI_STREET { get; set; }
        public virtual DbSet<NSI_STREET_TYPE> NSI_STREET_TYPE { get; set; }

        public virtual DbSet<NSI_VILLAGE> NSI_VILLAGE { get; set; }
        public virtual DbSet<NSI_VILLAGE_TYPE> NSI_VILLAGE_TYPE { get; set; }

        #region Gos
        public virtual DbSet<street> street { get; set; }
        public virtual DbSet<type_street> type_street { get; set; }
        #endregion
    }
}
