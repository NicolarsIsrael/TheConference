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

                    if (await _roleManager.FindByNameAsync("SuperAdmin") == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole("SuperAdmin"));
                        var user = new ApplicationUser { UserName = "SuperAdmin", Email = "superadmin@gmail.com" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                    }


                    if (await _roleManager.FindByNameAsync("DeptAdmin") == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole("DeptAdmin"));

                        var deptAdmin1 = new ApplicationUser { UserName = "deptAdmin1", Email = "deptadmin1@gmail.com" };
                        var result = await _userManager.CreateAsync(deptAdmin1, "Abc123*");
                        if (result.Succeeded)
                        {
                            Department dept = new Department()
                            {
                                DeptName = "Dept1",
                                UserId = deptAdmin1.Id,
                            };
                            await context.Department.AddAsync(dept);
                            await context.SaveChangesAsync();
                        }

                        var deptAdmin2 = new ApplicationUser { UserName = "deptAdmin2", Email = "deptadmin2@gmail.com" };
                        result = await _userManager.CreateAsync(deptAdmin2, "Abc123*");
                        if (result.Succeeded)
                        {
                            Department dept = new Department()
                            {
                                DeptName = "Dept2",
                                UserId = deptAdmin2.Id
                            };
                            await context.Department.AddAsync(dept);
                            await context.SaveChangesAsync();
                        }

                        var deptAdmin3 = new ApplicationUser { UserName = "deptAdmin3", Email = "deptadmin3@gmail.com" };
                        result = await _userManager.CreateAsync(deptAdmin3, "Abc123*");
                        if (result.Succeeded)
                        {
                            Department dept = new Department()
                            {
                                DeptName = "Dept3",
                                UserId = deptAdmin3.Id
                            };
                            await context.Department.AddAsync(dept);
                            await context.SaveChangesAsync();
                        }

                        var deptAdmin4 = new ApplicationUser { UserName = "deptAdmin4", Email = "deptadmin4@gmail.com" };
                        result = await _userManager.CreateAsync(deptAdmin4, "Abc123*");
                        if (result.Succeeded)
                        {
                            Department dept = new Department()
                            {
                                DeptName = "Dept4",
                                UserId = deptAdmin4.Id
                            };
                            await context.Department.AddAsync(dept);
                            await context.SaveChangesAsync();
                        }

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
