using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoConference.Web.Core;
using VideoConference.Web.CustomFilters;
using VideoConference.Web.Data;
using VideoConference.Web.Models;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private static string key = "e1b05d2ccb6d4614b4b4119de9668add";
        private static string appID = "368dbe.vidyo.io";
        private static long expiresInSecs = 1800;
        private static string expiresAt = null;

        private const long EPOCH_SECONDS = 62167219200;
        public MeetingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }


        public IActionResult Index(int id = 0)
        {
            Meeting meeting = _context.Meeting.Where(m => m.Id == id).FirstOrDefault();
            if (meeting == null)
                throw new Exception();

            return Redirect("~/Meeting/" + GenerateMeetingRoute(meeting.Topic, id));
        }

        public IActionResult Meet(int id = 0)
        {
            Meeting meeting = _context.Meeting.Where(m => m.Id == id).FirstOrDefault();
            if (meeting == null)
                throw new Exception();

            if (DateTime.Compare(meeting.StartTime, DateTime.UtcNow.AddHours(1)) > 0)
                return RedirectToAction(nameof(TimeAccessDenied));
            var username = GetLoggedInUser().UserName;

            ViewBag.Room = meeting.RoomName;
            ViewBag.Topic = meeting.Topic;
            ViewBag.Username = username;
            ViewBag.Token = generateToken(username);
            return View();
        }

        [AllowAnonymous]
        public IActionResult AnonMeeting(int id = 0)
        {
            Meeting meeting = _context.Meeting.Where(m => m.Id == id).FirstOrDefault();
            if (meeting == null)
                throw new Exception();

            if (DateTime.Compare(meeting.StartTime, DateTime.UtcNow.AddHours(1)) > 0)
                return RedirectToAction(nameof(TimeAccessDenied));

            string username = "Anon-" + DateTime.Now.ToString("hhmmssfff");
            ViewBag.Room = meeting.RoomName;
            ViewBag.Topic = meeting.Topic;
            ViewBag.Username = username;
            ViewBag.Token = generateToken(username);
            return View();
        }

        public IActionResult ChatBox()
        {
            return PartialView("_chatBox");
        }

        public IActionResult TimeAccessDenied()
        {
            return View();
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

        private string generateToken(string userName)
        {
            // As long as proper arguments were entered, generate the token
            if ((appID != null) && (key != null) && (userName != null))
            {
                string expires = "";

                // Check if using expiresInSecs or expiresAt
                if (expiresInSecs > 0)
                {
                    TimeSpan timeSinceEpoch = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                    expires = (Math.Floor(timeSinceEpoch.TotalSeconds) + EPOCH_SECONDS + expiresInSecs).ToString();
                }
                else if (expiresAt != null)
                {
                    try
                    {
                        TimeSpan epochToExpires = DateTime.Parse(expiresAt).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                        expires = (Math.Floor(epochToExpires.TotalSeconds) + EPOCH_SECONDS).ToString();
                    }
                    catch (Exception e)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }

               
                if (expiresAt != null)
                    Console.WriteLine("Setting expiresAt     : " + expiresAt);
                Console.WriteLine("Expirey time          : " + expires);
                string jid = userName + "@" + appID;
                string body = "provision" + "\0" + jid + "\0" + expires + "\0" + "";

                var encoder = new UTF8Encoding();
                var hmacsha = new HMACSHA384(encoder.GetBytes(key));
                byte[] mac = hmacsha.ComputeHash(encoder.GetBytes(body));

                // macBase64 can be used for debugging
                //string macBase64 = Convert.ToBase64String(hashmessage);

                // Get the hex version of the mac
                string macHex = BytesToHex(mac);

                string serialized = body + '\0' + macHex;
                return Convert.ToBase64String(encoder.GetBytes(serialized));
                //Console.WriteLine("\nGenerated token:\n" + Convert.ToBase64String(encoder.GetBytes(serialized)));
            }
            else
            {
                throw new Exception();
            }
        }

        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        private ApplicationUser GetLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception();
            return user;
        }

        private string GenerateMeetingRoute(string Name, int Id)
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