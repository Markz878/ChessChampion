using Microsoft.Extensions.Primitives;

namespace ChessChampion.Installers;

public sealed class SecurityHeadersMiddlewareInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);
        builder.Services.AddSingleton<SecurityHeadersMiddleware>();
    }
}

public sealed class SecurityHeadersMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Append("X-Frame-Options", new StringValues("DENY"));
        context.Response.Headers.Append("Cross-Origin-Opener-Policy", new StringValues("same-origin"));
        context.Response.Headers.Append("Content-Security-Policy", new StringValues(
            "default-src 'self'; " +
            "base-uri 'self'; " +
            "connect-src 'self'; " +
            "object-src 'none'; " +
            "script-src 'self' 'wasm-unsafe-eval'; " +
            "style-src 'self'; " +
            "img-src 'self'; " +
            "upgrade-insecure-requests;"
        ));
        return next(context);
    }
}
