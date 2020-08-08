using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using VideoConference.Web.Services;

namespace VideoConference.Web.Controllers
{
    [Authorize(Roles = AppConstant.ESRole)]
    public class EsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult Dashboard()
        {
            IEnumerable<ScheduleMeetingVM> meetingsModel = _context.Meeting.Where(m => m.MeetingType ==MeetingType.ExecutiveSecretary)
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

        public IActionResult ScheduleMeeting()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleMeeting(ScheduleMeetingVM meetingModel)
        {
            if (!ModelState.IsValid)
            {   ModelState.AddModelError("", "One or more validation errors");
                return View(meetingModel);
            }

            string roomName = Regex.Replace(meetingModel.Topic, @"\s+", "");
            if (_context.Meeting.Where(m => m.RoomName == roomName).Count() > 0)
            {
                ModelState.AddModelError("", "Topic already exist");
                return View(meetingModel);
            }

            Meeting meeting = new Meeting()
            {
                Topic = meetingModel.Topic,
                RoomName = roomName,
                StartTime = meetingModel.StartDate,
                DeptID = 0,
                DeptName = "Exec private meeting",
                MeetingType = MeetingType.ExecutiveSecretary,
            };

            await _context.Meeting.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
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