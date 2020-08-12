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
        public const string ZonalDirectorAdminRole = "Zonal Director Admin";
        public const string UserRole = "User";
        public const string SubebRole = "SUBEB";
        public const string SubebAdminRole = "SUBEB Admin";

        public static readonly string[,] UserTypes =
        {
            { "1",UserRole},
            { "2",SecretaryRole},
            { "3",DeptAdminRole},
            { "4",ZonalDirectorRole},
            {"5",SubebRole },
            //{"5","Executive secretary" },
        };
    }
}
