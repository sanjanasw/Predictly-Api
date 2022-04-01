using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Predictly_Api.Enums;
using Predictly_Api.Models;
using Predictly_Api.Services;
using Predictly_Api.ViewModels.Authentication;
using Predictly_Api.ViewModels.User;

namespace Predictly_Api.Controllers
{
    [Route("auth")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(UserManager<ApplicationUserModel> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context,
            IConfiguration configuration, IEmailService emailService, ILogger<AuthenticateController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/login
        ///     {
        ///        "userName": "sanjana",
        ///        "password": "$Sanjana1"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns user data with JWT</response>
        /// <response code="401">Unothorized user</response>
        /// <response code="403">User doesn't have access to this endpoint</response>
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponseViewModel>> Login([FromBody] LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null && !user.DeleteStatus && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    if (!user.EmailConfirmed)
                    {
                        _logger.LogWarning(string.Format("{0} is tried loggin to the system with out confirming email.", user.UserName));
                        return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Please verify your email!" });
                    }

                    if (user.DeleteStatus)
                    {
                        _logger.LogWarning(string.Format("{0} deleted user is tried loggin to the system.", user.UserName));
                        return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Your account is deleted by admins. Please contact us ASAP!" });
                    }

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

                    var school = _context.School.Where(x => x.Id == user.SchoolId).Select(x => x.Name).FirstOrDefault();

                    _logger.LogInformation(string.Format("{0} logged in to the system", user.UserName));
                    return Ok(new LoginResponseViewModel
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        Id = user.Id,
                        Name = user.FirstName + ' ' + user.LastName,
                        Username = user.UserName,
                        Email = user.Email,
                        SchoolId = user.SchoolId,
                        School = school,
                        Role = (List<string>)userRoles,
                    });
                }
                _logger.LogWarning(string.Format("{0} Invalid login attempt.", model.UserName));
                return Unauthorized(new ResponseModel { Status = "Unauthorized", Message = "Username or password incorrect!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/login.");
                throw;
            }
        }

