using ChessChampion.Shared.Models;

namespace ChessChampion.Shared.Services;
public interface IChessService
{
    Task CreateGame(CreateGameRequest request);
    Task JoinGame(JoinGameRequest request);
    Task LeaveGame(LeaveGameRequest request);
    Task<BaseError?> SubmitMove(SubmitMoveRequest request);
}
