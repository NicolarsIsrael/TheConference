using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoConference.Web.Core;
using VideoConference.Web.Data;

namespace VideoConference.Web
{
    public static class DatabaseSeeder
    {

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            try
            {
                var _roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                var _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                using (var context = new ApplicationDbContext(
                    serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
                {

                    if (await _roleManager.FindByNameAsync("Admin") == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole("Admin"));
                        var user = new ApplicationUser { UserName = "admin", Email = "admin@gmail.com" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, "Admin");

                        var depts =  new List<Department>
                        {
                            new Department { DeptName = "Planning research and statistics"},
                            new Department { DeptName ="Administration & Supplies"},
                        };
                            
                        depts.ForEach(d => context.Department.Add(d));
                        await context.SaveChangesAsync();
                    }

                    if (await _roleManager.FindByNameAsync("User") == null)
                        await _roleManager.CreateAsync(new ApplicationRole("User"));

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
