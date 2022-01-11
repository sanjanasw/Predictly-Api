using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Predictly_Api.Enums;

namespace Predictly_Api.Models
{
    public class ApplicationUserModel : IdentityUser
    {

        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public Genders Gender { get; set; }

        [PersonalData]
        public int OLYear { get; set; } = 0;

        public int SchoolId { get; set; } = 0;

        public int BSub1 { get; set; } = 0;

        public int BSub2 { get; set; } = 0;

        public int BSub3 { get; set; } = 0;

        public bool DeleteStatus { get; set; } = false;
        public EducationLevels FathersEduLevel { get; set; } = EducationLevels.Phd;

        public EducationLevels MothersEduLevel { get; set; } = EducationLevels.Phd;

    }
}
