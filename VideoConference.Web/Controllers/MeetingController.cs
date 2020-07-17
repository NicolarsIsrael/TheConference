using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ScheduleMeeting()
        {
            ViewBag.Today = DateTime.Today;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ScheduleMeeting(ScheduleMeetingVM scheduleModel)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}