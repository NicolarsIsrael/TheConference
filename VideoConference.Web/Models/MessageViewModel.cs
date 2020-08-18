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

    public class MessageViewModel
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string DateCreatedString { get; set; }
        public string Title { get; set; }
        public string MessageBody { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Urgency { get; set; }
        public string Attachment { get; set; }
        public bool HaveAttachment { get; set; }
        public bool IsRead { get; set; }
    }

    public class MessageIndexViewModel
    {
        public IEnumerable<MessageViewModel> ReceivedMessages { get; set; }
        public IEnumerable<MessageViewModel> SentMessages { get; set; }
    }
}
