using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationDbContext _context;
        private static string key = "e1b05d2ccb6d4614b4b4119de9668add";
        private static string appID = "368dbe.vidyo.io";
        private static long expiresInSecs = 1800;
        private static string expiresAt = null;

        private const long EPOCH_SECONDS = 62167219200;
        public MeetingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("index", "department");

        }

        //[AllowSameSiteAttribute]
        public IActionResult Meet(int id=0)
        {
            Meeting meeting = _context.Meeting.Where(m => m.Id == id).FirstOrDefault();
            if (meeting == null)
                return Content("null");

            string username = "aadkdhlshjshksdjhflshfkslfjsslsfhskhfskhsfsskjshlsjlasdkkfhlasdflsjflsjfhsdfjljfasdfcbasdfalcmfaierirerywofasdfalfxfxmashffamxlfkfhdfamfhsflmsdfmlsfmhfslfmfshflmsajdflshdfaxfalsnfafkhfkasfsdfnhafskfhsfnksfhskdnfasdfhksdfasakfacxacahaldhahwueroawurcxbankdfhavkadfcnaxiadkaeihwecakjfaxnafaiecaxfankfekrwebdxbfkdhfbabxkdfaladadfwryiwerxnacbakdhaoeuhfskdlaweskdflcbk";
            Random rand = new Random();
            int a = rand.Next(1, username.Length - 13);
            int b = rand.Next(5, 10);
            username = username.Substring(a, b);

            ViewBag.Room = meeting.RoomName;
            ViewBag.Topic = meeting.Topic;
            ViewBag.Username = username;
            ViewBag.Token = generateToken(username);
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
    }
}