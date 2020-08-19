using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using VideoConference.Web.Services;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.DeptAdminRole
        + "," + AppConstant.ESRole + "," + AppConstant.SubebAdminRole + "," + AppConstant.ZonalDirectorAdminRole)]
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

        public async Task<IActionResult> Index()
        {
            IEnumerable<UserViewModel> users = _context.Users
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.UserName,
                    Name = u.Name,
                    User = u,
                    Dept = u.DeptName,
                    DeptId = u.DeptId,
                }).ToList();

            if (User.IsInRole(AppConstant.DeptAdminRole))
            {
                var dept = GetLoggedInUserDept();
                users = users.Where(u => u.DeptId == dept.Id).ToList();
            }

            foreach(var user in users)
            {
                if (await _userManager.IsInRoleAsync(user.User, AppConstant.AdminRole) || await _userManager.IsInRoleAsync(user.User,AppConstant.ESRole))
                {
                    user.IsAdmin = true;user.IsDeptAdmin = false;
                }
                else if(await _userManager.IsInRoleAsync(user.User,AppConstant.DeptAdminRole))
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

        public async Task<IActionResult> ViewUser(string id)
        {
            var user = _context.Users.Where(u => u.Id == id).First();
            UserViewModel userModel = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName,
                Name = user.Name,
                User = user,
                Dept = user.DeptName,
                DeptId = user.DeptId,
            };
            if (await _userManager.IsInRoleAsync(userModel.User, AppConstant.AdminRole) || await _userManager.IsInRoleAsync(userModel.User, AppConstant.ESRole))
            {
                userModel.IsAdmin = true; userModel.IsDeptAdmin = false;
            }
            else if (await _userManager.IsInRoleAsync(userModel.User, AppConstant.DeptAdminRole))
            {
                userModel.IsAdmin = false; userModel.IsDeptAdmin = true;
            }
            else
            {
                userModel.IsAdmin = false; userModel.IsDeptAdmin = false;
            }
            return View(userModel);
        }

        public IActionResult RegisterUser()
        {
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                Departments = GetDeptSelectList(),
                UserTypes = GetUserTypeSelectList(),
                UserName = "",
            };
            return View(registerModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerModel,int deptId, int userType)
        {
            if (ModelState.IsValid)
            {
                var dept = _context.Department.Where(d => d.Id == deptId).FirstOrDefault();
                if (dept == null && deptId>0)
                    throw new Exception();

                var deptName = "";
                int _deptId = 0;
                if (dept == null)
                {
                    if (deptId == -1)
                        deptName = "SUBEB";
                    else if (deptId == -2)
                        deptName = "Zonal Director";
                }
                else
                {
                    deptName = dept.DeptName;
                    _deptId = dept.Id;
                }
                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email,Name=registerModel.Name, DeptId=_deptId,DeptName = deptName };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                   
                    string userRole = AppConstant.UserTypes[userType - 1, 1];
                    if (await _roleManager.FindByNameAsync(userRole) == null)
                        await _roleManager.CreateAsync(new ApplicationRole(userRole));
                    await _userManager.AddToRoleAsync(user, userRole);
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    registerModel.Departments = GetDeptSelectList(deptId);
                    registerModel.UserTypes = GetUserTypeSelectList(userType);
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(registerModel);
                }
            }
            else
            {
                registerModel.Departments = GetDeptSelectList(deptId);
                registerModel.UserTypes = GetUserTypeSelectList(userType);
                ModelState.AddModelError("", "One or more validation errors");
                return View(registerModel);
            }
            return View(registerModel);
        }

        public IActionResult EditUser(string userId)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validation errors");
                return View();
            }

            var user = _context.Users.Where(u => u.Id == userId).First();
            EditUserViewModel userModel = new EditUserViewModel()
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
            };
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel userModel)
        {
            var user = _context.Users.Where(u => u.Id == userModel.UserId).First();
            user.Name = userModel.Name;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.ESRole)]
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
                Name = user.Name,
            };

            return PartialView("_deleteUser",userModel);
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.ESRole)]
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

        private List<SelectListItem> GetDeptSelectList(int selectedDeptId = 0)
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();
            if (User.IsInRole(AppConstant.AdminRole) || User.IsInRole(AppConstant.ESRole))
            {
                foreach (var dept in depts)
                    deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString(), Selected = dept.Id == selectedDeptId ? true : false, });
                deptSelectList.Add(new SelectListItem { Text = "SUBEB", Value = "-1", Selected = selectedDeptId == -1 ? true : false });
                deptSelectList.Add(new SelectListItem { Text = "Zonal Director", Value = "-2", Selected = selectedDeptId == -2 ? true : false });
            }
            else if(User.IsInRole(AppConstant.DeptAdminRole))
            {
                var userDept = GetLoggedInUserDept();
                deptSelectList.Add(new SelectListItem { Text = userDept.DeptName, Value = userDept.Id.ToString() });
            }else if (User.IsInRole(AppConstant.ZonalDirectorAdminRole))
            {
                deptSelectList.Add(new SelectListItem { Text = "Zonal Director", Value = "-2" });
            }else if (User.IsInRole(AppConstant.SubebAdminRole))
            {
                deptSelectList.Add(new SelectListItem { Text = "SUBEB", Value = "-1" });
            }
            return deptSelectList;
        }

        private List<SelectListItem> GetUserTypeSelectList(int selectedUserType = 0)
        {
            List<SelectListItem> userTypeSelectList = new List<SelectListItem>();
            if (User.IsInRole(AppConstant.AdminRole) || User.IsInRole(AppConstant.ESRole) || User.IsInRole(AppConstant.DeptAdminRole))
            {
                for (int i = 0; i < AppConstant.UserTypes.GetLength(0); i++)
                {
                    userTypeSelectList.Add(new SelectListItem
                    {
                        Text = AppConstant.UserTypes[i, 1],
                        Value = AppConstant.UserTypes[i, 0],
                        Selected = selectedUserType.ToString() == AppConstant.UserTypes[i, 0] ? true : false,
                    });
                }
            }
            else if (User.IsInRole(AppConstant.DeptAdminRole))
                userTypeSelectList.Add(new SelectListItem { Text = AppConstant.UserRole, Value = "1" });
            else if (User.IsInRole(AppConstant.SubebAdminRole))
                userTypeSelectList.Add(new SelectListItem { Text = AppConstant.SubebRole, Value = "5" });
            else if (User.IsInRole(AppConstant.ZonalDirectorAdminRole))
                userTypeSelectList.Add(new SelectListItem { Text = AppConstant.ZonalDirectorRole, Value = "4" });


            return userTypeSelectList;
        }

    }
}