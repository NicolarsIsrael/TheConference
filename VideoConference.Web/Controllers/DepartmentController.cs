using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

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
            var allMeetings = _context.Meeting.ToList();
            IEnumerable<ScheduleMeetingVM> meetingsModel = allMeetings
                .Where(m => m.DeptID == dept.Id).Select(m => new ScheduleMeetingVM()
                {
                    Id = m.Id,
                    Topic = m.Topic,
                    StartDateString = m.StartTime.ToString("dd/MMM/yyyy (hh:mm tt)"),
                    StartDate = m.StartTime,
                    CanJoin = m.StartTime < DateTime.Now ? true : false,
                    RoomName = m.RoomName,
                }).OrderBy(m => m.StartDate);
            return View(meetingsModel);
        }


        [Authorize(Roles = "DeptAdmin")]
        public IActionResult ScheduleMeeting()
        {

            var dept = GetLoggedInUserDept();
            if (dept == null)
                throw new Exception();

            ScheduleMeetingVM scheduleMeeting = new ScheduleMeetingVM()
            {
                DeptId = dept.Id,
            };

            ViewBag.Today = DateTime.Today;
            return View(scheduleMeeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DeptAdmin")]
        public async Task<IActionResult> ScheduleMeeting(ScheduleMeetingVM scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validation errors");
                return View(scheduleModel);
            }

            var dept = _context.Department.Where(d => d.Id == scheduleModel.DeptId).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            string roomName = Regex.Replace(scheduleModel.Topic, @"\s+", "");
            if (_context.Meeting.Where(m => m.RoomName == roomName).Count() > 0)
            {
                ModelState.AddModelError("", "Topic already exist");
                return View(scheduleModel);
            }

            Meeting meeting = new Meeting()
            {
                Topic = scheduleModel.Topic,
                RoomName = roomName,
                StartTime = scheduleModel.StartDate,
                DeptID = dept.Id,
            };

            await _context.Meeting.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "DeptAdmin")]
        public IActionResult RegisterUser()
        {
            Department dept = GetLoggedInUserDept();
            RegisterViewModel registerModel = new RegisterViewModel()
            {
                DeptId = dept.Id,
                DeptName = dept.DeptName,
            };
            return View(registerModel);
        }

        [Authorize(Roles ="DeptAdmin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                Department dept = GetLoggedInUserDept();
                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email,DeptID = dept.Id };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync("User") == null)
                        await _roleManager.CreateAsync(new ApplicationRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");
                    return RedirectToAction(nameof(Users));
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

        public IActionResult Users()
        {
            var dept = GetLoggedInUserDept();
            IEnumerable<UserViewModel> users = _context.Users
                .Where(u => u.DeptID == dept.Id).Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.UserName,
                }).ToList();

            return View(users);
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