using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Predictly_Api.Models;
using Predictly_Api.Services;
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
        private readonly ILogger<SchoolDashboardController> _logger;

        public SchoolDashboardController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, 
            ILogger<SchoolDashboardController> logger, IPredictionService predictionService)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
            _predictionService = predictionService;

        }

        [Authorize(Roles ="Staff")]
        [HttpGet]
        public async Task<IActionResult> GetDashboardPredictions()
        {
            var accessToken = HttpContext.GetTokenAsync("access_token");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
            var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;
            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
            if (loggedInUser == null)
            {
                return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
            }

            var students = _context.Users.Where(x => x.SchoolId == loggedInUser.SchoolId).Select(x => x.Id).ToList();
            var results = _context.PredictedResults.Where(x => students.Contains(x.UserId)).ToList();

            var prediction = _predictionService.GetSchoolStudentsPredictions(results);
            return Ok();
        }
    }
}
