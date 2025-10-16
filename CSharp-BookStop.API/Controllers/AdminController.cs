using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSharp_BookStop.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) : ControllerBase
    {
        // POST: api/Admin/addRole
        [HttpPost("addRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return Conflict("Role already exists");
            }
            
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Created(roleName, roleName);
            }
            return BadRequest(result.Errors);
        }
        
        // POST: api/Admin/addUserToRole
        [HttpPost("addUserToRole")]
        public async Task<IActionResult> AddUserToRole(string roleName, Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }
    }
}
