using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class MemoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Title is required")]
        [BeginWIthAlphabeth(ErrorMessage ="Title must begin with alphabeth")]
        public string Title { get; set; }

        public string DeptName { get; set; }

        [Display(Name ="Department")]
        public List<SelectListItem> Departments { get; set; }

        public DateTime DateCreated { get; set; }
        public string DateCreatedString { get; set; }

        [Display(Name = "Memo file")]
        [DataType(DataType.Upload)]
        public IFormFile MemoFile { get; set; }
        public string MemoFilePath { get; set; }
    }


}
