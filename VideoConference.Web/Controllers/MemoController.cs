using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VideoConference.Web.Core;
using VideoConference.Web.Data;
using VideoConference.Web.Models;
using VideoConference.Web.Services;

namespace VideoConference.Web.Controllers
{
    [Authorize]
    public class MemoController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MemoController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id=0)
        {
            if (id > 0)
            {
                var dept = _context.Department.Where(d => d.Id == id).FirstOrDefault();
                if (dept == null)
                    throw new Exception();
                return Redirect("~/Memo/" + GenerateRoute(dept.DeptName,id));
            }

            var allMemos = _context.Memo
                .Select(m => new MemoViewModel()
                {
                    Id = m.Id,
                    Title = m.Title,
                    DeptName = m.DeptName,
                    DateCreated = m.DateCreated,
                    DateCreatedString = m.DateCreated.ToString("dd/MMM/yyyy(hh: mm tt)"),
                    MemoFilePath = m.MemoFile,
                }).OrderByDescending(m=>m.DateCreated).ToList();

            return View(allMemos);
        }

        public IActionResult DeptMemo(int id)
        {
            var allMemos = _context.Memo.Where(d=>d.DeptId == id)
               .Select(m => new MemoViewModel()
               {
                   Id = m.Id,
                   Title = m.Title,
                   DeptName = m.DeptName,
                   DateCreated = m.DateCreated,
                   DateCreatedString = m.DateCreated.ToString("dd/MMM/yyyy(hh: mm tt)"),
                   MemoFilePath = m.MemoFile,
               }).OrderByDescending(m => m.DateCreated).ToList();
            var dept = _context.Department.Where(d => d.Id == id).FirstOrDefault();
            if (dept == null)
                throw new Exception();

            ViewBag.Dept = dept.DeptName;
            ViewBag.DeptId = dept.Id;
            return View(allMemos);
        }

        [Authorize(Roles =AppConstant.AdminRole + "," + AppConstant.ESRole)]
        public IActionResult Add(int deptId=0)
        {
            MemoViewModel memoModel = new MemoViewModel()
            {
                Departments = GetDeptSelectList(deptId),
            };

            return View(memoModel);
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.ESRole)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MemoViewModel memoModel, int selectedDeptId)
        {
            if (!ModelState.IsValid)
            {
                memoModel.Departments = GetDeptSelectList(selectedDeptId);
                ModelState.AddModelError("", "One or more validation errors");
                return View(memoModel);
            }

            var dept = _context.Department.Where(d => d.Id == selectedDeptId).FirstOrDefault();
            if (selectedDeptId != 0 && dept == null)
                throw new Exception();

            var file = memoModel.MemoFile;
            string uri = string.Empty;
            if (file == null)
                throw new Exception();
            else
                uri = FileService.SaveDoc(file, "MemoFiles");

            if (string.IsNullOrEmpty(uri) || string.IsNullOrWhiteSpace(uri))
                throw new Exception();

            Memo memo = new Memo()
            {
                Title = memoModel.Title,
                DeptId = selectedDeptId,
                DeptName = selectedDeptId == 0 ? "General" : dept.DeptName,
                DateCreated = DateTime.UtcNow.AddHours(1),
                MemoFile = uri,
            };
            await _context.Memo.AddAsync(memo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.ESRole)]
        public IActionResult DeleteMemo(int id)
        {
            var memo = _context.Memo.Where(m => m.Id == id).First();
            MemoViewModel memoModel = new MemoViewModel()
            {
                Id = memo.Id,
                DeptName = memo.DeptName,
                DateCreatedString = memo.DateCreated.ToString("dd/MMM/yyyy(hh: mm tt)"),
                Title = memo.Title,
            };
            return PartialView("_deleteMemo",memoModel);
        }

        [Authorize(Roles = AppConstant.AdminRole + "," + AppConstant.ESRole)]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var memo = _context.Memo.Where(m => m.Id == id).First();
            _context.Memo.Remove(memo);
            await _context.SaveChangesAsync();
            try
            {
                FileService.DeleteFile(memo.MemoFile);
            }
            catch (Exception){}
            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetDeptSelectList(int selectedDeptId = 0)
        {
            var depts = _context.Department;
            List<SelectListItem> deptSelectList = new List<SelectListItem>();

            deptSelectList.Add(new SelectListItem { Text = "General", Value = "0",Selected = true, });
            foreach (var dept in depts)
                deptSelectList.Add(new SelectListItem { Text = dept.DeptName, Value = dept.Id.ToString(), Selected = dept.Id == selectedDeptId ? true : false, });

            return deptSelectList;
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