using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Predictly_Api.ViewModels.Authentication;

namespace Predictly_Api.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUserModel>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<StudyDataModel> StudyData { get; set; }
        public DbSet<SchoolModel> School { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
    }
}
