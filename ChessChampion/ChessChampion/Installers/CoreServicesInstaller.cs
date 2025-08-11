using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using ChessChampion.Core.Data;

namespace ChessChampion.Installers;

public sealed class CoreServicesInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<GamesService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<APIService>();
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<HubConnectionService>();
    }
}
