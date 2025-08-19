using ChessChampion.Shared.Models;

namespace ChessChampion.Shared.Services;

public interface IChessService
{
    Task<Result<CreateGameResponse, BaseError>> CreateGame(CreateGameRequest request);
    Task<Result<JoinGameResponse, BaseError>> JoinGame(JoinGameRequest request);
    Task<BaseError?> LeaveGame(LeaveGameRequest request);
    Task<BaseError?> SubmitMove(SubmitMoveRequest request);
}
