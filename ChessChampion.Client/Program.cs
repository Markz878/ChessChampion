using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddHttpClient("api", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddStandardResilienceHandler();
builder.Services.AddScoped<MainViewModel>();
builder.Services.AddScoped<HubConnectionService>();
builder.Services.AddScoped<IChessService, ChessClientService>();
builder.Services.AddScoped<IHubConnectionService, HubConnectionService>();

await builder.Build().RunAsync();
