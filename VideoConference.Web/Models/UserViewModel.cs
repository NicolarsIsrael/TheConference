﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoConference.Web.Core;

namespace VideoConference.Web.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeptAdmin { get; set; }
        public string Dept { get; set; }
        public int DeptId { get; set; }
    }
}
