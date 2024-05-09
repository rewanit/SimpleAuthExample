using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using SimpleAuthExample.DB;
using SimpleAuthExample.DB.Model;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace SimpleAuthExample.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class LoginController : ControllerBase
    {
        private readonly SimpleAuthContext context;

        public LoginController( SimpleAuthContext context)
        {
            this.context = context;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn(string login, string password)
        {

            var normalizedLogin = login.Normalize().ToLower();

            var user = await context.Users.FirstOrDefaultAsync(x => x.Login == normalizedLogin && x.Password == password.Trim());
            if (user == null)
            {
                return Ok("User not found");
            }

            var roles = context.Users.Include(x=>x.Roles).First(x=>x.Id == user.Id).Roles.Select(x => x.Title);
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                roles.Select(x => new Claim(ClaimsIdentity.DefaultRoleClaimType, x)),
                CookieAuthenticationDefaults.AuthenticationScheme
                );
            claimsIdentity.AddClaim(new Claim("Login", normalizedLogin));
            claimsIdentity.AddClaim(new Claim("UserId", user.Id.ToString()));
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(principal);

            
            return Ok("Successful");


        }
        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            var data = new StringBuilder();
            data.AppendLine($"UserId: {ExtractUniqueClaim(this.HttpContext.User,"UserId")}");
            data.AppendLine($"Login: {ExtractUniqueClaim(this.HttpContext.User,"Login")}");
            data.AppendLine($"Authenticated: {this.HttpContext.User.Identity?.IsAuthenticated}");
            data.AppendLine($"Roles: {string.Join(" ",HttpContext.User.Claims.Where(x=>x.Type == ClaimTypes.Role).Select(x=>x.Value))}");



            return Ok(data.ToString());   
        }


        private string? ExtractUniqueClaim(ClaimsPrincipal claimsPrincipal,string name)
        {
            return claimsPrincipal.Claims.FirstOrDefault(x => x.Type == name)?.Value;
        }

    }
}
