using System.Collections.Generic;
using System.Linq;

namespace Server.Core.AuthModel
{
    public partial class AuthServ : ServiceLib.EntityService<AuthEdm>
    {
        public AuthServ()
        { }
        public AuthServ(string connectionString)
            : base(connectionString)
        { }

        public IQueryable<scr_user> Get_USER() => Context.scr_user;
    }
}
