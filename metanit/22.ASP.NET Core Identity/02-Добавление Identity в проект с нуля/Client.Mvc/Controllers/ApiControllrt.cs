using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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
            if (this.UsersL != null)
            { }

            return new ObjectResult(this.UsersL);
        }
        #endregion

        #region http://localhost:58982/api/users/GetUsers
        [HttpGet("{action}")]
        public async Task<ActionResult<IEnumerable<scr_user>>> GetUsers()
        {
            if (this.UsersL != null)
            { }

            return new ObjectResult(this.UsersL);
        }
        #endregion

        #region http://localhost:58982/api/users/123
        [HttpGet("{id}")]
        public async Task<ActionResult<scr_user>> Get(int id)
        {
            if (this.UsersL != null)
            { }
            scr_user user = this.UsersL.Where(ss => ss.user_id == id).FirstOrDefault();
            return new ObjectResult(user);
        }
        #endregion

        #region http://localhost:58982/api/users/GetUser/123
        [HttpGet("{action}/{id}")]
        public async Task<ActionResult<scr_user>> GetUser(int id)
        {
            if (this.UsersL != null)
            { }
            scr_user user = this.UsersL.Where(ss => ss.user_id == id).FirstOrDefault();
            return new ObjectResult(user);
        }
        #endregion

        // POST api/users
        //[HttpPost]
        //public async Task<ActionResult<scr_user>> Post(scr_user user)
        //{
        //    if (user == null)
        //    {
        //        return BadRequest();
        //    }
        //    //db.Users.Add(user);
        //    //await db.SaveChangesAsync();
        //    return Ok(user);
        //}

        [HttpPost("{action}")]
        public async Task<ActionResult<scr_user>> SaveUser(scr_user _user)
        {
            if (_user == null)
            {
                return BadRequest();
            }
            scr_user user = this.UsersL.Where(ss => ss.id == _user.id).FirstOrDefault();
            if (user == null)
            {
                user.name = _user.name;
            }
            return Ok(user);
        }

        [HttpPost("{action}")]
        public async Task<ActionResult<scr_user>> CreateUser(scr_user _user)
        {
            if (_user == null)
            {
                return BadRequest();
            }
            UsersL.Add(_user);
            return Ok(_user);
        }



        public List<scr_user> UsersL
        {
            get
            {
                return DbContext.UsersL;
            }
        }

    }

    public partial class scr_user
    {
        public int id { get; set; }
        public string name { get; set; }
        public int age { get; set; }

        public int user_id { get { return id; } }
        public string user_name { get { return name; } }
    }

    public static class DbContext
    {
        public static List<scr_user> UsersL
        {
            get
            {
                if (usersL == null)
                {
                    usersL = new List<scr_user>
                    {
                        new scr_user{ id = 1, age = 1964, name = "Евгений"},
                        new scr_user{ id = 2, age = 1986, name = "Андрей"},
                        new scr_user{ id = 3, age = 1990, name = "Рита"},
                        new scr_user{ id = 4, age = 2014, name = "Алиса"},
                    };
                }
                return usersL;
            }
        }
        private static List<scr_user> usersL;
    }

}
