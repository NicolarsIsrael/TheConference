using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Core
{
    public class Memo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public string MemoFile { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
    }
}
