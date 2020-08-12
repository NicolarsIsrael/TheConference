using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Services;

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

                    if (await _roleManager.FindByNameAsync(AppConstant.AdminRole) == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.AdminRole));
                        var user = new ApplicationUser { UserName = "admin", Email = "admin@gmail.com",DeptId=0,DeptName="Admin" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.AdminRole);

                        var depts = new List<Department>
                        {
                            new Department { DeptName ="Deputy E.S Technical"},
                            new Department { DeptName ="Deputy E.S Services"},
                            new Department { DeptName ="Federal Teachers Scheme"},
                            new Department { DeptName ="Procurement"},
                            new Department { DeptName ="Public Relation/Protocol"},
                            new Department { DeptName ="Legal"},
                            new Department { DeptName ="Internal Audit"},
                            new Department { DeptName ="Special Project"},
                            new Department { DeptName ="Servicom"},
                            new Department { DeptName ="Administration & Supplies"},
                            new Department { DeptName ="Finance & Accounts"},
                            new Department { DeptName ="Planning Research & Statistics"},
                            new Department { DeptName ="Academic Services"},
                            new Department { DeptName ="Social Mobilization"},
                            new Department { DeptName ="ICT"},
                        };
                            
                        depts.ForEach(d => context.Department.Add(d));
                        await context.SaveChangesAsync();
                    }

                    if (await _roleManager.FindByNameAsync(AppConstant.ESRole) == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.ESRole));
                        var user = new ApplicationUser { UserName = "ExecutiveSec", Email = "es@gmail.com", DeptId = 0, DeptName = "Executive secretary" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.ESRole);
                    }
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.ESRole));

                    if (await _roleManager.FindByNameAsync(AppConstant.ZonalDirectorRole) == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.ZonalDirectorRole));
                        var user = new ApplicationUser { UserName = "ZonalDirector", Email = "zonaldirector1@gmail.com", DeptId = 0, DeptName = "Zonal Director" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.ZonalDirectorRole);
                    }

                    if (await _roleManager.FindByNameAsync(AppConstant.DeptAdminRole) == null)
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.DeptAdminRole));

                    if (await _roleManager.FindByNameAsync(AppConstant.ZonalDirectorAdminRole) == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.ZonalDirectorAdminRole));
                        var user = new ApplicationUser { UserName = "ZonalDirectorAdmin", Email = "zonaldirectoradmin@gmail.com", DeptId = 0, DeptName = "Zonal Director" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.ZonalDirectorAdminRole);
                    }

                    if (await _roleManager.FindByNameAsync(AppConstant.SubebAdminRole) == null)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.SubebAdminRole));
                        var user = new ApplicationUser { UserName = "SUBEBAdmin", Email = "subebadmin@gmail.com", DeptId = 0, DeptName = "SUBEB" };
                        var result = await _userManager.CreateAsync(user, "Abc123*");
                        if (!result.Succeeded)
                            throw new Exception();
                        await _userManager.AddToRoleAsync(user, AppConstant.SubebAdminRole);
                    }



                    if (await _roleManager.FindByNameAsync(AppConstant.SecretaryRole) == null)
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.SecretaryRole));

                    if (await _roleManager.FindByNameAsync(AppConstant.UserRole) == null)
                        await _roleManager.CreateAsync(new ApplicationRole(AppConstant.UserRole));
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
