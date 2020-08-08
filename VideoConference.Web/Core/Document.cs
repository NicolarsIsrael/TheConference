using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class Document
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public string Title { get; set; }
        public virtual Department CurrentDepartment { get; set; }
        public string SubmittedBy { get; set; }
        public string RecievedBy { get; set; }
        public DateTime DateReceived { get; set; }
        public string Remarks { get; set; }
        public virtual IEnumerable<DocumentMinute> DocumentMinutes { get; set; }
    }
}
