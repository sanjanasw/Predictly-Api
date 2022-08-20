using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Predictly_Api.Helpers;
using Predictly_Api.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Predictly_Api.Middlewares
{
    public class ApiExceptionHandlerMiddleware
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<ApiExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ApiExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env,
            ILogger<ApiExceptionHandlerMiddleware> logger)
        {
            _env = env;
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUserModel> userManager)
        {
            try
            {
                if (context.Request.Path.Value.Contains("api"))
                {
                    var claims = context.User.Claims;
                    if (claims.Any())
                    {
                        var userId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)
                            ?.Value;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user == null)
                            {
                                context.Response.Clear();
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                await context.Response.WriteAsync("Unauthorized");
                                return;
                            }

                            var roleName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                            var role = await userManager.GetRolesAsync(user);
                            if (role == null)
                            {
                                context.Response.Clear();
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                await context.Response.WriteAsync("Unauthorized");
                                return;
                            }
                        }
                    }
                }

                await _next(context);
            }
            catch (HumanErrorException ex)
            {
                await context.Response.WriteAsync(GenerateError(ex, (int)HttpStatusCode.BadRequest, context));
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(GenerateError(ex, (int)HttpStatusCode.InternalServerError, context));
            }
        }

        private string GenerateError(Exception ex, int responseCode, HttpContext context)
        {
            _logger.LogError("Server Error", ex);
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = responseCode;
            var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, };

            ApiException response = FindError(responseCode, ex);

            return JsonSerializer.Serialize(response, option);
        }

        private ApiException FindError(int responseCode, Exception ex)
        {
            if (ex is NullReferenceException)
            {
                responseCode = 700;
            }

            if (ex is IndexOutOfRangeException)
            {
                responseCode = 701;
            }

            if (ex is IOException)
            {
                responseCode = 702;
            }

            if (ex is WebException)
            {
                responseCode = 703;
            }

            if (ex is SqlException)
            {
                responseCode = 704;
            }

            if (ex is StackOverflowException)
            {
                responseCode = 705;
            }

            if (ex is OutOfMemoryException)
            {
                responseCode = 706;
            }

            if (ex is InvalidCastException)
            {
                responseCode = 707;
            }

            if (ex is InvalidOperationException)
            {
                responseCode = 708;
            }

            if (ex is FormatException)
            {
                responseCode = 710;
            }

            if (ex is NotSupportedException)
            {
                responseCode = 711;
            }
            if (ex is HumanErrorException x)
            {
                if (x.Details != null)
                {
                    responseCode = 712;
                }
                return new ApiException(responseCode, ex.Message, x.Details);
            }

            return new ApiException(responseCode, ex.Message, SerializeExceptionAsJsonObject(ex, null));
        }

        public dynamic SerializeExceptionAsJsonObject(Exception e, dynamic exceptionMessage)
        {
            if (e == null) return string.Empty;

            exceptionMessage = new { parent = exceptionMessage ?? "", message = e.Message, trance = e.StackTrace };


            if (e.InnerException != null)
                exceptionMessage = SerializeExceptionAsJsonObject(e.InnerException, exceptionMessage);

            return exceptionMessage;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.  
    public static class ExceptionHandlerMiddleware
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiExceptionHandlerMiddleware>();
        }
    }
}
