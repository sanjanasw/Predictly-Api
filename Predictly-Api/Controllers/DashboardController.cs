using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.Dashboard;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("student-dashboard")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class DashboardController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPredictionService _predictionService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, ILogger<DashboardController> logger, IPredictionService predictionService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _predictionService = predictionService;
        }

        /// <summary>
        /// Get student dashboard data.
        /// </summary>
        /// <response code="200">Returns dashboard data</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        public async Task<ActionResult<StudentDashboardViewModel>> GetStudentDashboard()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = _userManager.FindByIdAsync(loggedInUserId);
                if (await loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                var goals =  _context.Goals.Where(x => x.UserId == loggedInUserId).ToList();
                var subjects =  _context.Subjects.ToList();
                var predictedResult = _context.PredictedResults.Where(x => x.UserId == loggedInUserId).ToList();
                var dashboardData = new StudentDashboardViewModel
                {
                    PredictedResult = _predictionService.GetStudentsOwnPredictions(predictedResult, subjects, goals)
                };

                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: dashboard/student.");
                throw;
            }
        }


    }
}
