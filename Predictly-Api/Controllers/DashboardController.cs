using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Dashboard;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

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

        // TODO: Remove with db integration
        private List<PredictedResultModel> MockPredictedResult = new List<PredictedResultModel>();

        public DashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;

            // TODO: Remove with db integration
            MockPredictedResult.Add(new PredictedResultModel
            {
              UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
              SubjectId = 2,
              A = 18.5,
              B = 83.5,
              C = 5.5,
              S = 1.3,
              W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 3,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 4,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 5,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 6,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 7,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });

            MockPredictedResult.Add(new PredictedResultModel
            {
                UserId = "79949b2b-4752-47f6-9fab-a116e7590881",
                SubjectId = 8,
                A = 18.5,
                B = 83.5,
                C = 5.5,
                S = 1.3,
                W = 0.5,
            });
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
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                var id = token.Claims.First(claim => claim.Type == "nameid").Value;

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                var goals = await _context.Goals.Where(x => x.UserId == id).ToListAsync();
                var subjects = await _context.Subjects.ToListAsync();
                var dashboardData = new StudentDashboardViewModel();
                var predictedResults = new List<PredictedResultViewModel>();
                foreach(var item in MockPredictedResult)
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
                        A = item.A,
                        B = item.B,
                        C = item.C,
                        S = item.S,
                        W = item.W,
                        Goal = goal,
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
