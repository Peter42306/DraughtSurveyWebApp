using Microsoft.AspNetCore.Identity;
using DraughtSurveyWebApp.Models;

namespace DraughtSurveyWebApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // List of admins
            var adminUsers = new List<SeedAdmin>
            {
                new SeedAdmin { Email = "admin@example.com", Password = "Admin123!"}
                //new SeedAdmin { Email = "admin1@example.com", Password = "Admin123!"},
                //new SeedAdmin { Email = "admin2@example.com", Password = "Admin123!"}
            };            

            foreach (var adminUser in adminUsers)
            {
                var existingUser = await userManager.FindByEmailAsync(adminUser.Email);
                if (existingUser == null)
                {
                    var newAdmin = new ApplicationUser
                    {
                        UserName = adminUser.Email,
                        Email = adminUser.Email,
                        EmailConfirmed = true,
                        IsActive = true,                        
                        CreatedAt = DateTime.UtcNow,
                        AdminNote = "Seeded admin user"
                    };

                    var result = await userManager.CreateAsync(newAdmin, adminUser.Password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newAdmin, "Admin");
                    }
                }
                else
                {
                    Console.WriteLine($"User `{adminUser.Email}` already exists. Skipping creation.");
                }

            }


        }
    }
}
