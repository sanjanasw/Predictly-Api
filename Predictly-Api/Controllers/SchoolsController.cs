using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.School;
using System.Collections.Generic;
using System.Linq;

namespace Predictly_Api.Controllers
{
    [Route("school")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SchoolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<SchoolViewModel>> GetSchools()
        {
            var schools = _context.School.Select(x  => new {x.Id, x.Name}).ToList();
            return Ok(schools);
        }
    }
}
