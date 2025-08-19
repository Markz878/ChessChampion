using Azure.Identity;
using ChessChampionWebUI;
using ChessChampionWebUI.Data;
using Microsoft.ApplicationInsights.Extensibility;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSingleton<GamesService>();
if (builder.Environment.IsProduction())
{
    builder.Logging.AddApplicationInsights();
    builder.Services.AddApplicationInsightsTelemetry(x => x.EnableDependencyTrackingTelemetryModule = false);
    builder.Services.Configure<TelemetryConfiguration>(c => c.SetAzureTokenCredential(new ManagedIdentityCredential()));
}
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();