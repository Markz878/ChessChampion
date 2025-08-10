namespace ChessChampion.Shared.Models;

public sealed record class LeaveGameRequest(
    Guid GameId,
    string UserName
);
