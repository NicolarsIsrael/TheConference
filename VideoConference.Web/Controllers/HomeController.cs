using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }
        
        public IActionResult Index()
        {
            IEnumerable<ScheduleMeetingVM> meetingsModel = _context.Meeting.Where(m => m.MeetingType == MeetingType.General)
             .Select(m => new ScheduleMeetingVM()
             {
                 Id = m.Id,
                 Topic = m.Topic,
                 DeptName = m.DeptName,
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
            //return View();
        }

        public IActionResult Meeting()
        {
            IEnumerable<ScheduleMeetingVM> meetingsModel = _context.Meeting.Where(m => m.MeetingType == MeetingType.General)
               .Select(m => new ScheduleMeetingVM()
               {
                   Id = m.Id,
                   Topic = m.Topic,
                   DeptName = m.DeptName,
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

        [AllowAnonymous]
        public IActionResult EndMeeting()
        {
            return PartialView("_endMeetingMessage");
        }

        [AllowAnonymous]
        public IActionResult DepartmentMeeting()
        {
            var depts = _context.Department
                .Select(d => new DeptAndIdViewModel()
                {
                    DeptId = d.Id,
                    DeptName = d.DeptName,
                }).ToList();

            DeptViewModel deptModel = new DeptViewModel()
            {
                Department = depts,
            };

            return PartialView("_departmentMeeting",deptModel);
        }

        public IActionResult DepartmentMemo()
        {
            var depts = _context.Department
              .Select(d => new DeptAndIdViewModel()
              {
                  DeptId = d.Id,
                  DeptName = d.DeptName,
              }).ToList();

            DeptViewModel deptModel = new DeptViewModel()
            {
                Department = depts,
            };

            return PartialView("_departmentMemo", deptModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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

    }
}
