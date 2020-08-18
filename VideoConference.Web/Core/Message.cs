using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public enum Urgency
    {
        [Display(Name ="Treat as urgent")]
        Urgent,
        [Display(Name="Not urgent")]
        NotUrgent
    }

    public class Message
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string MessageBody { get; set; }
        public string Title { get; set; }
        public string AttachmentPath { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public bool IsRead { get; set; }
        public Urgency Urgency { get; set; }
    }
}
