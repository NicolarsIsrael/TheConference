using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class DocumentMinute
    {
        public int Id { get; set; }
        public virtual Document Document { get; set; }
        public DateTime DateMinuted { get; set; }
        public virtual Department FromDepartment { get; set; }
        public virtual Department ToDepartment { get; set; }
        public string SubmittedBy { get; set; }
        public string RecievedBy { get; set; }
        public string UserId { get; set; }
        public string Remarks { get; set; }
        public string AddressedTo { get; set; }
    }
}
