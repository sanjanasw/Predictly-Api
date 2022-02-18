using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Predictly_Api.Enums;
using System.Net.Mime;

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

        public UsersController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Update user data.
        /// </summary>
        /// <remarks>
        /// <param name="id"></param>
        /// <param name="model"></param>
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
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete user. [Access: Admins only]
        /// </summary>
        /// <remarks>
        /// <param name="id"></param>
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Vaccination centers must have at least one staff member</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" });
                }
                user.DeleteStatus = true;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new ResponseModel { Status = "Success", Message = "User delete successfully!" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User deletion failed!" });
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get user profile.
        /// </summary>
        /// <response code="200">Returns users profile</response>
        /// <response code="404">User not found</response>
        [HttpGet("profile")]
        public async Task<ActionResult<UserViewModel>> GetProfileAsync()
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

                var studyData = _context.StudyData.Where(x => x.UserId == id).FirstOrDefault();
                return Ok(new UserViewModel
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    Email = user.Email,
                    SchoolId = user.SchoolId,
                    OLYear = user.OLYear,
                    Role = string.Join(",", _userManager.GetRolesAsync(user).Result.ToArray()),
                });
            }
            catch (Exception)
            {

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
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                var id = token.Claims.First(claim => claim.Type == "nameid").Value;

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ResponseModel { Status = "Error", Message = "User not found!" }); ;
                }

                var usersList = _userManager.Users.ToList();
                var users = new List<StudentViewModel>();
                users = usersList.Where(u => !u.DeleteStatus && u.SchoolId == user.SchoolId).Select(c => new StudentViewModel
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
            catch (Exception)
            {

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
        public async Task<ActionResult<StafftViewModel>> GetStaff()
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

                var usersList = _userManager.Users.ToList();
                var users = new List<StafftViewModel>();
                users = usersList.Where(u => !u.DeleteStatus && u.SchoolId == user.SchoolId).Select(c => new StafftViewModel
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
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Update study data.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /study-data
        ///     "studyData": {
        ///         "sub1Hours": 0,
        ///         "sub1Class": true,
        ///         "sub1AvgMarks": 45,
        ///         "sub2Hours": 1,
        ///         "sub2Class": true,
        ///         "sub2AvgMarks": 0,
        ///         "sub3Hours": 3,
        ///         "sub3Class": false,
        ///         "sub3AvgMarks": 67,
        ///         "sub4Hours": 0,
        ///         "sub4Class": true,
        ///         "sub4AvgMarks": 56,
        ///         "sub5Hours": 1,
        ///         "sub5Class": false,
        ///         "sub5AvgMarks": 46,
        ///         "sub6Hours": 2,
        ///         "sub6Class": true,
        ///         "sub6AvgMarks": 98,
        ///         "sub7Hours": 2,
        ///         "sub7Class": true,
        ///         "sub7AvgMarks": 78,
        ///         "sub8Hours": 4,
        ///         "sub8Class": false,
        ///         "sub8AvgMarks": 56,
        ///         "sub9Hours": 0,
        ///         "sub9Class": true,
        ///         "sub9AvgMarks": 53
        ///      }
        ///
        /// </remarks>
        /// <response code="200">Returns updated user data</response>
        /// <response code="404">User not found</response>
        [HttpPut("study-data")]
        public async Task<ActionResult> PutStudyData(StudyDataModel model)
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

                model.UserId = user.Id;

                _context.Entry(model).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new ResponseModel { Status = "Success", Message = "Study data update successfully!" });

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
