using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;

namespace ChessChampion.Server.Services;

public sealed class ChessServerService : IChessService
{
    public Task<Result<CreateGameResponse, BaseError>> CreateGame(CreateGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<JoinGameResponse, BaseError>> JoinGame(JoinGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseError?> LeaveGame(LeaveGameRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BaseError?> SubmitMove(SubmitMoveRequest request)
    {
        throw new NotImplementedException();
    }
}
