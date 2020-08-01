using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = "Admin,Users")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }


        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            IEnumerable<UserViewModel> users = _context.Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.UserName,
                }).ToList();

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterUser()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync("User") == null)
                        await _roleManager.CreateAsync(new ApplicationRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(registerModel);
                }
            }
            else
            {
                ModelState.AddModelError("", "One or more validation errors");
                return View(registerModel);
            }
            return View(registerModel);
        }

    }
}