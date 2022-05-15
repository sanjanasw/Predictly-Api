using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.Goal;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("api/goals")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class GoalsController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, ILogger<GoalsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get students goals.
        /// </summary>
        /// <response code="200">Returns users goals</response>
        /// <response code="404">User not found</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalViewModel>>> GetGoals()
        {
            try
            {

                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
                }

                var userSubjects = _context.StudyData.Where(x => x.UserId == loggedInUserId).ToList();
                var userGoals = _context.Goals.Where(x => x.UserId == loggedInUserId).ToList();
                var subjects = _context.Subjects.ToList();
                var output = new List<GoalViewModel>();

                foreach (var usersubject in userSubjects)
                {
                    var userGoal = userGoals.Where(x => x.SubjectId == usersubject.SubjectId).FirstOrDefault();
                    output.Add(new GoalViewModel
                    {
                        Id = userGoal?.Id,
                        SubjectId = usersubject.SubjectId,
                        Goal = userGoal?.Goal,
                        Subject = subjects.Where(x => x.Id == usersubject.SubjectId).Select(x => x.Name).FirstOrDefault()
                    });
                }
                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: goal.");
                throw;
            }
        }

        /// <summary>
        /// Set new goals.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /goal
        ///     {
        ///        "subjectId": "2",
        ///        "goal: "A"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns goal Id</response>
        /// <response code="404">User not found</response>
        [HttpPost]
        public async Task<ActionResult<GoalModel>> PostGoalModel(GoalCreateViewModel goalModel)
        {
            try
            {

                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
                }

                var goal = new GoalModel
                {
                    Goal = goalModel.Goal,
                    SubjectId = goalModel.SubjectId,
                    UserId = loggedInUserId,
                };
                _context.Goals.Add(goal);
                await _context.SaveChangesAsync();
                _logger.LogInformation(string.Format("{0} is updated goal settings.", loggedInUser.UserName));
                return Ok(new { Id = goal.Id});
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occcured in POST: goal.");
                throw;
            }
        }

        /// <summary>
        /// Update goals.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /goal
        ///     {
        ///        "Id": "2",
        ///        "goal: "A"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="404">User not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel>> PutGoalModel(int id, GoalUpdateViewModel goalModel)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    var accessToken = HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                    var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                    var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                    if (loggedInUser == null)
                    {
                        return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
                    }

                    if (id != goalModel.Id)
                    {
                        return BadRequest(new ResponseModel { Status = "Error", Message = "Something went wrong!" });
                    }

                    var goal = await _context.Goals.Where(x => x.Id == goalModel.Id).FirstOrDefaultAsync();

                    if (goal == null)
                    {
                        return NotFound(new ResponseModel { Status = "Error", Message = "Goal not found!" });
                    }

                    if (goalModel.Goal == Results.W)
                    {
                        _context.Goals.Remove(goal);
                    }
                    else
                    {
                        goal.Goal = goalModel.Goal;
                        _context.Entry(goal).State = EntityState.Modified;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation(string.Format("{0} is updated goal settings.", loggedInUser.UserName));
                    return Ok(new ResponseModel { Status = "Success", Message = "Goal updated successfull!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in PUT: goal.");
                    throw;
                }
            }
        }

    }
}
