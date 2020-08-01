using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

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
            IEnumerable<ScheduleMeetingVM> meetingsModel = _context.Meeting
                .Select(m => new ScheduleMeetingVM()
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
    }
}
