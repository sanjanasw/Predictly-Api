using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Predictly_Api.Models;
using Predictly_Api.Enums;
using Predictly_Api.ViewModels.Authentication;
using Predictly_Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Predictly_Api.Controllers
{
    [AllowAnonymous]
    [Route("auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public AuthenticateController(UserManager<ApplicationUserModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && !user.DeleteStatus && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    id = user.Id,
                    name = user.FirstName + ' ' + user.LastName,
                    username = user.UserName,
                    email = user.Email,
                    school = user.SchoolId,
                    role = userRoles,
                });
            }
            return Unauthorized(new ResponseModel { Status = "401", Message = "Username or password incorrec!" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.UserInfo.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(model.UserInfo.Email);
                if (userExists != null || userEmailExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

                ApplicationUserModel user = new()
                {
                    UserName = model.UserInfo.Username,
                    FirstName = model.UserInfo.FirstName,
                    LastName = model.UserInfo.LastName,
                    Email = model.UserInfo.Email,
                    Gender = model.UserInfo.Gender,
                    SchoolId = model.UserInfo.SchoolId,
                    FathersEduLevel = model.UserInfo.FathersEduLevel,
                    MothersEduLevel = model.UserInfo.MothersEduLevel,
                    BSub1 = model.UserInfo.BSub1,
                    BSub2 = model.UserInfo.BSub2,
                    BSub3 = model.UserInfo.BSub3,
                    OLYear = model.UserInfo.OLYear,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };
                var result = await _userManager.CreateAsync(user, model.UserInfo.Password);

                model.StudyData.UserId = user.Id;

                _context.StudyData.Add(model.StudyData);
                _context.SaveChanges();

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                string confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                sendEmail("verify", null, user, confirmationToken);
                return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("school-register")]
        public async Task<IActionResult> SchoolRegister([FromBody] SchoolRegisterViewModel model)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(model.UserInfo.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(model.UserInfo.Email);
                if (userExists != null || userEmailExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

                ApplicationUserModel user = new()
                {
                    UserName = model.UserInfo.Username,
                    FirstName = model.UserInfo.FirstName,
                    LastName = model.UserInfo.LastName,
                    Email = model.UserInfo.Email,
                    Gender = model.UserInfo.Gender,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, model.UserInfo.Password);

                if (!await _roleManager.RoleExistsAsync(UserRoles.Staff.ToString()))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff.ToString()));
                if (await _roleManager.RoleExistsAsync(UserRoles.Staff.ToString()))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Staff.ToString());
                }

                SchoolModel school = new()
                {
                    StaffUserId = user.Id,
                    Name = model.SchoolInfo.Name,
                    Address = model.SchoolInfo.Address
                };

                _context.School.Add(school);
                _context.SaveChanges();

                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                string confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                sendEmail("verify", null, user, confirmationToken);
                return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail(ConfirmEmailViewModel model)
        {
            ApplicationUserModel user = _userManager.FindByIdAsync(model.Userid).Result;
            IdentityResult result = _userManager.ConfirmEmailAsync(user, model.Token).Result;
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
            }

            sendEmail("verified", user.Email, null, null);
            return Ok(new ResponseModel { Status = "Success", Message = "Verification successful, you can now login" });
        }

        [HttpPost("forgot-Password")]
        public IActionResult ForgotPassword(ForgetPasswordViewModel model)
        {
            ApplicationUserModel user = _userManager.FindByEmailAsync(model.Email).Result;

            if (user == null || !(_userManager.IsEmailConfirmedAsync(user).Result))
            {
                return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
            }

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
            sendEmail("resetPass", null, user, token);
            return Ok(new ResponseModel { Status = "Success", Message = "Reset Password Link Sent!" });
        }

        [HttpPost("reset-Password")]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            ApplicationUserModel user = _userManager.FindByIdAsync(model.Userid).Result;
            IdentityResult result = _userManager.ResetPasswordAsync(user, model.Token, model.Password).Result;

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
            }

            sendEmail("resetted", user.Email, null, null);
            return Ok(new ResponseModel { Status = "Success", Message = "Password Reset Successfull!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("admin-account-create")]
        public async Task<IActionResult> NewUser([FromBody] NewUserViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            var userEmailExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null || userEmailExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            ApplicationUserModel user = new()
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                SchoolId = 0,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, "$NewUserPassword1Temp");
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                }
            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

            sendEmail("newUser", null, user, token);
            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("new-user-setup")]
        public IActionResult NewUserSetup(ResetPasswordViewModel model)
        {
            ApplicationUserModel user = _userManager.FindByIdAsync(model.Userid).Result;
            IdentityResult result = _userManager.ResetPasswordAsync(user, model.Token, model.Password).Result;
            user.EmailConfirmed = true;
            _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
            }

            sendEmail("newUserSetup", user.Email, null, null);
            return Ok(new ResponseModel { Status = "Success", Message = "New User Setup Successfull!" });
        }

        private void sendEmail(string _type, string? _email, ApplicationUserModel? _user, string? _token)
        {
            string message;
            string subject = "";
            string html = "";
            string verifyUrl;
            string email = _user != null ? _user.Email : _email;

            try
            {
                switch (_type)
                {
                    case "verify":
                        verifyUrl = $"http://localhost:4200/auth/confirm-email?userid={_user.Id}&token={_token}";
                        message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
                        subject = "Predictly Signup - Verify Email";
                        html = $@"<h4>Verify Email</h4>
                         {message}";
                        break;
                    case "resetPass":
                        verifyUrl = $"http://localhost:4200/auth/reset-password?userid={_user.Id}&token={_token}";
                        message = $@"<p>Please click the below link to reset your password:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
                        subject = "Predictly - Reset password";
                        html = $@"<h4>Reset Password</h4>
                         {message}";
                        break;
                    case "verified":
                        message = $@"<p>Verification Successfull!</p>";
                        subject = "Predictly Signup";
                        html = $@"<h4>Verify Email</h4>
                         {message}";
                        break;
                    case "resetted":
                        message = $@"<p>Reset Password Successfull!</p>";
                        subject = "Predictly Password Reset";
                        html = $@"<h4>Password Reset</h4>
                         {message}";
                        break;
                    case "newUser":
                        verifyUrl = $"{Request.Headers["origin"]}/account/reset-password?userid={_user.Id}&token={_token}";
                        message = $@"<p>Please click the below link to set password to your account:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
                        subject = "Predictly - New User Invitation";
                        html = $@"<h4>New User Setup</h4>
                         {message}";
                        break;
                    case "newUserSetup":
                        message = $@"<p>Password Setup Successfull!</p>";
                        subject = "Predictly Password Setup";
                        html = $@"<h4>Welcome TO the Vaccination Management System</h4>
                         {message}";
                        break;

                }

                _emailService.Send(
                    to: email,
                    subject: subject,
                    html: html
                );

            }
            catch (Exception)
            {

                throw;
            }


        }

    }
}
