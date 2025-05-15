using DMSMicroservice.UserManagementService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using static DMSMicroservice.UserManagementService.ApiEndPoints;

namespace DMSMicroservice.UserManagementService.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet(ApiEndPoints.UserApiEndPoints.Users)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();

            var usersWithRoles = new List<object>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(usersWithRoles);
        }


        [HttpPut(ApiEndPoints.UserApiEndPoints.DeleteUser)]
        public async Task<IActionResult> DeleteUser([FromBody] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return BadRequest(new { message = "Admin cannot be deleted." });
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User deleted successfully." });
            }

            return BadRequest(result.Errors);
        }


        [HttpGet(ApiEndPoints.UserApiEndPoints.GetAllRoles)]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await roleManager.Roles.Select(e => new { e.Id, e.Name }).ToListAsync();
            return Ok(roles);
        }

        [HttpPost(ApiEndPoints.UserApiEndPoints.AddNewRole)]
        public async Task<IActionResult> AddNewRole([FromBody] string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role already exists.");
            }
            var result = await roleManager.CreateAsync(new IdentityRole { Name = roleName });
            if (result.Succeeded)
            {
                return Ok(new { message = "Role added successfully." });
            }
            return BadRequest(result.Errors);
        }

        [HttpPut(ApiEndPoints.UserApiEndPoints.DeleteRole)]
        public async Task<IActionResult> DeleteRole([FromBody] string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.Name == "Admin")
            {
                return BadRequest(new { message = "Admin role cannot be deleted." });
            }

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role Deleted Successfully." });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost(ApiEndPoints.UserApiEndPoints.ChangeUserRole)]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRole model)
        {
            var user = await userManager.FindByEmailAsync(model.UserEmail);
            if (user == null)
            {
                return NotFound($"User with email {model.UserEmail} not found.");
            }

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return BadRequest(new { message = "Admin role cannot be changed." });
            }

            if (!await roleManager.RoleExistsAsync(model.NewRole))
            {
                return BadRequest($"Role {model.NewRole} does not exists.");
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return BadRequest("Failed to remove user's current role");
            }

            var addResult = await userManager.AddToRoleAsync(user, model.NewRole);
            if (addResult.Succeeded)
            {
                return Ok($"User {model.UserEmail} role changes to {model.NewRole} successfully.");
            }

            return BadRequest("Failed to add user to the new role.");
        }

        [HttpPost(ApiEndPoints.UserApiEndPoints.GetAdminInfo)]

        public async Task<IActionResult> GetAdminInfo([FromBody] string email)
        {
            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(admin);
        }


        [HttpPut(ApiEndPoints.UserApiEndPoints.UpdateAdminInfo)]
        public async Task<IActionResult> UpdateAdminInfo([FromBody] UpdateUserInfo model)
        {
            var admin = await userManager.FindByEmailAsync(model.Email);

            if (admin == null)
            {
                return NotFound(new { message = "User not found." });
            }

            admin.Email = model.Email;
            admin.UserName = model.Email;
            admin.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(admin);

            if (result.Succeeded)
            {
                return Ok(new { message = "Admin info updated successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPut(ApiEndPoints.UserApiEndPoints.ChangeAdminPassword)]
        public async Task<IActionResult> ChangeAdminPassword([FromBody] ChangePassword model)
        {
            var admin = await userManager.FindByEmailAsync(model.Email);

            if (admin == null)
            {
                return NotFound(new { message = "Admin not found." });
            }

            var result = await userManager.ChangePasswordAsync(admin, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "Admin password updated successfully" });
            }
            return BadRequest(result.Errors);
        }

        private bool isValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
