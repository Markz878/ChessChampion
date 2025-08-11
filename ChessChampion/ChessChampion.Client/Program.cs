using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddHttpClient("api", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
    //.AddStandardResilienceHandler();
string x = builder.HostEnvironment.BaseAddress;
builder.Services.AddSingleton(new MainViewModel());
builder.Services.AddSingleton<HubConnectionService>();
builder.Services.AddSingleton<APIService>();

await builder.Build().RunAsync();
