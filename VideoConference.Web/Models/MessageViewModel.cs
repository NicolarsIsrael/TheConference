using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VideoConference.Web.Core;

namespace VideoConference.Web.Models
{
    public class AddMessageViewModel
    {
        [Required(ErrorMessage ="Message body is required")]
        [BeginWIthAlphaNumeric(ErrorMessage ="Should begin with alphabeth or number")]
        [Display(Name ="Message body")]
        public string MessageBody { get; set; }

        [Display(Name ="To")]
        public List<SelectListItem> ToEmailList { get; set; }

        [Required(ErrorMessage ="Title is required")]
        [BeginWIthAlphaNumeric(ErrorMessage ="Should begin with alphabeth or number")]
        public string Title { get; set; }

        public Urgency Urgency { get; set; }

        [Display(Name = "Attachment file")]
        [DataType(DataType.Upload)]
        public IFormFile AttachmentFile { get; set; }
    }
}
