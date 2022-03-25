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
using Predictly_Api.ViewModels.Dashboard;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("dashboard")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class DashboardController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get student dashboard data.
        /// </summary>
        /// <response code="200">Returns dashboard data</response>
        /// <response code="404">User not found</response>
        [HttpGet("student")]
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
                var preditedReault = _context.PredictedResults.Where(x => x.UserId == loggedInUserId).ToListAsync();
                var dashboardData = new StudentDashboardViewModel();
                var predictedResults = new List<PredictedResultViewModel>();
                foreach(var item in await preditedReault)
                {
                    var subjectGoal = goals.Where(x => x.SubjectId == item.SubjectId).FirstOrDefault();
                    string goal = null;
                    if(subjectGoal != null)
                    {
                        goal = subjectGoal.Goal.ToString();
                    }
                    predictedResults.Add(new PredictedResultViewModel
                    {
                        Subject = subjects.Where(x => x.Id == item.SubjectId).Select(x => x.Name).FirstOrDefault(),
                        Goal = goal,
                        Result = new ResultViewModel
                        {
                            A = item.A,
                            B = item.B,
                            C = item.C,
                            S = item.S,
                            W = item.W,
                        }
                    });
                }
                dashboardData.PredictedResult = predictedResults;

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
