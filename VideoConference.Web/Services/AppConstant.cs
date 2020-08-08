using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Services
{
    public class AppConstant
    {
        public const string AdminRole = "Admin";
        public const string ESRole = "Executive secretary";
        public const string DeptAdminRole = "Department Admin";
        public const string SecretaryRole = "Secretary";
        public const string ZonalDirectorRole = "Zonal Director";
        public const string UserRole = "User";

        public static readonly string[,] UserTypes =
        {
            { "1","User"},
            { "2","Secretary"},
            { "3","Department Admin"},
            { "4","Zonal Director"},
            //{"5","Executive secretary" },
        };
    }
}
