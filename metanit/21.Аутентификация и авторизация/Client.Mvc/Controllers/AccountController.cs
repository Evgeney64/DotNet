using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using System.Linq;

using Server.Core.Public;
using Server.Core.ViewModel;
using Server.Core.CoreModel;
using Server.Core.AuthModel;

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
                new Claim("state", _user.state)
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
    }
}