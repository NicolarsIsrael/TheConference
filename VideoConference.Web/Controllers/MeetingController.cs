using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MeetingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var allMeetings = _context.Meeting.ToList();
            IEnumerable<ScheduleMeetingVM> meetingsModel = allMeetings
                .Select(m => new ScheduleMeetingVM()
                {
                    Topic = m.Topic,
                    StartDateString = m.StartTime.ToString("dd/MMM/yyyy (hh:mm tt)"),
                    GeneratedId = m.GeneratedId,
                });
            return View(meetingsModel);
        }

        public IActionResult ScheduleMeeting()
        {
            ViewBag.Today = DateTime.Today;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleMeeting(ScheduleMeetingVM scheduleModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "One or more validation errors");
                return View(scheduleModel);
            }

            Meeting meeting = new Meeting()
            {
                Topic = scheduleModel.Topic,
                StartTime = scheduleModel.StartDate,
                GeneratedId = GenerateMeetingId(),
            };

            await _context.Meeting.AddAsync(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private string GenerateMeetingId()
        {
            string meetingId = string.Empty;

            Random rand = new Random();
            for (int i = 0; i < 9; i++)
            {
                meetingId += rand.Next(0, 10);
                if (i == 2 || i == 5)
                    meetingId += "-";
            }

            return meetingId;
        }
    }
}