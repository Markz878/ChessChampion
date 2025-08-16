namespace ChessChampion.Shared.Models;

public sealed record LeaveGameRequest(
    Guid GameId,
    string UserName
);
