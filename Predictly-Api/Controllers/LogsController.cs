using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Logs;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

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
        public async Task<ActionResult<LogsViewModel>> GetInformations()
        {
            var logs =  await _context.Logs.Where(x => x.Level == "Information").Select(x => new LogsViewModel
            {
                Message = x.Message,
                Exception = x.Exception,
                TimeStamp = x.TimeStamp.ToString("dd/MM/yyyyy HH:mm:ss"),
            }).ToListAsync();

            return Ok(logs);
        }

        /// <summary>
        /// Get warning logs list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns warning logs list</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("warning")]
        public async Task<ActionResult<LogsViewModel>> GetWarnings()
        {
            var logs = await _context.Logs.Where(x => x.Level == "Warning").Select(x => new LogsViewModel
            {
                Message = x.Message,
                Exception = x.Exception,
                TimeStamp = x.TimeStamp.ToString("dd/MM/yyyyy HH:mm:ss"),
            }).ToListAsync();

            return Ok(logs);
        }

        /// <summary>
        /// Get error logs list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns error logs list</response>
        /// <response code="403">Forbidden</response>
        [HttpGet("error")]
        public async Task<ActionResult<LogsViewModel>> GetErrors()
        {
            var logs = await _context.Logs.Where(x => x.Level == "Error").Select(x => new LogsViewModel
            {
                Message = x.Message,
                Exception = x.Exception,
                TimeStamp = x.TimeStamp.ToString("dd/MM/yyyyy HH:mm:ss"),
            }).ToListAsync();

            return Ok(logs);
        }
    }
}
