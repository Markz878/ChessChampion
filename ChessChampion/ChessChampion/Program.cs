using ChessChampion.Components;
using ChessChampion.Endpoints;
using ChessChampion.Hubs;
using ChessChampion.Installers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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
