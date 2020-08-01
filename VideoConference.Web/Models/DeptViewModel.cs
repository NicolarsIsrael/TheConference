using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Models
{
    public class DeptViewModel
    {
        public IEnumerable<DeptAndIdViewModel> Department { get; set; }
    }

    public class DeptAndIdViewModel
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
    }
}
