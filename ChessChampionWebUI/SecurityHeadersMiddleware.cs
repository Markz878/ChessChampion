using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChessChampionWebUI
{
    public class SecurityHeadersMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Feature-Policy", "geolocation 'none';midi 'none';notifications 'none';push 'none';sync-xhr 'none';microphone 'none';camera 'none';magnetometer 'none';gyroscope 'none';speaker 'self';vibrate 'none';fullscreen 'self';payment 'none'");
            return next(context);
        }
    }
}
