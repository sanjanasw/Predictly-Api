using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Logs;

namespace Predictly_Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("logs")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public LogsController(ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get informatoin logs list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns information logs list</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("information")]
        public async Task<ActionResult<LogViewModel>> GetInformations()
        {
            var logs = _context.Logs.Where(x => x.Level == "Information").OrderByDescending(x => x.TimeStamp).
                Select(x => new LogViewModel
                {
                    Message = x.Message,
                    Exception = x.Exception,
                    TimeStamp = x.TimeStamp.ToString("HH:mm:ss dd/MM/yyyy"),
                }).ToListAsync();

            return Ok(await logs);
        }

        /// <summary>
        /// Get warning logs list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns warning logs list</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("warning")]
        public async Task<ActionResult<LogViewModel>> GetWarnings()
        {
            var logs = _context.Logs.Where(x => x.Level == "Warning").OrderByDescending(x => x.TimeStamp).
                Select(x => new LogViewModel
                {
                    Message = x.Message,
                    Exception = x.Exception,
                    TimeStamp = x.TimeStamp.ToString("HH:mm:ss dd/MM/yyyy"),
                }).ToListAsync();
            return Ok(await logs);
        }

        /// <summary>
        /// Get error logs list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns error logs list</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("error")]
        public async Task<ActionResult<LogViewModel>> GetErrors()
        {
            var logs = _context.Logs.Where(x => x.Level == "Error").OrderByDescending(x => x.TimeStamp).
                Select(x => new LogViewModel
                {
                    Message = x.Message,
                    Exception = x.Exception,
                    TimeStamp = x.TimeStamp.ToString("HH:mm:ss dd/MM/yyyy"),
                }).ToListAsync();

            return Ok(await logs);
        }
    }
}
