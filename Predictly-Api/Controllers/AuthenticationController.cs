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

        public AuthenticateController(UserManager<ApplicationUserModel> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
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
                    userName = user.UserName,
                    role = userRoles,
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            ApplicationUserModel user = new()
            {
                UserName = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Staff.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Staff.ToString()));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));

            if (!string.IsNullOrEmpty(model.Role.ToString()) && model.Role == UserRoles.Admin)
            {

                if (await _roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
                }
            }
            else if (!string.IsNullOrEmpty(model.Role.ToString()) && model.Role == UserRoles.Staff)
            {

                if (await _roleManager.RoleExistsAsync(UserRoles.Staff.ToString()))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Staff.ToString());
                }
            }

            string confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            sendEmail("verify", null, user, confirmationToken);
            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
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
                        verifyUrl = $"{Request.Headers["origin"]}/account/confirm-email?userid={_user.Id}&token={_token}";
                        message = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
                        subject = "Sign-up Verification Vaccination Management System - Verify Email";
                        html = $@"<h4>Verify Email</h4>
                         {message}";
                        break;
                    case "resetPass":
                        verifyUrl = $"{Request.Headers["origin"]}/account/reset-password?userid={_user.Id}&token={_token}";
                        message = $@"<p>Please click the below link to reset your password:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
                        subject = "Vaccination Management System - Reset password";
                        html = $@"<h4>Reset Password</h4>
                         {message}";
                        break;
                    case "verified":
                        message = $@"<p>Verification Successfull!</p>";
                        subject = "Sign-up Verification Vaccination Management System";
                        html = $@"<h4>Verify Email</h4>
                         {message}";
                        break;
                    case "resetted":
                        message = $@"<p>Reset Password Successfull!</p>";
                        subject = "Password Reset Successfull";
                        html = $@"<h4>Password Reset</h4>
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
