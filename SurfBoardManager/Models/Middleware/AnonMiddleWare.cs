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

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                // Find the IP of the Anonymous user
                string? anonIp = context.Connection.RemoteIpAddress?.ToString();

                // Print out IP to screen
                _logger.LogInformation($"Anonymous IP: {anonIp}");
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
