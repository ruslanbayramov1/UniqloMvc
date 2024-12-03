using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqloMvc.Enums;
using UniqloMvc.Models;

namespace UniqloMvc.Extensions
{
    public static class SeedExtension
    {
        public static void UseCustomUserData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var _userManager = scope.ServiceProvider.GetRequiredService <UserManager<User>>();

                CreateRoles(_roleManager).Wait();
                CreateAdmin(_userManager).Wait();
            }
        }

        private async static Task CreateRoles(RoleManager<IdentityRole> _roleManager)
        {
            int res = await _roleManager.Roles.CountAsync();
            if (res == 0)
            {
                foreach (Roles role in Enum.GetValues(typeof(Roles)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role.ToString()));
                }
            }
        }

        private async static Task CreateAdmin(UserManager<User> _userManager)
        {
            if (!await _userManager.Users.AnyAsync(x => x.UserName == "admin"))
            {
                User admin = new User
                {
                    Fullname = "admin",
                    UserName = "admin",
                    Email = "admin@gmail.com",
                };

                await _userManager.CreateAsync(admin, "123");
                await _userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
