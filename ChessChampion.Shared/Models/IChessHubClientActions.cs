namespace ChessChampion.Shared.Models;

public interface IChessHubClientActions
{
    Task JoinGame(Guid gameId);
    Task LeaveGame(Guid gameId);
}
