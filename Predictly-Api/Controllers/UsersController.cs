using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Predictly_Api.Models;
using Predictly_Api.ViewModels.User;

namespace Predictly_Api.Controllers
{
    [Authorize]
    [Route("user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<ApplicationUserModel> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult<IEnumerable<UserViewModel>> GetUsers(string role)
        {

            if (role != "Admin" && role != "Staff" && String.IsNullOrEmpty(role))
            {
                role = "";
            }
            try
            {
                var usersList = _userManager.Users.ToList();

                var users = usersList.Where(u => !u.DeleteStatus).Select(c => new UserViewModel
                {
                    Id = c.Id,
                    Username = c.UserName,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Gender = c.Gender,
                    Email = c.Email,
                    Role = string.Join(",", _userManager.GetRolesAsync(c).Result.ToArray()),
                    StudyData = _context.StudyData.Where(x => x.UserId == c.Id).FirstOrDefault()
            }).Where(c => c.Role == role).ToList();

                if (users.Count < 1)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(users);
                }
            }
            catch (Exception)
            {

                return BadRequest();
            }

        }

        [HttpGet("{id}")]
        public ActionResult<UserViewModel> GetUser(string id)
        {
            try
            {
                var user = _userManager.FindByIdAsync(id).Result;
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
                    StudyData = studyData
                });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutUser(string id, UpdateUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                var findUser = await _userManager.FindByIdAsync(id);

                if (findUser == null)
                {
                    return NotFound();
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
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
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

        [HttpGet("profile")]
        public async Task<ActionResult<UserViewModel>> GetProfileAsync()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(accessToken) as JwtSecurityToken;
                var id = token.Claims.First(claim => claim.Type == "nameid").Value;
                var user = _userManager.FindByIdAsync(id).Result;
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
                    StudyData = studyData
                });
            }
            catch (Exception)
            {

                return BadRequest();
            }
        }
    }
}
