using ChessChampion.Server.Components;
using ChessChampion.Server.Endpoints;
using ChessChampion.Server.Hubs;
using ChessChampion.Server.Installers;

WebApplicationBuilder builder = WebApplication.CreateBuilder();

builder.InstallAssemblyServices();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapStaticAssets();
app.UseHttpLogging();
if (app.Environment.IsProduction())
{
    app.UseMiddleware<SecurityHeadersMiddleware>();
}
app.UseAntiforgery();
app.UseRateLimiter();
app.MapStaticAssets();
app.MapEndpoints();
app.MapHub<ChessHub>("/chesshub", options => options.AllowStatefulReconnects = true);
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ChessChampion.Client._Imports).Assembly);
app.MapHealthChecks("/health");
app.Run();