        /// <summary>
        /// Register as a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/register
        ///     {
        ///        "userInfo": {
        ///            "firstName": "sanjana",
        ///            "lastName": "witharanage",
        ///            "email": "sanjanasw99@gmail.com",
        ///            "olYear": 2017,
        ///            "gender": 0,
        ///            "schoolId": 1,
        ///            "bSub1": 42,
        ///            "bSub2": 33,
        ///            "bSub3": 15,
        ///            "fathersEduLevel": 0,
        ///            "mothersEduLevel": 0,
        ///            "username": "sanjanasw",
        ///            "password": "$Sanjana1"
        ///         },
        ///        "studyData": [
        ///         {
        ///            "SubjectId": 2,
        ///            "Commitment": 0,
        ///            "ClassStatus": true,
        ///            "AvgMarks": 45,
        ///         }
        ///         ]
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="409">Username or email already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<ResponseModel>> Register([FromBody] RegisterViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var userExists =  await _userManager.FindByNameAsync(model.UserInfo.Username);
                    var userEmailExists = await  _userManager.FindByEmailAsync(model.UserInfo.Email);
                    if (userExists != null)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username is already exists!" });
                    }
                    else if ( userEmailExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create another account.", model.UserInfo.Email));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Email is already exists!" });
                    }

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

                    foreach (var item in model.StudyData)
                    {
                        _context.StudyData.Add(new StudyDataModel
                        {
                            UserId = user.Id,
                            Commitment = item.Commitment,
                            AvgMarks = item.AvgMarks,
                            ClassStatus = item.ClassStatus,
                            SubjectId = item.SubjectId
                        });
                    };
                    _context.SaveChanges();
                    await transaction.CommitAsync();

                    if (!result.Succeeded)
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                    var confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user);
                    SendEmail("verify", null, user, await confirmationToken);
                    _logger.LogInformation(string.Format("{0} new student registered.", user.UserName));
                    return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in auth/register.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/school-register
        ///     {
        ///       "userInfo": {
        ///          "firstName": "sanjana",
        ///          "lastName": "sulakshana",
        ///          "email": "sanjana@dharmaraja.lk",
        ///          "gender": 0,
        ///          "username": "sanjana",
        ///          "password": "$Sanjana1"
        ///         },
        ///      "schoolInfo": {
        ///          "name": "Dharmaraja College",
        ///          "address": "Dharmaraja Rd. Kandy."
        ///         }
        ///       }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="409">Username or email already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Route("school-register")]
        public async Task<ActionResult<ResponseModel>> SchoolRegister([FromBody] SchoolRegisterViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (_context.School.Any(x => x.Name.ToLower().Equals(model.SchoolInfo.Name.ToLower())))
                    {
                        _logger.LogWarning(string.Format("{0} is tried to add {1} school again.", model.UserInfo.Email, model.SchoolInfo.Name));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "School is already exists!" });
                    }

                    var userExists =  await _userManager.FindByNameAsync(model.UserInfo.Username);
                    var userEmailExists = await _userManager.FindByEmailAsync(model.UserInfo.Email);
                    if ( userExists != null)
                    {
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username is already exists!" });
                    }
                    else if ( userEmailExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create another account.", model.UserInfo.Email));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Email is already exists!" });
                    }

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

                    user.SchoolId = school.Id;
                    await _userManager.UpdateAsync(user);
                    await transaction.CommitAsync();

                    if (!result.Succeeded)
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User creation failed! Please check user details and try again." });

                    var confirmationToken = _userManager.GenerateEmailConfirmationTokenAsync(user);
                    SendEmail("verify", null, user, await confirmationToken);
                    _logger.LogInformation(string.Format("{0}, new school registered. School: {1}", user.UserName, school.Name));
                    return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in auth/school-register.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Confirm email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/confirm-email
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token</response>
        /// <response code="404">User not found</response>
        [HttpPost("confirm-email")]
        public async Task<ActionResult<ResponseModel>> ConfirmEmail(ConfirmEmailViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.Userid);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ConfirmEmailAsync(user, model.Token);
                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to comfirm email with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
                }

                SendEmail("verified", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully confirmed email!", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Verification successful, you can now login" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/confirm-email");
                throw;
            }
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/forgot-password
        ///     {
        ///         "email": "sanjanasw99@gmail.com"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="404">User not found</response>
        [HttpPost("forgot-Password")]
        public async Task<ActionResult<ResponseModel>> ForgotPassword(ForgetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                SendEmail("resetPass", null, user, token);
                _logger.LogInformation(string.Format("{0} generated reset password link.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Reset Password Link Sent!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/forgot-password.");
                throw;
            }
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/reset-password
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi",
        ///         "password": "Not@1234"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token</response>
        /// <response code="404">User not found</response>
        [HttpPost("reset-Password")]
        public async Task<ActionResult<ResponseModel>> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.Userid);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to reset password with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Token Invalid!" });
                }

                SendEmail("resetted", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully resetted password.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Password Reset Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/reset-password");
                throw;
            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/change-password
        ///     {
        ///         "currentPassword": "Not@1234",
        ///         "newPassword": "1234@Not"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Password doesn't meet minimum requirements</response>
        /// <response code="403">Current password incorrect</response>
        /// <response code="404">User not found</response>
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ResponseModel>> NewUserSetupAsync(ChangePasswordViewModel model)
        {
            try
            {
                var accessToken = HttpContext.GetTokenAsync("access_token");
                var token = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                var loggedInUserId = token.Claims.First(claim => claim.Type == "nameid").Value;
                var loggedInUser = await _userManager.FindByIdAsync(loggedInUserId);

                if (loggedInUser == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                var resetToken = _userManager.GeneratePasswordResetTokenAsync(loggedInUser);

                if (!await _userManager.CheckPasswordAsync(loggedInUser, model.CurrentPassword))
                {
                    _logger.LogWarning(string.Format("{0} tried to change password with incorrect current password.", loggedInUser.UserName));
                    return StatusCode(StatusCodes.Status403Forbidden, new ResponseModel { Status = "Error", Message = "Current password is incorrect!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(loggedInUser, await resetToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Somethig went wrong!" });
                }

                SendEmail("passwordChanged", loggedInUser.Email, null, null);
                _logger.LogInformation(string.Format("{0} successfully changed password.", loggedInUser.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "Password change Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/change-password.");
                throw;
            }
        }

        /// <summary>
        /// Create new admin/staff account. [Access: Admins and Staff only]
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/force-onboard
        ///     {
        ///         "username": "sanjanasw",
        ///         "firstName": "sanjana",
        ///         "lastName": "witharanage",
        ///         "email": "sanjanasw99@gmail.com",
        ///         "gender": 0,
        ///         "role": 1,
        ///         "schoolId": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns new user details</response>
        /// <response code="400">Username or email already exists</response>
        /// <response code="403">Forbidden</response>
        /// <response code="409">User details conflict</response>
        /// <response code="500">Internal server error</response>
        [Authorize(Roles = "Admin, Staff")]
        [HttpPost]
        [Route("force-onboard")]
        public async Task<ActionResult<StaffViewModel>> NewUser([FromBody] NewUserViewModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var accessToken = HttpContext.GetTokenAsync("access_token");
                    var JWTtoken = new JwtSecurityTokenHandler().ReadJwtToken(await accessToken) as JwtSecurityToken;
                    var loggedInUserId = JWTtoken.Claims.First(claim => claim.Type == "nameid").Value;
                    var loggedInUser  = await _userManager.FindByIdAsync(loggedInUserId);

                    var userExists = await _userManager.FindByNameAsync(model.Username);
                    var userEmailExists = await _userManager.FindByEmailAsync(model.Email);

                    if (userExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create force-onboard account for existing username: {1}.", loggedInUser.UserName, model.Username));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Username is already exists!" });
                    }
                    else if (userEmailExists != null)
                    {
                        _logger.LogWarning(string.Format("{0} is tried to create force-onboard account for existing email: {1}.", loggedInUser.UserName, model.Email));
                        return StatusCode(StatusCodes.Status409Conflict, new ResponseModel { Status = "Error", Message = "Email is already exists!" });
                    }

                    ApplicationUserModel user = new()
                    {
                        UserName = model.Username,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        SchoolId = model.SchoolId,
                        SecurityStamp = Guid.NewGuid().ToString(),
                    };

                    var result = await _userManager.CreateAsync(user, "$NewUserPassword1Temp");
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

                    var token = _userManager.GeneratePasswordResetTokenAsync(user);
                    await transaction.CommitAsync();

                    var userResponse = new StaffViewModel
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Email = user.Email,
                        Role = string.Join(",", _userManager.GetRolesAsync(user).Result.ToArray()),
                        isActive = user.EmailConfirmed,
                        SchoolId = user.SchoolId,
                    };

                    _logger.LogInformation(string.Format("{0}, new user onborded to username: {1}", loggedInUser.UserName, user.UserName));
                    SendEmail("newUser", null, user, await token);
                    return Ok(userResponse);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occcured in auth/force-onboard.");
                    throw;
                }
            }
        }

        /// <summary>
        /// New user account setup
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/new-user-setup
        ///     {
        ///         "userid": "gfuie-8feiufb-reufberf-rei",
        ///         "token": "kjufbkjdfuirefu8h4r94ruiuwb38dbnie844bu44bi",
        ///         "password": "Not@1234"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns success message</response>
        /// <response code="400">Invalid token or password doesn't meet minimum requirements</response>
        /// <response code="404">User not found</response>
        [HttpPost("new-user-setup")]
        public async Task<ActionResult<ResponseModel>> ChangePassword(ResetPasswordViewModel model)
        {
            try
            {
                ApplicationUserModel user = await _userManager.FindByIdAsync(model.Userid);

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ResponseModel { Status = "Error", Message = "User Not Found!" });
                }

                IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(string.Format("{0} is tried to setup fresh account with invalid token.", user.UserName));
                    return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Invalid token or password doesn't meet minimum requirements!" });
                }

                SendEmail("newUserSetup", user.Email, null, null);
                _logger.LogInformation(string.Format("{0} is onboarded successfully.", user.UserName));
                return Ok(new ResponseModel { Status = "Success", Message = "New User Setup Successfull!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occcured in auth/new-user-setup");
                throw;
            }
        }

        private void SendEmail(string _type, string _email = null, ApplicationUserModel _user = null, string _token = null)
        {
            string subject = "";
            string html = "";
            string verifyUrl;
            string email = _user != null ? _user.Email : _email;

            try
            {
                switch (_type)
                {
                    case "verify":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/confirm-email?userid={_user.Id}&token={_token}";
                        subject = "Sign-up Verification Vaccination Management System - Verify Email";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1kIvq5gRqlUM_y-Y-7KpQw3oGtuX7Im0A'
                                alt=''
                                />
                            <h2>
                                Please click the below button to <br /> verify your email
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(4, 128, 201);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Verify email
                                </a></center>";
                        break;
                    case "resetPass":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/reset-password?userid={_user.Id}&token={_token}";
                        subject = "Vaccination Management System - Reset password";
                        html = $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=12MmOUkndXs65qf7kd6FCzV4iZGKPF16s'
                                alt=''
                                />
                            <h2 style=""
                                    color: black;
                                "">
                                Please click the below button to <br />
                                reset your password
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(255, 115, 0);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Reset Password
                                </a>
                            </center>";
                        break;
                    case "verified":
                        subject = "Sign-up Verification Vaccination Management System";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1LqFsaoDVUdXQMUMoEZ8MkNTjDiYQp1FZ'
                                alt=''
                                />
                            <h2 style=""
                                    color: black;
                                "">
                                Your email verification is successfull
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(37, 199, 50);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a></center>";
                        break;
                    case "resetted":
                        subject = "Password Reset Successfull";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1tQNONuwfg5phj1teyBbG7W02lpQ6nPBi'
                                alt=''
                                />
                            <h2>
                                Password Resetted Successfully!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(143, 179, 46);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a>
                            </center>";
                        break;
                    case "newUser":
                        verifyUrl = $"https://predictly.z13.web.core.windows.net/auth/new-user-setup?userid={_user.Id}&token={_token}";
                        subject = "Vaccination Management System - New User Invitation";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1ornFZghAE9F3kNLxmMYNo5F9H0azVKU3'
                                alt=''
                                />
                            <h2>
                                New User Setup
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(179, 80, 204);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""{verifyUrl}""
                                >
                                Continue to Login
                                </a>
                            </center>";
                        break;
                    case "newUserSetup":
                        subject = "Password Setup Successfull";
                        html =
                        $@" <center>
                                <img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=1GhZJQfcGeJhxZ0_kPh2MrfSMe9izMsi-'
                                alt=''
                                />
                            <h2>
                                Password of new user account <br />
                                setup successfully!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgb(104, 107, 109);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a>
                        </center>";
                        break;
                    case "passwordChanged":
                        subject = "Password Change Successfull";
                        html =
                        $@" <center><img
                                style=""width: 40%""
                                src='https://docs.google.com/uc?id=10uumtpFjMuE7CIXYiQjeKmNPMIhr1YkX'
                                alt=''
                                />
                            <h2>
                                Password change successfull!
                            </h2>
                            <br />
                                <a
                                style=""
                                    border-radius: 5px;
                                    color: white;
                                    background-color: rgba(245, 55, 91, 1);
                                    padding: 15px;
                                    border: none;
                                    letter-spacing: 0.1rem;
                                    text-transform: uppercase;
                                    text-decoration: none;
                                ""
                                href=""https://predictly.z13.web.core.windows.net/auth/login""
                                >
                                Continue to Login
                                </a><center>";
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
