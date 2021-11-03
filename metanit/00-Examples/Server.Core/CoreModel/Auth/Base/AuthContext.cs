using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using ServiceLib;

namespace Server.Core.AuthModel
{
    public partial class AuthEdm : ServiceLib.EntityContext
    {
        public AuthEdm()
        { }
        public AuthEdm(DbContextOptions<EntityContext> options)
            : base(options)
        { }

        public virtual DbSet<scr_user> scr_user { get; set; }
    }
}
