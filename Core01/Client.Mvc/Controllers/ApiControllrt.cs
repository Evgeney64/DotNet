using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.CoreModel;
using Server.Core.AuthModel;
using ru.tsb.mvc;

namespace Data.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        #region Define
        private IConfiguration configuration { get; }
        public UsersController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        #endregion

        #region http://localhost:58982/api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<scr_user>>> Get()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }
        #endregion

        #region http://localhost:58982/api/users/GetUsers
        [HttpGet("{action}")]
        public async Task<ActionResult<IEnumerable<scr_user>>> GetUsers()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }
        #endregion

        #region http://localhost:58982/api/users/123
        [HttpGet("{id}")]
        public async Task<ActionResult<scr_user>> Get(int id)
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }
        #endregion

        #region http://localhost:58982/api/users/GetUsers/123
        [HttpGet("{action}/{id}")]
        public async Task<ActionResult<scr_user>> GetUsers(int id)
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }
        #endregion

        // POST api/users
        //[HttpPost]
        //public async Task<ActionResult<User>> Post(User user)
        //{
        //    if (user == null)
        //    {
        //        return BadRequest();
        //    }

        //    db.Users.Add(user);
        //    await db.SaveChangesAsync();
        //    return Ok(user);
        //}
    }
}
