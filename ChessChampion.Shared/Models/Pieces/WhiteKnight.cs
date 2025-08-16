using static ChessChampion.Shared.Services.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class WhiteKnight() : ChessPiece("♘", true)
{
    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetKnightMovableSquares(gameState, x, y, this);
    }

    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetKnightThreatSquares(gameState, x, y, this);
    }
}