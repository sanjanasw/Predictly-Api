using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Predictly_Api.Middlewares
{
    public class AccessLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessLoggerMiddleware> _logger;

        public AccessLoggerMiddleware(RequestDelegate next, ILogger<AccessLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var info = $"UserId: {context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "0"} | Username: {context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? "anonymous"}| Route: {context.Request.Path.Value}";
            _logger.LogInformation(info);
            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.  
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AccessLoggerMiddleware>();
        }
    }
}
