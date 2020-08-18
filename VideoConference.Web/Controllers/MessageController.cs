using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using VideoConference.Web.Services;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public MessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> New()
        {
            AddMessageViewModel message = new AddMessageViewModel()
            {
                ToEmailList = await GetMessageToSelectList(),
                Urgency = Urgency.Urgent,
            };

            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> New(AddMessageViewModel messageModel,string ToEmail)
        {
            if (!ModelState.IsValid)
            {
                messageModel.ToEmailList = await GetMessageToSelectList(ToEmail);
                ModelState.AddModelError("", "One or more validation errors");
                return View(messageModel);
            }

            var file = messageModel.AttachmentFile;
            string uri = string.Empty;
            if (file != null)
                uri = FileService.SaveDoc(file, "MessageAttachments");

            var toUserId = _context.Users.Where(u => u.Email == ToEmail).First().Id;
            Message message = new Message()
            {
                Title = messageModel.Title,
                ToUserId = toUserId,
                FromUserId = GetLoggedInUser().Id,
                Urgency = messageModel.Urgency,
                MessageBody = messageModel.MessageBody,
                AttachmentPath = uri,
                IsRead = false,
                DateCreated = DateTime.UtcNow.AddHours(1),
            };

            await _context.Message.AddAsync(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<List<SelectListItem>> GetMessageToSelectList(string selectedEmail="")
        {
            List<SelectListItem> messageToList = new List<SelectListItem>();
            var users = _context.Users;
            foreach(var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, AppConstant.ESRole) || await _userManager.IsInRoleAsync(user, AppConstant.SubebRole)
                    || await _userManager.IsInRoleAsync(user, AppConstant.ZonalDirectorRole))
                {
                    messageToList.Add(new SelectListItem
                    {
                        Text = user.UserName,
                        Value = user.Email,
                        Selected = string.Compare(user.Email, selectedEmail, true) == 0 ? true : false,
                    });
                }
            }

            return messageToList.ToList();
        }

        private ApplicationUser GetLoggedInUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _userManager.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
                throw new Exception();
            return user;
        }

    }
}