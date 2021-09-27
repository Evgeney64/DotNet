using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ru.tsb.mvc.Models.Users
{
    public class AppidentityDbContext : IdentityDbContext<AppUser>
    {
        public AppidentityDbContext(DbContextOptions<AppidentityDbContext> options)
            : base(options) { }
    }
}
