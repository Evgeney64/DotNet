using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.CoreModel;

using ru.tsb.mvc;
using Server.Core.ViewModel;
using Server.Core.AuthModel;

namespace Data.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private IConfiguration configuration { get; }
        public ApiController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<scr_user>>> Get()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<scr_user>>> GetUsers()
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }
        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<scr_user>> Get(int id)
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);
            if (vmBase.UsersL != null)
            { }

            return new ObjectResult(vmBase.UsersL);
        }

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
