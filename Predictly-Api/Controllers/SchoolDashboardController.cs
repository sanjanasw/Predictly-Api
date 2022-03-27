using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.SchoolDashboard;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("school-dashboard")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class SchoolDashboardController : Controller
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IPredictionService _predictionService;

        public SchoolDashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, 
            IPredictionService predictionService)
        {
            _userManager = userManager;
            _context = context;
            _predictionService = predictionService;

        }

        [Authorize(Roles ="Staff")]
        [HttpGet]
        public async Task<ActionResult<SchoolDashboardViewModel>> GetDashboardPredictions()
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
            var students = _context.Users.Where(x => x.SchoolId == loggedInUser.SchoolId && x.OLYear > 0).ToList();
            var results = _context.PredictedResults.Where(x => students.Select(x => x.Id).Contains(x.UserId)).ToList();
            var prediction = _predictionService.GetSchoolStudentsPredictions(results, subjects);
            var studyData = _context.StudyData.Where(x => students.Select(x => x.Id).Contains(x.UserId))
                .Select(x => new SubjectClassStatusViewModel { SubjectId = x.SubjectId, ClassStatus = x.ClassStatus}).ToList();

            var subjectClass = studyData.GroupBy(x => x.SubjectId).Select(y => new { y.Key, count = y.Count() }).ToList();

            var output = new SchoolDashboardViewModel()
            {
                ResultPrediction = prediction,
                GenderDistribution = new SchoolGenderDistributionViewModel()
                {
                    Male = students.Where(x => x.Gender == Genders.Male).Count(),
                    Female = students.Where(x => x.Gender == Genders.Female).Count(),
                }

            };
            return Ok(output);
        }
    }
}
