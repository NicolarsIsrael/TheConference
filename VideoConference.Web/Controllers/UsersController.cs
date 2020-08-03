using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = "Admin,DeptAdmin")]
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


        [Authorize(Roles ="Admin,DeptAdmin")]
        public async Task<IActionResult> Index()
        {
            IEnumerable<UserViewModel> users = _context.Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.UserName,
                    User = u,
                    Dept = u.DeptName,
                }).ToList();

            foreach(var user in users)
            {
                if (await _userManager.IsInRoleAsync(user.User, "Admin"))
                {
                    user.IsAdmin = true;user.IsDeptAdmin = false;
                }
                else if(await _userManager.IsInRoleAsync(user.User, "DeptAdmin"))
                {
                    user.IsAdmin = false;user.IsDeptAdmin = true;
                }
                else
                {
                    user.IsAdmin = false;user.IsDeptAdmin = false;
                }
            }

            return View(users);
        }

        private List<SelectListItem> GetDeptSelectList(int selectedDeptId=0)
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();
            if (User.IsInRole("Admin"))
            {
                foreach (var dept in depts)
                    deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString(), Selected =dept.Id==selectedDeptId ? true : false, });
            }
            else
            {
                var userDept = GetLoggedInUserDept();
                deptSelectList.Add(new SelectListItem { Text = userDept.DeptName, Value = userDept.Id.ToString() });
            }
            return deptSelectList;
        }

        [Authorize(Roles = "Admin,DeptAdmin")]
        public IActionResult RegisterUser()
        {
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                Departments = GetDeptSelectList(),
            };
            return View(registerModel);
        }

        [Authorize(Roles = "Admin,DeptAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerModel,int deptId)
        {
            if (ModelState.IsValid)
            {
                var dept = _context.Department.Where(d => d.Id == deptId).FirstOrDefault();
                if (dept == null)
                    throw new Exception();

                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email,DeptId=dept.Id,DeptName = dept.DeptName };
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
                    registerModel.Departments = GetDeptSelectList(deptId);
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(registerModel);
                }
            }
            else
            {
                registerModel.Departments = GetDeptSelectList(deptId);
                ModelState.AddModelError("", "One or more validation errors");
                return View(registerModel);
            }
            return View(registerModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult RegisterDeptAdmin()
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();
            foreach (var dept in depts)
                deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString() });
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                Departments = GetDeptSelectList(),
            };
            return View(registerModel);
        }

        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterDeptAdmin(RegisterViewModel registerModel, int deptId)
        {
            if (ModelState.IsValid)
            {
                var dept = _context.Department.Where(d => d.Id == deptId).FirstOrDefault();
                if (dept == null)
                    throw new Exception();

                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email, DeptId = dept.Id, DeptName = dept.DeptName };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync("DeptAdmin") == null)
                        await _roleManager.CreateAsync(new ApplicationRole("DeptAdmin"));
                    await _userManager.AddToRoleAsync(user, "DeptAdmin");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    registerModel.Departments = GetDeptSelectList(deptId);
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(registerModel);
                }
            }
            else
            {
                registerModel.Departments = GetDeptSelectList(deptId);
                ModelState.AddModelError("", "One or more validation errors");
                return View(registerModel);
            }
            return View(registerModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(string id)
        {
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();
            if (user == null)
                throw new Exception();

            UserViewModel userModel = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
            };

            return PartialView("_deleteUser",userModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmDeleteUser(string id)
        {
            var user = _userManager.Users.Where(u => u.Id == id).FirstOrDefault();
            if (user == null)
                throw new Exception();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public Department GetLoggedInUserDept()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception();

            Department dept = _context.Department.Where(d=>d.Id==user.DeptId).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            return dept;
        }
    }
}