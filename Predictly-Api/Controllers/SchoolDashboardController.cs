using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.SchoolDashboard;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("api/school-dashboard")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class SchoolDashboardController : Controller
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPredictionAnalizingService _predictionAnalizingService;

        public SchoolDashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context,
            IPredictionAnalizingService predictionAnalizingService)
        {
            _userManager = userManager;
            _context = context;
            _predictionAnalizingService = predictionAnalizingService;

        }

        /// <summary>
        /// Get schools dashboard data. [Access: Staff only]
        /// </summary>
        /// <response code="200">Returns dashboard data</response>
        /// <response code="404">User not found</response>
        [Authorize(Roles = "Staff")]
        [HttpGet("{year}")]
        public async Task<ActionResult<SchoolDashboardViewModel>> GetDashboardPredictions([FromRoute]int year)
        {
            var accessToken = HttpContext.GetTokenAsync("access_token");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
            var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
            if (loggedInUser == null)
            {
                return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
            }
            var subjects = _context.Subjects.ToList();
            var students = _context.Users.Where(x => x.SchoolId == loggedInUser.SchoolId && x.OLYear == year).ToList();
            var results = _context.PredictedResults.Where(x => students.Select(x => x.Id).Contains(x.UserId)).ToList();
            var studyData = _context.StudyData.Where(x => students.Select(x => x.Id).Contains(x.UserId))
                .Select(x => new SubjectClassStatusViewModel { SubjectId = x.SubjectId, ClassStatus = x.ClassStatus }).ToList();

            var output = new SchoolDashboardViewModel()
            {
                ResultPrediction = _predictionAnalizingService.GetSchoolStudentsPredictions(results, subjects),
                GenderDistribution = new List<SchoolGenderDistributionViewModel>()
                {
                  new SchoolGenderDistributionViewModel(){
                      Name = "Male",
                      Value = students.Where(x => x.Gender == Genders.Male).Count(),
                  },
                  new SchoolGenderDistributionViewModel(){
                      Name = "Female",
                      Value = students.Where(x => x.Gender == Genders.Female).Count(),
                  }
                },
                ClassStatus = _predictionAnalizingService.GetClassStatus(studyData, subjects),

            };
            return Ok(output);
        }

        /// <summary>
        /// Get Accessible O/L years list. [Access: Staff only]
        /// </summary>
        /// <response code="200">Returns years</response>
        /// <response code="400">No years in db</response>
        [Authorize(Roles = "Staff")]
        [HttpGet("years-list")]
        public ActionResult<List<int>> GetStudentsOLYears()
        {
            var years = _context.Users.Where(x => x.OLYear > 0).Select(x => x.OLYear).Distinct().ToList();
            if (years.Count() > 0)
            {
                return Ok(years);
            }
            return BadRequest(new ResponseModel { Status = "Error", Message = "Something went wrong!" });
        }
    }
}
