using ChessChampion.Client.Models;
using ChessChampion.Core.Data;
using ChessChampion.Services;
using ChessChampion.Shared.Services;

namespace ChessChampion.Installers;

public sealed class CoreServicesInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<GamesRepository>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<IChessService, MockChessService>();
    }
}
