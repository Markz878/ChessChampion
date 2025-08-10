using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddHttpClient<APIService>(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddStandardResilienceHandler();
builder.Services.AddSingleton(new MainViewModel());
builder.Services.AddSingleton<HubConnectionService>();
builder.Services.AddSingleton<APIService>();

await builder.Build().RunAsync();
