﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class Meeting
    {
        public int Id { get; set; }
        public string GeneratedId { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
    }

}
