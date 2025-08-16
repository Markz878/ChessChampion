namespace ChessChampion.Shared.Services;

public interface IChessHubClientActions
{
    Task JoinGame(Guid gameId);
    Task LeaveGame(Guid gameId);
}
