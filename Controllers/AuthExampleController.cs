using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthExampleController : Controller
    {
        [HttpGet("Login")]
        public IActionResult Login(string returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginCheck([FromQuery]string returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            string username = HttpContext.Request.Form["username"].ToString();
            string password = HttpContext.Request.Form["password"].ToString();
            string name = HttpContext.Request.Form["name"].ToString();

            // Context.Users... check user here...
            bool userExists = username == "proba" && password == "proba";
            if (userExists)
            {
                var claims = new List<Claim>
                {
                    new Claim("username", username),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.IsPersistent, "True") // And much more...

                };
                var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var cp = new ClaimsPrincipal(ci);
                await HttpContext.SignInAsync(cp);
                return Redirect(returnURL);
            }
            else
            {
                ViewBag.Error = "Username or password invalid!";
                return View("Login");
            }
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/AuthExample/Index");
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("Secret")]
        public IActionResult Secret()
        {
            return View("SecretPage");
        }
    }
}
