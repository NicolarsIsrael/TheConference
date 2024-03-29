﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class Meeting
    {
        public int Id { get; set; }
        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public string Topic { get; set; }
        public string RoomName { get; set; }
        public MeetingType MeetingType { get; set; }
        public DateTime StartTime { get; set; }
    }

    public enum MeetingType
    {
        Admin, ZonalDirector, General, ExecutiveSecretary, Subeb
    }
}
