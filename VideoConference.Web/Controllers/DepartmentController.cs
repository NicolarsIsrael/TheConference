using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VideoConference.Web.Core;
using VideoConference.Web.Data;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,DeptAdmin,User")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public DepartmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }

        public IActionResult Index()
        {
            return Redirect(GenerateDeptRoute());
        }

        public IActionResult Dept(int id=0)
        {
            var dept= _context.Department.Where(d => d.Id == id).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            ViewBag.Dept = dept.DeptName;
            return View();
        }

        public Department GetLoggedInUserDept()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception();

            Department dept = _context.Department.Where(d => d.Id == user.DeptID).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            return dept;
        }

        private string GenerateDeptRoute()
        {
            string route = "";
            Department dept = GetLoggedInUserDept();
            route = dept.DeptName + "-" + dept.Id;
            string str = GetByteArray(route).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");// Remove invalid characters for param  
            str = Regex.Replace(str, @"\s+", "-").Trim(); // convert multiple spaces into one hyphens
            str = Regex.Replace(str, @"\s", "-"); // Replaces spaces with hyphens    
            route ="~/department/"+ str;
            return route;
        }

        private string GetByteArray(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

    }
}