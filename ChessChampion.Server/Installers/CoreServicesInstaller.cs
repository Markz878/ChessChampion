using ChessChampion.Client.Models;
using ChessChampion.Core.Data;
using ChessChampion.Server.Services;
using ChessChampion.Shared.Services;

namespace ChessChampion.Server.Installers;

public sealed class CoreServicesInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<GamesRepository>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<IChessService, ChessServerService>();
        builder.Services.AddSingleton<IHubConnectionService, MockHubConnectionService>();
        builder.Services.AddHttpContextAccessor();
    }
}
