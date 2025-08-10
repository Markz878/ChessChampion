using Microsoft.AspNetCore.ResponseCompression;

namespace ChessChampion.Installers;

public class SignalRInstaller : IInstaller
{
    private const string octetStream = "application/octet-stream";
    public void Install(WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSignalR(o => o.EnableDetailedErrors = true);
        }
        else
        {
            builder.Services.AddSignalR();
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Append(octetStream);
            });
        }
    }
}
