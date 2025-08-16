namespace ChessChampion.Shared.Services;

public interface IHubConnectionService
{
    string? ConnectionId();
    Task JoinGame(Guid gameId);
    Task LeaveGame(Guid gameId);
}