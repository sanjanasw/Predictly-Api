using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Subject;

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

        /// <summary>
        /// Get subjects list
        /// </summary>
        /// <response code="200">Returns subjects list</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        public async Task<ActionResult<SubjectViewModel>> GetSchools()
        {
            try
            {

                var core = _context.Subjects.Where(x => x.BucketType == 0).Select(x => new SubjectDataViewModel { Id = x.Id, Name = x.Name }).ToList();
                if (core == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Core subjects not found!" });
                }
                var B1 = _context.Subjects.Where(x => x.BucketType == 1).Select(x => new SubjectDataViewModel { Id = x.Id, Name = x.Name }).ToList();
                if (B1 == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Bucket 1 subjects not found!" });
                }
                var B2 = _context.Subjects.Where(x => x.BucketType == 2).Select(x => new SubjectDataViewModel { Id = x.Id, Name = x.Name }).ToList();
                if (B2 == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Bucket 2 subjects not found!" });
                }
                var B3 = _context.Subjects.Where(x => x.BucketType == 3).Select(x => new SubjectDataViewModel { Id = x.Id, Name = x.Name }).ToList();
                if (B3 == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Bucket 3 subjects not found!" });
                }
                return Ok(new SubjectViewModel { CoreSubjects =  core, Bucket1 =  B1, Bucket2 =  B2, Bucket3 =  B3 });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
