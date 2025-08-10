namespace ChessChampion.Shared.Models;

public sealed record CreateGameResponse(Guid Id, string GameCode, GameStateModel GameState);