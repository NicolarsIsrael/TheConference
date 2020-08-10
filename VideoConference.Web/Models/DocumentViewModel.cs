using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class RegisterDocumentViewModel
    {
        [BeginWIthAlphaNumeric(ErrorMessage = "Should begin with an alphabeth or number")]
        [Required(ErrorMessage ="Title is required")]
        public string Title { get; set; }

        public List<SelectListItem> Departments { get; set; }

        [BeginWIthAlphabeth(ErrorMessage = "Should begin with an alphabeth")]
        [Display(Name ="Submitted by")]
        [Required(ErrorMessage = "Submitted by is required")]
        public string SubmittedBy { get; set; }

        [BeginWIthAlphabeth(ErrorMessage = "Should begin with an alphabeth")]
        [Display(Name ="Rececived by")]
        [Required(ErrorMessage = "Received by is required")]
        public string RecievedBy { get; set; }

        [BeginWIthAlphaNumeric(ErrorMessage ="Should begin with an alphabeth or number")]
        [Required(ErrorMessage = "Remarks is required")]
        public string Remarks { get; set; }

        [BeginWIthAlphabeth(ErrorMessage = "Should begin with an alphabeth")]
        [Display(Name = "Addressed to")]
        [Required(ErrorMessage = "Addressed to is required")]
        public string AddressedTo { get; set; }
    }

    public class ViewDocumentViewModel
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public string Title { get; set; }
        public string CurrentOffice { get; set; }
        public bool CanMinute { get; set; }
        public IEnumerable<DocumentMinuteViewModel> DocMinutes { get; set; }
        public DateTime DateReceived { get; set; }
        public string DateReceivedString { get; set; }
        public string Remarks { get; set; }
        public string AddressedTo { get; set; }
    }

    public class DocumentMinuteViewModel
    {
        public DateTime DateCreated { get; set; }
        public string DateCreatedString { get; set; }
        public string FromDept { get; set; }
        public string ToDept { get; set; }
        public string SubmittedBy { get; set; }
        public string ReceivedBy { get; set; }
        public string Remarks { get; set; }
        public string AddressedTo { get; set; }
    }

    public class AddDocumentMinuteViewModel
    {
        public string DocTitle { get; set; }
        public int DocId { get; set; }
        public string DocNumber { get; set; }
        public int FromDeptId { get; set; }
        public string FromDept { get; set; }
        public int ToDeptId { get; set; }
        public string ToDept { get; set; }
        public List<SelectListItem> Departments { get; set; }

        [BeginWIthAlphabeth(ErrorMessage ="Should begin with an alphabeth")]
        [Display(Name = "Delivered by")]
        [Required(ErrorMessage = "Delivered by is required")]
        public string SubmittedBy { get; set; }

        [BeginWIthAlphabeth(ErrorMessage = "Should begin with an alphabeth")]
        [Display(Name = "Rececived by")]
        [Required(ErrorMessage = "Received by is required")]
        public string ReceivedBy { get; set; }

        [BeginWIthAlphaNumeric(ErrorMessage = "Should begin with an alphabeth or number")]
        [Required(ErrorMessage = "Remarks is required")]
        public string Remarks { get; set; }

        [BeginWIthAlphabeth(ErrorMessage = "Should begin with an alphabeth")]
        [Display(Name = "Addressed to")]
        [Required(ErrorMessage = "Addressed to is required")]
        public string AddressedTo { get; set; }

    }
}
