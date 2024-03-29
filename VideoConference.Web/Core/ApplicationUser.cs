﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base() { }
        public string Name { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }

    }
}
