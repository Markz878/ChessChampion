using ChessChampion.Core.Data;
using ChessChampion.Core.Models;
using ChessChampion.Endpoints;
using ChessChampion.Hubs;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace ChessChampion.Tests;

public class EndpointTests
{
    [Fact]
    public async Task WhenGivenACheckMateSituation_AICheckMates_AndWinnerIsSent()
    {
        string moves = "e2e4 e7e5 d2d3 b8c6 g1f3 f8c5 a2a3 a7a6 b2b4 c5b6 c2c4 b6a7 c4c5 g8f6 c1g5 d7d6 b1c3 c8e6 d3d4 c6d4 f3d4 e5d4 d1d4 d6c5 d4d8 a8d8 b4c5 c7c6 g5e3 f6g4 a1d1 d8d1 e1d1 a6a5 h2h3 g4e3 f2e3 a7c5 a3a4 h7h5 d1d2 h8h6 h3h4 c5d6 h1g1 d6e5 f1e2 h6g6 e2h5 g6f6 g2g4 f6h6 d2d3 h6h8 c3e2 g7g6 h5g6 f7g6 h4h5 e5c7 h5g6 e8e7 g6g7 h8a8 e2f4 a8g8 f4e6 e7e6 d3d4 g8g7 d4c5 g7g5 c5d4 g5g8 e4e5 c7b8 g4g5 g8g6 d4e4 b8c7 e4f4 c7e5 f4f3 g6g7 g5g6 e6f6 f3e4 e5d6 e4d4 f6e6 e3e4 b7b6 e4e5 d6c5 d4e4 c5g1 e4f4 g1h2 f4g5 g7c7 g5h6 h2f4 h6h5 e6e5 g6g7 c7g7 h5h4 e5f5 h4h5 g7d7";
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>() { { "AICalculationTime", "3000" }, { "EngineFileName", "stockfish-windows-18-x86-64-avx2.exe" } }).Build();
        GamesService gamesService = new(configuration);
        PlayerModel player = new("Player", true);
        GameModel game = gamesService.CreateGameWithAI(player, 20);
        foreach (string move in moves.Split(' '))
        {
            MoveError? error = game.TryMakeMove(move);
            Assert.Null(error);
        }
        IHubContext<ChessHub> hub = Substitute.For<IHubContext<ChessHub>>();
        IClientProxy clientProxy = Substitute.For<IClientProxy>();
        hub.Clients.Group(Arg.Is(game.Id.ToString())).Returns(clientProxy);
        Results<NoContent, BadRequest<string>, NotFound<string>> result = await EndpointsMapper.SubmitMove(new SubmitMoveRequest(game.Id, "h5h4", "Player"), gamesService, hub, NullLogger<AIPlayerModel>.Instance);
        Assert.True(result.Result is NoContent);
        await clientProxy.SendAsync(Arg.Is(nameof(IChessHubNotifications.GameOver)), Arg.Any<string>()).ReceivedWithAnyArgs(1);
    }
}

public class FakeHub : IHubContext<ChessHub>
{
    public IHubClients Clients => throw new NotImplementedException();

    public IGroupManager Groups => throw new NotImplementedException();
}

public class FakeHubClients : IHubClients
{
    public IClientProxy All => throw new NotImplementedException();

    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds)
    {
        throw new NotImplementedException();
    }

    public IClientProxy Client(string connectionId)
    {
        throw new NotImplementedException();
    }

    public IClientProxy Clients(IReadOnlyList<string> connectionIds)
    {
        throw new NotImplementedException();
    }

    public IClientProxy Group(string groupName)
    {
        throw new NotImplementedException();
    }

    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds)
    {
        throw new NotImplementedException();
    }

    public IClientProxy Groups(IReadOnlyList<string> groupNames)
    {
        throw new NotImplementedException();
    }

    public IClientProxy User(string userId)
    {
        throw new NotImplementedException();
    }

    public IClientProxy Users(IReadOnlyList<string> userIds)
    {
        throw new NotImplementedException();
    }
}
