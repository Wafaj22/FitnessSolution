using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FitnessSolution.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the FitnessSolutionUser class
    public class FitnessSolutionUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public DateTime DOB { get; set; }

        [PersonalData]
        public string Role { get; set; }
    }
}
