using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.School;

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

        /// <summary>
        /// Get schools list
        /// </summary>
        /// <response code="200">Returns schools list</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        public async Task<ActionResult<SchoolViewModel>> GetSchools()
        {
            try
            {

                var schools = _context.School.Select(x => new { x.Id, x.Name }).ToListAsync();
                if (schools == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Schools not found!" });
                }
                return Ok(await schools);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
