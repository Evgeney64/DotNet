using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLib
{
    public partial class EntityContext : DbContext
    {
        public EntityContext()
        { }
        public EntityContext(DbContextOptions<EntityContext> options)
            : base(options)
        { }
    }
}