using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace VideoConference.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IActionResult D()
        {
            return View();
        }

        [Authorize()]
        public IActionResult Index()
        {
            var a = DateTime.Now;
            var b = _context.Meeting;
            var c = _context.Meeting.Where(m => m.Id == 4);
            IEnumerable<ScheduleMeetingVM> meetingsModel = _context.Meeting
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
                                    $"/AnonMeeting/{GenerateRoute(m.Topic,m.Id)}"
                }).OrderBy(m => m.StartDate).ToList();


            return View(meetingsModel);
        }

        public IActionResult EndMeeting()
        {
            return PartialView("_endMeetingMessage");
        }

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
