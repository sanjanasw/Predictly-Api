using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.User;

using Predictly_Api.Enums;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("user")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context, ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get user profile.
        /// </summary>
        /// <response code="200">Returns users profile</response>
        /// <response code="404">User not found</response>
        [HttpGet("profile")]
        public async Task<ActionResult<UserViewModel>> GetProfile()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }
 
                return Ok(new UserViewModel
                {
                    Id = loggedInUser.Id,
                    Username = loggedInUser.UserName,
                    FirstName = loggedInUser.FirstName,
                    LastName = loggedInUser.LastName,
                    Gender = loggedInUser.Gender,
                    Email = loggedInUser.Email,
                    SchoolId = loggedInUser.SchoolId,
                    OLYear = loggedInUser.OLYear,
                    Role = string.Join(",", _userManager.GetRolesAsync(loggedInUser).Result.ToArray()),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/profile.");
                throw;
            }
        }

        /// <summary>
        /// Get Students list according to school id. [Access: Admins and Staff only]
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Returns users list according to selected role</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Requested user not found</response>
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("student")]
        public async Task<ActionResult<StudentViewModel>> GetStudents()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" }); ;
                }

                var usersList = await _userManager.Users.ToListAsync();
                var users = new List<StudentViewModel>();
                users = usersList.Where(u => !u.DeleteStatus && u.SchoolId == loggedInUser.SchoolId).Select(c => new StudentViewModel
                {
                    Id = c.Id,
                    Username = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Gender = c.Gender,
                    Email = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray()),
                    OLYear = c.OLYear,
                    SchoolId = c.SchoolId,
                }).Where(c => c.Role.ToLower() == "").ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/student.");
                throw;
            }

        }

        /// <summary>
        /// Get students current status.
        /// </summary>
        /// <response code="200">Returns users profile</response>
        /// <response code="404">User not found</response>
        [HttpGet("student/current-status")]
        public async Task<ActionResult<CurrentStatusViewModel>> GetCurrentStatus()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                var currentStatus = _context.StudyData.Where(x => x.UserId == loggedInUserId).Select(x => new CurrentStatusViewModel
                {
                    Id = x.Id,
                    SubjectId = x.SubjectId,
                    Commitment = x.Commitment,
                    ClassStatus = x.ClassStatus,
                    AvgMarks = x.AvgMarks,
                }).ToList();

                var subjectsList = _context.Subjects.ToList();
                foreach (var item in currentStatus)
                {
                    var subjectInfo = subjectsList.Where(x => x.Id == item.SubjectId).Select(y => new {y.Name , y.BucketType}).FirstOrDefault();
                    item.Subject = subjectInfo.Name;
                    item.BucketType = subjectInfo.BucketType;
                }

                return Ok(currentStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/student/current-status.");
                throw;
            }
        }

        /// <summary>
        /// Get students bucket subject status.
        /// </summary>
        /// <response code="200">Returns users profile</response>
        /// <response code="404">User not found</response>
        [HttpGet("student/buckets-status")]
        public async Task<ActionResult<BucketsStatusViewModel>> GetBucketsStatus()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                var bucketStatus = new BucketsStatusViewModel
                {
                    Bucket1 = loggedInUser.BSub1 != 0,
                    Bucket2 = loggedInUser.BSub2 != 0,
                    Bucket3 = loggedInUser.BSub3 != 0
                };

                return Ok(bucketStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/student/current-status.");
                throw;
            }
        }

        /// <summary>
        /// Get staff members list. [Access: Admins and Staff only]
        /// </summary>
        /// <response code="200">Returns staff members list</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Requested user not found</response>
        [Authorize(Roles = "Admin, Staff")]
        [HttpGet("staff")]
        public async Task<ActionResult<StaffViewModel>> GetStaff()
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                if (loggedInUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                var usersList = await _userManager.Users.ToListAsync();
                var users = new List<StaffViewModel>();
                users = usersList.Where(u => !u.DeleteStatus && u.SchoolId == loggedInUser.SchoolId).Select(c => new StaffViewModel
                {
                    Id = c.Id,
                    Username = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Gender = c.Gender,
                    Email = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray()),
                    isActive = c.EmailConfirmed,
                    SchoolId = c.SchoolId,
                }).Where(c => c.Role == UserRoles.Staff.ToString()).ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/staff.");
                throw;
            }

        }

        /// <summary>
        /// Get admin members list. [Access: Admins only]
        /// </summary>
        /// <response code="200">Returns admin members list</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Requested user not found</response>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<ActionResult<AdminViewModel>> GetAdmins()
        {
            try
            {
                var usersList = await _userManager.Users.ToListAsync();
                var users = new List<AdminViewModel>();
                users = usersList.Where(u => !u.DeleteStatus).Select(c => new AdminViewModel
                {
                    Id = c.Id,
                    Username = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Gender = c.Gender,
                    Email = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray()),
                    isActive = c.EmailConfirmed,
                }).Where(c => c.Role == UserRoles.Admin.ToString()).ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/staff.");
                throw;
            }

        }

        /// <summary>
        /// Update user data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /user/1
        ///     {
        ///         "id": 1,
        ///         "username": "sanjana",
        ///         "firstName": "sanjana",
        ///         "lastName": "witharanage",
        ///         "email": "sanajnasw99@gmai.com",
        ///         "gender": 0,
        ///         "schoolId": 1,
        ///         "OL Year"?: 0,
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns updated user data</response>
        /// <response code="400">User ids not matching</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(string id, UpdateUserViewModel model)
        {

            var accessToken = HttpContext.GetTokenAsync("access_token");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
            var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

            var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
            if (loggedInUser == null)
            {
                return NotFound(new ResponseModel { Status = "Error", Message = "Logged in user not found!" });
            }

            if (id != model.Id)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = "Something went wrong!" });
            }

            try
            {
                var findUser = await _userManager.FindByIdAsync(id);

                if (findUser == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }

                findUser.UserName = model.Username;
                findUser.FirstName = model.FirstName;
                findUser.LastName = model.LastName;
                findUser.Gender = model.Gender;
                findUser.Email = model.Email;
                findUser.OLYear = model.OLYear;
                findUser.SchoolId = model.SchoolId;

                var result = await _userManager.UpdateAsync(findUser);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByIdAsync(id);

                    if (loggedInUser.Id == user.Id)
                    {
                        _logger.LogInformation(string.Format("{0} is updated own user info.", user.UserName));
                    }
                    else
                    {
                        _logger.LogInformation(string.Format("{0} is updated user info of {1}.", loggedInUser.UserName, user.UserName));
                    }

                    return Ok(new
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Email = user.Email,
                        OLYear = user.OLYear,
                        SchoolId = user.SchoolId,
                        Role = string.Join(",", _userManager.GetRolesAsync(findUser).Result.ToArray())
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User data updating failed!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in GET: user/id.");
                throw;
            }
        }

        /// <summary>
        /// Delete user. [Access: Admins and Staff only]
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Cannot delete own account or school creators account</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [Authorize(Roles = "Admin, Staff")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
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

                if (loggedInUserId == id)
                {
                    _logger.LogWarning(string.Format("{0} is tried to delete own account.", loggedInUser.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Cannot delete own account!" });
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }
                var userRole = await _userManager.GetRolesAsync(user);
                if (userRole[0] == UserRoles.Staff.ToString())
                {
                    var isBaseUser = await _context.School.Where(x => x.StaffUserId == user.Id).AnyAsync();
                    if (isBaseUser)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to delete school creators acccount ({1}).", loggedInUser.UserName, user.UserName));
                        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Cannot delete school creators account!" });
                    }
                }
                user.DeleteStatus = true;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation(string.Format("{0} is deleted {1}'s account.", loggedInUser.UserName, user.UserName));
                    return Ok(new ResponseModel { Status = "Success", Message = "User delete successfully!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User deletion failed!" });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in DELETE: user/id.");
                throw;
            }
        }

        /// <summary>
        /// Add study data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /study-data
        ///      {
        ///         "subjectId": 1,
        ///         "commitment": 0,
        ///         "classsStatus": 2,
        ///         "avgMarks" :75
        ///      }
        ///
        /// </remarks>
        /// <response code="200">Returns newly created subject</response>
        /// <response code="400">Selected subject invalid</response>
        /// <response code="404">User not found</response>
        [HttpPost("study-data")]
        public async Task<ActionResult> AddStudyData(StudyDataInsertViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var subjectInfo = _context.Subjects.Where(x => x.Id == model.SubjectId).Select(x => new { x.BucketType, x.Name }).FirstOrDefault();

                    var accessToken = HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                    var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                    var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                    if (loggedInUser == null)
                    {
                        return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                    }

                    switch (subjectInfo.BucketType)
                    {
                        case 1:
                            loggedInUser.BSub1 = model.SubjectId;
                            break;
                        case 2:
                            loggedInUser.BSub2 = model.SubjectId;
                            break;
                        case 3:
                            loggedInUser.BSub3 = model.SubjectId;
                            break;
                        default:
                            return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Selected subject is invalid!" });
                    }

                    await _userManager.UpdateAsync(loggedInUser);
                    var studyData = new StudyDataModel
                    {
                        SubjectId = model.SubjectId,
                        Commitment = model.Commitment,
                        AvgMarks = model.AvgMarks,
                        ClassStatus = model.ClassStatus,
                        UserId = loggedInUser.Id
                    };
                    _context.StudyData.Add(studyData);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var currentStatus = new CurrentStatusViewModel
                    {
                        BucketType = subjectInfo.BucketType,
                        SubjectId = model.SubjectId,
                        Subject = subjectInfo.Name,
                        AvgMarks = model.AvgMarks,
                        ClassStatus = model.ClassStatus,
                        Commitment = model.Commitment,
                        Id = studyData.Id,
                    };

                    _logger.LogInformation(string.Format("{0} is added study data.", loggedInUser.UserName));
                    return Ok(currentStatus);

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in POST: user/study-data.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Update study data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /study-data
        ///      {
        ///         "id": 1,
        ///         "subjectId": 1,
        ///         "commitment": 0,
        ///         "classsStatus": 2,
        ///         "avgMarks" :75
        ///      }
        ///
        /// </remarks>
        /// <response code="200">Returns success Message</response>
        /// <response code="400">Study data ids not matching</response>
        /// <response code="404">User not found</response>
        [HttpPut("study-data/{id}")]
        public async Task<ActionResult> PutStudyData(StudyDataUpdateViewModel model, int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (id != model.Id)
                    {
                        return BadRequest(new ResponseModel { Status = "Error", Message = "Something went wrong!" });
                    }

                    var accessToken = HttpContext.GetTokenAsync("access_token");
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                    var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;

                    var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);
                    if (loggedInUser == null)
                    {
                        return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                    }

                    var updateData = new StudyDataModel
                    {
                        Id = model.Id,
                        SubjectId = model.SubjectId,
                        AvgMarks = model.AvgMarks,
                        ClassStatus = model.ClassStatus,
                        Commitment = model.Commitment,
                        UserId = loggedInUserId
                    };

                    _context.Entry(updateData).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation(string.Format("{0} is updated study data.", loggedInUser.UserName));
                    return Ok(new ResponseModel { Status = "Success", Message = "Study data update successfully!" });

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in PUT: user/study-data.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Delete study data [Bucket Subject Data].
        /// </summary>
        /// <response code="200">Returns success Message</response>
        /// <response code="400">Cannot remove core subject</response>
        /// <response code="404">User not found</response>
        [HttpDelete("study-data/{id}")]
        public async Task<ActionResult> DeleteStudyData(int id)
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
                        return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                    }

                    var studyData = _context.StudyData.Where(x => x.Id == id).FirstOrDefault();
                    var subjectInfo = _context.Subjects.Where(x => x.Id == studyData.SubjectId)
                        .Select(x => new { x.BucketType, x.Name }).FirstOrDefault();
                    switch (subjectInfo.BucketType)
                    {
                        case 1:
                            loggedInUser.BSub1 = 0;
                            break;
                        case 2:
                            loggedInUser.BSub2 = 0;
                            break;
                        case 3:
                            loggedInUser.BSub3 = 0;
                            break;
                        default:
                            return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Cannot remove core subjects!" });
                    }

                    _context.StudyData.Remove(studyData);

                    var goal = _context.Goals.Where(x => x.SubjectId == studyData.SubjectId && x.UserId == loggedInUserId).FirstOrDefault();
                    if (goal != null)
                        _context.Goals.Remove(goal);

                    await _userManager.UpdateAsync(loggedInUser);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation(string.Format("{0} is deleted study data.", loggedInUser.UserName));
                    return Ok(new ResponseModel { Status = "Success", Message = "Study data deleted successfully!" });

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in DELETE: user/study-data.");
                    throw;
                }
            }
        }
    }
}
