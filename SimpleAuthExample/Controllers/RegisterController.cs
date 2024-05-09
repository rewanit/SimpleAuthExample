using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAuthExample.DB;
using SimpleAuthExample.DB.Model;
using System.ComponentModel.DataAnnotations;

namespace SimpleAuthExample.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class RegisterController: ControllerBase
    {
        private SimpleAuthContext context;


        public RegisterController(SimpleAuthContext context)
        {
            this.context = context;
        }




        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> Register(string login,[MinLength(3)] string password)
        {
            var normalizedLogin = login.Normalize().ToLower();

            var user = await context.Users.FirstOrDefaultAsync(x => x.Login == normalizedLogin);
            if (user!= null)
            {
                return Ok("login already in use");
            }

            var newUser = new User()
            {
                Login = normalizedLogin,
                Password = password.Trim(),
            };

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            return Ok("Successful");
        }
    }
}
