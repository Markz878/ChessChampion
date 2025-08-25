using System.Threading.RateLimiting;

namespace ChessChampion.Server.Installers;

public sealed class RateLimitInstaller : IInstaller
{
    public const string PolicyName = "RateLimitPolicy";
    public void Install(WebApplicationBuilder builder)
    {
        RateLimitOptions rateLimitOptions = new();
        builder.Configuration.GetSection(nameof(RateLimitOptions)).Bind(rateLimitOptions);
        builder.Services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            opt.AddPolicy(PolicyName, httpContext =>
            {
                ILogger<RateLimitInstaller> logger = httpContext.RequestServices.GetRequiredService<ILogger<RateLimitInstaller>>();
                string? ip = httpContext.Connection.RemoteIpAddress?.ToString();
                logger.LogInformation("Remote IP in rate limiting: {Ip}", ip);
                if (ip is null)
                {
                    return RateLimitPartition.GetTokenBucketLimiter(string.Empty, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = rateLimitOptions.AnonTokenLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = rateLimitOptions.AnonQueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitOptions.AnonReplenishmentPeriod),
                            TokensPerPeriod = rateLimitOptions.AnonTokensPerPeriod,
                            AutoReplenishment = true
                        });
                }
                else
                {
                    return RateLimitPartition.GetTokenBucketLimiter(ip, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = rateLimitOptions.UserTokenLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = rateLimitOptions.UserQueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(rateLimitOptions.UserReplenishmentPeriod),
                            TokensPerPeriod = rateLimitOptions.UserTokensPerPeriod,
                            AutoReplenishment = true
                        });
                }
            });
        });
    }
}

internal sealed class RateLimitOptions
{
    public int AnonTokenLimit { get; set; }
    public int AnonQueueLimit { get; set; }
    public int AnonReplenishmentPeriod { get; set; }
    public int AnonTokensPerPeriod { get; set; }
    public int UserTokenLimit { get; set; }
    public int UserQueueLimit { get; set; }
    public int UserReplenishmentPeriod { get; set; }
    public int UserTokensPerPeriod { get; set; }
}
