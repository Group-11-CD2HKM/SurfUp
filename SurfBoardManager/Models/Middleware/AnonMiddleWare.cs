using Microsoft.AspNetCore.Identity;
using SurfUpLibary;

namespace SurfBoardManager.Models.Middleware
{
    public class AnonMiddleWare
    {
        private readonly ILogger<AnonMiddleWare> _logger;
        private readonly RequestDelegate _next;

        public AnonMiddleWare(ILogger<AnonMiddleWare> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext context, UserManager<SurfUpUser> userManager)
        {
            // Checks that we are at the right place, and makes sure that we only run this in post methods.
            if (context.Request.Path.StartsWithSegments("/BoardPosts/Rent") && context.Request.Method == HttpMethod.Post.Method && !context.User.Identity.IsAuthenticated)
            {
                // Find the IP of the Anonymous user and tries to get the surfUpUser by name(ip)
                string? anonIp = context.Connection.RemoteIpAddress?.ToString();
                var surfUpUser = await userManager.FindByNameAsync(anonIp);

                // Checks if surfUpUser exists, and creates a new surfUpUser if the user doesn't already exist.
                // Only attribute set for anonymous users will be the UserName as an ip-address
                if (surfUpUser == null)
                {
                    var newUser = new SurfUpUser()
                    {
                        UserName = anonIp
                    };

                    // userManager creates the user and stores it in DB.
                    await userManager.CreateAsync(newUser);
                }
                // Print out IP to screen
                _logger.LogInformation($"Anonymous IP: {anonIp}");
            }
            else
            {
                _logger.LogInformation($"User already authenticated.");
            }
            await _next.Invoke(context);
        }
    }

    public static class MiddleWareExtensions
    {
        public static IApplicationBuilder AnonIp(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnonMiddleWare>();
        }
    }
}
