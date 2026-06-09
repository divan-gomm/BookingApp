using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using BookingApp.Data;

namespace BookingApp.Data
{
    public static class IdentityUserSeeder
    {
        public static async Task SeedUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            async Task CreateUser(string email, string password, string role)
            {
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, password);
                }

                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            await CreateUser("admin@test.com", "Password123!", "Admin");
            await CreateUser("owner@test.com", "Password123!", "BusinessOwner");
            await CreateUser("staff@test.com", "Password123!", "Staff");
            await CreateUser("customer@test.com", "Password123!", "Customer");
        }
    }
}