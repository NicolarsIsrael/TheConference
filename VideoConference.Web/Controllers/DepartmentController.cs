using System;
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
using VideoConference.Web.Services;

namespace VideoConference.Web.Controllers
{
    //[Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.DeptAdminRole + "," + AppConstant.UserRole)]
    [AllowAnonymous]
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
            return Redirect("~/department/"+GenerateRoute(name, id));
        }


        public IActionResult Dept(int id=0)
        {
            var dept= _context.Department.Where(d => d.Id == id).FirstOrDefault();
            if (id!=0 && dept == null)
                throw new Exception();

            ViewBag.Dept =id>0? dept.DeptName:"General";
            var deptMeetings = _context.Meeting.Where(m => m.MeetingType == MeetingType.General).ToList();
            if (id > 0)
                deptMeetings = deptMeetings.Where(d => d.DeptID == id).ToList();

            IEnumerable<ScheduleMeetingVM> meetingsModel = deptMeetings
                .Select(m => new ScheduleMeetingVM()
                {
                    Id = m.Id,
                    Topic = m.Topic,
                    DeptName =  m.DeptName,
                    StartDateString = m.StartTime.ToString("dd/MMM/yyyy (hh:mm tt)"),
                    StartDate = m.StartTime,
                    RoomName = m.RoomName,
                    CanJoin = true,
                    AnonymousLink = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" +
                                    $"/AnonMeeting/{GenerateRoute(m.Topic, m.Id)}"
                }).OrderBy(m => m.StartDate).ToList();

            foreach (var meeting in meetingsModel)
            {
                if ((DateTime.Compare(meeting.StartDate, DateTime.UtcNow.AddHours(1)) > 0))
                    meeting.CanJoin = false;
            }

            return View(meetingsModel);
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.DeptAdminRole )]
        public IActionResult ScheduleMeeting()
        {
            ScheduleMeetingVM scheduleMeeting = new ScheduleMeetingVM()
            {
                SelectDepts = GetDeptSelectList(0,true),
                StartDate = DateTime.Today,
            };

            return View(scheduleMeeting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.DeptAdminRole)]
        public async Task<IActionResult> ScheduleMeeting(ScheduleMeetingVM scheduleModel,int selectedDeptId)
        {
            if (!ModelState.IsValid)
            {
                scheduleModel.SelectDepts = GetDeptSelectList(selectedDeptId,true);
                ModelState.AddModelError("", "One or more validation errors");
                return View(scheduleModel);
            }

            var dept = _context.Department.Where(d => d.Id == selectedDeptId).FirstOrDefault();
            if (selectedDeptId != 0 && dept == null)
                throw new Exception();

            string roomName = Regex.Replace(scheduleModel.Topic, @"\s+", "");
            if (_context.Meeting.Where(m => m.RoomName == roomName).Count() > 0)
            {
                scheduleModel.SelectDepts = GetDeptSelectList(selectedDeptId,true);
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
               MeetingType = MeetingType.General,
            };

            await _context.Meeting.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction("Meeting","Home");
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.DeptAdminRole)]
        public IActionResult Users()
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

        //public Department GetLoggedInUserDept()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
        //    if (user == null)
        //        throw new Exception();

        //    Department dept = _context.Department.FirstOrDefault();
        //    if (dept == null)
        //        throw new Exception();

        //    return dept;
        //}

        private string GenerateRoute(string Name, int Id)
        {
            string phrase = string.Format("{0}-{1}", Name, Id);// Creates in the specific pattern  
            string route = "";
            route = GetByteArray(phrase).ToLower();
            route = Regex.Replace(route, @"[^a-z0-9\s-]", "");// Remove invalid characters for param  
            route = Regex.Replace(route, @"\s+", "-").Trim(); // convert multiple spaces into one hyphens
            route = Regex.Replace(route, @"\s", "-"); // Replaces spaces with hyphens    
            return route;
        }

        private string GetByteArray(string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private List<SelectListItem> GetDeptSelectList(int selectedDeptId = 0,bool includeGeneral = false)
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();
            if (User.IsInRole("Admin"))
            {
                if (includeGeneral)
                    deptSelectList.Add(new SelectListItem { Text = "General", Value = "0", Selected = true });
                foreach (var dept in depts)
                    deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString(), Selected = dept.Id == selectedDeptId ? true : false, });
            }
            else
            {
                var userDept = GetLoggedInUserDept();
                deptSelectList.Add(new SelectListItem { Text = userDept.DeptName, Value = userDept.Id.ToString() });
            }
            return deptSelectList;
        }

        public Department GetLoggedInUserDept()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception();

            Department dept = _context.Department.Where(d => d.Id == user.DeptId).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            return dept;
        }

    }
}