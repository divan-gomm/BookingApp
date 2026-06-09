using Microsoft.AspNetCore.Identity;

namespace BookingApp.Data

//Create roles and seed them indto the DB
//Seeder will be called in Program.cs

{
    public class IdentitySeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = ["Admin", "BusinessOwner", "Staff", "Customer"];

            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

            }

        }

    }
}
