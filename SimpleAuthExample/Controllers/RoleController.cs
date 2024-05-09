using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleAuthExample.DB;
using SimpleAuthExample.DB.Model;
using System.Security.Claims;

namespace SimpleAuthExample.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly SimpleAuthContext context;

        public RoleController(SimpleAuthContext context) {
            this.context = context;
        }



        [HttpPost]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> AllRoles()
        {
            return Ok(context.Roles.Select(x => new {x.Id, x.Title }));
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AddRole(string roleName)
        {

            var normalizedRoleName = roleName.Normalize().Trim().ToLower();
            var role = context.Roles.FirstOrDefault(x => x.Title == normalizedRoleName);
            if (role!=null)
            {
                return Ok("Already in use");
            }

            var newRole = new Role()
            { 
                Title = normalizedRoleName
            };
            await context.Roles.AddAsync(newRole);
            await context.SaveChangesAsync();
            return Ok(newRole.Id);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(int userId,int roleId)
        {
            var role = context.Roles.FirstOrDefault(x => x.Id == roleId);
            if (role == null)
            {
                return Ok("Role not found");
            }

            var inRole = context.Users.Where(x => x.Id == userId && x.Roles.Any(x => x.Id == roleId)).Any();

            if (inRole)
            {
                return Ok("User In Role");
            }
                
            var user = context.Users.Include(x=>x.Roles).FirstOrDefault(x=>x.Id == userId);
            if (user == null)
            {
                return Ok("User 404");
            }

            user.Roles.Add(role);

            context.Users.Update(user);
            await context.SaveChangesAsync();
            return Ok();


        }


    }
}
