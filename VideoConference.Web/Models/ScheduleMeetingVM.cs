using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class ScheduleMeetingVM
    {
        [Required(ErrorMessage ="Topic is required")]
        public string Topic { get; set; }

        [Display(Name ="Start date")]
        [Required(ErrorMessage ="Start date is required")]
        public DateTime StartDate { get; set; }
    }
}
