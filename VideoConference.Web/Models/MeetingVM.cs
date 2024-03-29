﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class ScheduleMeetingVM
    {
        public int Id { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        [BeginWIthAlphabeth(ErrorMessage ="Topic should begin with an alphabeth")]
        [Required(ErrorMessage ="Topic is required")]
        public string Topic { get; set; }

        [Display(Name ="Start date")]
        [Required(ErrorMessage ="Start date is required")]
        public DateTime StartDate { get; set; }
        public bool CanJoin { get; set; }
        public string StartDateString { get; set; }
        public string RoomName { get; set; }
        public string AnonymousLink { get; set; }
        public IEnumerable<DeptAndIdViewModel> Departments { get; set; }
        public List<SelectListItem> SelectDepts { get; set; }
    }



}
