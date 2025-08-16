using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;

namespace ChessChampion.Services;

public sealed class MockChessService : IChessService
{
    public Task CreateGame(CreateGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task JoinGame(JoinGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task LeaveGame(LeaveGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseError?> SubmitMove(SubmitMoveRequest request)
    {
        throw new NotImplementedException();
    }
}
