using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdmin(IServiceProvider serviceProvider)
        {
            // Get required services
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            // Create Admin role if it doesn't exist
            string adminRoleName = "Admin";
            var adminRoleExists = await roleManager.RoleExistsAsync(adminRoleName);
            if (!adminRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }
            // Create default admin user if it doesn't exist
            string adminEmail = "admin@brothersoats.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true, // Skip email confirmation
                    FirstName = "Admin",
                    LastName = "User"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
            }
        }

        public static async Task SeedUser(IServiceProvider serviceProvider)
        {
            // Get required services
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            // Create User role if it doesn't exist
            string userRoleName = "User";
            var userRoleExists = await roleManager.RoleExistsAsync(userRoleName);
            if (!userRoleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(userRoleName));
            }
        }
    }
}