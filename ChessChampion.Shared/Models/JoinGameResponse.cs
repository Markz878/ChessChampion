namespace ChessChampion.Shared.Models;

public sealed record JoinGameResponse(Guid Id, GameStateModel GameState, PlayerModel Player1);
