﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Authorize()]
    public class DocumentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DocumentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleMananger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleMananger;
        }

        public IActionResult Index(string query)
        {
            var user = GetLoggedInUser();
            var documents = _context.Document;
            IEnumerable<ViewDocumentViewModel> docModels = documents
                .Select(d => new ViewDocumentViewModel()
                {
                    Id = d.Id,
                    Title = d.Title,
                    DocumentNumber = d.DocumentNumber,
                    CurrentOffice = d.CurrentDepartment.DeptName,
                    DateReceived = d.DateReceived,
                    DateReceivedString = d.DateReceived.ToString("dd/MMM/yyyy (hh:mm tt)"),
                    //CanMinute = d.CurrentDepartment.Id != user.DeptId ? true : false,
                }).OrderByDescending(d => d.DateReceived).ToList();

            if (!string.IsNullOrEmpty(query))
                docModels = docModels.Where(d => d.Title.ToLower().Contains(query.ToLower())
                    || d.DocumentNumber.ToLower().Contains(query.ToLower())
                    || d.CurrentOffice.ToLower().Contains(query.ToLower())).ToList();

            ViewBag.Query = query;
            return View(docModels);
        }

        [Authorize(Roles =AppConstant.SecretaryRole)]
        public IActionResult RegisterDocument()
        {
            RegisterDocumentViewModel docModel = new RegisterDocumentViewModel()
            {
                Departments = GetDeptSelectList(),
            };

            return View(docModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstant.SecretaryRole)]
        public async Task<IActionResult> RegisterDocument(RegisterDocumentViewModel docModel, int SubmittedDeptId)
        {
            if (!ModelState.IsValid)
            {
                docModel.Departments = GetDeptSelectList(SubmittedDeptId);
                ModelState.AddModelError("", "One or more validation errors");
                return View(docModel);
            }

            var dept = _context.Department.Where(d => d.Id == SubmittedDeptId).First();
            Document document = new Document()
            {
                Title = docModel.Title,
                CurrentDepartment = dept,
                SubmittedBy = docModel.SubmittedBy,
                RecievedBy = docModel.RecievedBy,
                DateReceived = DateTime.UtcNow.AddHours(1),
                Remarks = docModel.Remarks,
                AddressedTo = docModel.AddressedTo,
                UserId = GetLoggedInUser().Id,
            };
            _context.Document.Add(document);
            await _context.SaveChangesAsync();
            document.DocumentNumber = document.Id.ToString() + DateTime.UtcNow.AddHours(1).ToString("hhmmssfff");
            _context.Entry(document).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ViewDocument(int id=0)
        {
            var document = _context.Document.Where(d => d.Id == id).Include("CurrentDepartment").First();
            var docMinute = _context.DocumentMinute.Include(d=>d.FromDepartment)
                            .Include(d=>d.ToDepartment).Where(d => d.Document.Id == document.Id);
            ViewDocumentViewModel docModel = new ViewDocumentViewModel()
            {
                CurrentOffice = document.CurrentDepartment.DeptName,
                DocumentNumber = document.DocumentNumber,
                Title = document.Title,
                Id = document.Id,
                DateReceivedString = document.DateReceived.ToString("dd/MMM/yyy (hh:mm tt)"),
                Remarks = document.Remarks,
                AddressedTo = document.AddressedTo,
                DocMinutes = docMinute
                    .Select(d => new DocumentMinuteViewModel()
                    {
                        FromDept = d.FromDepartment.DeptName,
                        ToDept = d.ToDepartment.DeptName,
                        SubmittedBy = d.SubmittedBy,
                        ReceivedBy = d.RecievedBy,
                        DateCreated = d.DateMinuted,
                        DateCreatedString = d.DateMinuted.ToString("dd/MMM/yyyy (hh:mm tt)"),
                        Remarks = d.Remarks,
                        AddressedTo = d.AddressedTo,
                    }).OrderByDescending(d=>d.DateCreated).ToList(),
            };

            return View(docModel);
        }

        [Authorize(Roles = AppConstant.SecretaryRole)]
        public IActionResult Minute(int id = 0)
        {
            var document = _context.Document.Include(d=>d.CurrentDepartment).Where(d => d.Id == id).First();

            var user = GetLoggedInUser();
            AddDocumentMinuteViewModel documentModel = new AddDocumentMinuteViewModel()
            {
                DocId = document.Id,
                DocTitle = document.Title,
                DocNumber = document.DocumentNumber,
                FromDeptId = document.CurrentDepartment.Id,
                FromDept = document.CurrentDepartment.DeptName,
                ToDeptId = user.DeptId,
                ToDept = user.DeptName,
                //Departments = GetDeptSelectList(0,document.CurrentDepartment.Id),
            };
            return View(documentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AppConstant.SecretaryRole)]
        public async Task<IActionResult> Minute(AddDocumentMinuteViewModel docMinuteModel)
        {
            if (!ModelState.IsValid)
            {
                //docMinuteModel.Departments = GetDeptSelectList(SubmittedDeptId,docMinuteModel.currentDeptId);
                ModelState.AddModelError("", "One or more validation errors");
                return View(docMinuteModel);
            }

            Document document = _context.Document.Where(d => d.Id == docMinuteModel.DocId).First();
            Department fromDepartment = _context.Department.Where(d => d.Id == docMinuteModel.FromDeptId).First();
            Department toDepartment = _context.Department.Where(d => d.Id == docMinuteModel.ToDeptId).First();

            DocumentMinute docMinute = new DocumentMinute()
            {
                DateMinuted = DateTime.UtcNow.AddHours(1),
                Document = document,
                FromDepartment = fromDepartment,
                ToDepartment = toDepartment,
                Remarks = docMinuteModel.Remarks,
                RecievedBy = docMinuteModel.ReceivedBy,
                SubmittedBy = docMinuteModel.SubmittedBy,
                AddressedTo=docMinuteModel.AddressedTo,
                UserId = GetLoggedInUser().Id,
            };
            document.CurrentDepartment = toDepartment;
            await _context.DocumentMinute.AddAsync(docMinute);
            _context.Document.Update(document);
            _context.Entry(document).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ViewDocument),new { id = document.Id });
        }

        private List<SelectListItem> GetDeptSelectList(int selectedDeptId = 0,int ignoreDept=0)
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();

            //deptSelectList.Add(new SelectListItem { Text = "General", Value = "0", Selected = true, });
            foreach (var dept in depts)
                if (dept.Id != ignoreDept)
                    deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString(), Selected = dept.Id == selectedDeptId ? true : false, });

            return deptSelectList;
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