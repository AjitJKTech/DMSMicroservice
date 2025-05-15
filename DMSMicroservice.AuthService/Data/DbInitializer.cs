using DMSMicroservice.AuthService.Models;
using Microsoft.AspNetCore.Identity;

namespace DMSMicroservice.AuthService.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndAdmin(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            // create role 
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            //Create Editor role
            if (!await roleManager.RoleExistsAsync("Editor"))
            {
                await roleManager.CreateAsync(new IdentityRole("Editor"));
            }

            // Create viewer role
            if (!await roleManager.RoleExistsAsync("Viewer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Viewer"));
            }

            // Check if the admin user exists
            var admin =await userManager.FindByEmailAsync("Admin@jktech.com");
            if (admin == null)
            {
                var newAdmin = new ApplicationUser
                {
                    Email = "Admin@jktech.com",
                    UserName = "Admin@jktech.com",
                    EmailConfirmed = true,
                    Role="Admin"
                };

                // Create the admin user
                var result = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (result.Succeeded)
                {
                    // Assign the admin role to the user
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
                else
                {
                    // Handle errors if needed
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creating admin user: {error.Description}");
                    }
                }
            }

        }
    }
}
