using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Subject;
using System.Collections.Generic;
using System.Linq;

namespace Predictly_Api.Controllers
{
    [Route("subject")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SubjectViewModel>> GetSchools()
        {
            var core = _context.Subjects.Where(x => x.BucketType ==0).Select(x => new SubjectModel { Id = x.Id, Name = x.Name }).ToList();
            var B1 = _context.Subjects.Where(x => x.BucketType == 1).Select(x => new SubjectModel { Id = x.Id, Name = x.Name }).ToList();
            var B2 = _context.Subjects.Where(x => x.BucketType == 2).Select(x => new SubjectModel { Id = x.Id, Name = x.Name }).ToList();
            var B3 = _context.Subjects.Where(x => x.BucketType == 3).Select(x => new SubjectModel { Id = x.Id, Name = x.Name }).ToList();
            return Ok(new SubjectViewModel { CoreSubjects = core, Bucket1 = B1, Bucket2 = B2, Bucket3 = B3 });
        }
    }
}
