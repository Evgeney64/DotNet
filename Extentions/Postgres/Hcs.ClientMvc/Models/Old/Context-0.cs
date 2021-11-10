using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hcs.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hcs.Store
{
    public partial class ApplicationContext : DbContext
    {
        public DbSet<SysOperation> SysOperation { get; set; }
        public DbSet<SysTransaction> SysTransaction { get; set; }
    }
}
