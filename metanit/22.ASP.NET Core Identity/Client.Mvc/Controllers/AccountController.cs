using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using System.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using TokenApp.Models; // класс Person

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.CoreModel;
using Server.Core.AuthModel;
using ru.tsb.mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration configuration { get; }

        public AccountController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(scr_user _user)
        {
            if (ModelState.IsValid)
            {
                VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);

                scr_user user = vmBase.Users
                    .Where(ss => ss.email == _user.email && ss.password == _user.password)
                    .FirstOrDefault();
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(_user);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(scr_user _user)
        {
            if (ModelState.IsValid)
            {
                VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);

                scr_user user = vmBase.Users
                    .Where(ss => ss.email == _user.email)
                    .FirstOrDefault();
                if (user == null)
                {
                    // добавляем пользователя в бд
                    //db.Users.Add(new User { Email = model.Email, Password = model.Password });
                    //await db.SaveChangesAsync();

                    //await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(_user);
        }

        private async Task Authenticate(scr_user _user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, _user.email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, _user.role),
                //new Claim(ClaimTypes.Locality, _user.city),
                new Claim("state", _user.state),
                new Claim(ClaimTypes.DateOfBirth, _user.year.ToString())
            };
            { }
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(
                claims,
                "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType
                );
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        #region 04 - Авторизация с помощью JWT-токенов
        [HttpPost("/token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            { }
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            VmBase vmBase = new VmBase(configuration, ConnectionType_Enum.Auth);

            scr_user user = vmBase.Users
                .Where(ss => ss.email == username)
                .FirstOrDefault();

            //Person person = people.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.role)
                };
                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", 
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType
                        );
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }
        #endregion

    }


    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [Authorize]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }

        [Authorize(Roles = "admin")]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            return Ok("Ваша роль: администратор");
        }
    }
}