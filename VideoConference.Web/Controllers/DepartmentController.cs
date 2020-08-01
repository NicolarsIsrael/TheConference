﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
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

        public IActionResult Index(int id=0)
        {
            var dept = _context.Department.Where(d => d.Id == id).FirstOrDefault();
            if (id != 0 && dept == null)
                throw new Exception();
            string name = id > 0 ? dept.DeptName : "General";
            return Redirect(GenerateDeptRoute(name, id));
        }


        public IActionResult Dept(int id=0)
        {
            var dept= _context.Department.Where(d => d.Id == id).FirstOrDefault();
            if (id!=0 && dept == null)
                throw new Exception();

            ViewBag.Dept =id>0? dept.DeptName:"General";
            var deptMeetings = _context.Meeting.ToList();
            if (id > 0)
                deptMeetings = deptMeetings.Where(d => d.DeptID == id).ToList();

            IEnumerable<ScheduleMeetingVM> meetingsModel = deptMeetings
                .Where(m => m.DeptID == dept.Id).Select(m => new ScheduleMeetingVM()
                {
                    Id = m.Id,
                    Topic = m.Topic,
                    DeptName = m.DeptName,
                    StartDateString = m.StartTime.ToString("dd/MMM/yyyy (hh:mm tt)"),
                    StartDate = m.StartTime,
                    CanJoin = m.StartTime < DateTime.Now ? true : false,
                    RoomName = m.RoomName,
                }).OrderBy(m => m.StartDate);
            return View(meetingsModel);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ScheduleMeeting()
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();
            foreach (var dept in depts)
                deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString() });
            deptSelectList.Add(new SelectListItem { Text = "General", Value = "0" });

            ScheduleMeetingVM scheduleMeeting = new ScheduleMeetingVM()
            {
                SelectDepts = deptSelectList,
                StartDate = DateTime.Today,
            };

            return View(scheduleMeeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ScheduleMeeting(ScheduleMeetingVM scheduleModel,int selectedDeptId)
        {
            if (!ModelState.IsValid)
            {
                var depts = _context.Department;
                List<SelectListItem> deptSelectList = new List<SelectListItem>();
                foreach (var _dept in depts)
                    deptSelectList.Add(new SelectListItem { Text = _dept.DeptName, Value = _dept.Id.ToString(),Selected =_dept.Id==selectedDeptId?true:false });
                deptSelectList.Add(new SelectListItem { Text = "General", Value = "0", Selected = 0 == selectedDeptId ? true : false });

                scheduleModel.SelectDepts = deptSelectList;
                ModelState.AddModelError("", "One or more validation errors");
                return View(scheduleModel);
            }

            var dept = _context.Department.Where(d => d.Id == selectedDeptId).FirstOrDefault();
            if (selectedDeptId != 0 && dept == null)
                throw new Exception();

            string roomName = Regex.Replace(scheduleModel.Topic, @"\s+", "");
            if (_context.Meeting.Where(m => m.RoomName == roomName).Count() > 0)
            {
                var depts = _context.Department;
                List<SelectListItem> deptSelectList = new List<SelectListItem>();
                foreach (var _dept in depts)
                    deptSelectList.Add(new SelectListItem { Text = _dept.DeptName, Value = _dept.Id.ToString(), Selected = _dept.Id == selectedDeptId ? true : false });
                deptSelectList.Add(new SelectListItem { Text = "General", Value = "0", Selected = 0 == selectedDeptId ? true : false });

                scheduleModel.SelectDepts = deptSelectList;
                ModelState.AddModelError("", "Topic already exist");
                return View(scheduleModel);
            }

            Meeting meeting = new Meeting()
            {
                Topic = scheduleModel.Topic,
                RoomName = roomName,
                StartTime = scheduleModel.StartDate,
                DeptID = selectedDeptId,
                DeptName = selectedDeptId == 0 ? "General" : dept.DeptName,
            };

            await _context.Meeting.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles ="Admin")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                Department dept = GetLoggedInUserDept();
                var user = new ApplicationUser { UserName = registerModel.UserName, Email = registerModel.Email};
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
                .Select(u => new UserViewModel()
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

            Department dept = _context.Department.FirstOrDefault();
            if (dept == null)
                throw new Exception();

            return dept;
        }

        private string GenerateDeptRoute(string Name, int Id)
        {
            string phrase = string.Format("{0}-{1}", Name, Id);// Creates in the specific pattern  
            string route = "";
            route = GetByteArray(phrase).ToLower();
            route = Regex.Replace(route, @"[^a-z0-9\s-]", "");// Remove invalid characters for param  
            route = Regex.Replace(route, @"\s+", "-").Trim(); // convert multiple spaces into one hyphens
            route = Regex.Replace(route, @"\s", "-"); // Replaces spaces with hyphens    
            route ="~/department/"+ route;
            return route;
        }

        private string GetByteArray(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

    }
}