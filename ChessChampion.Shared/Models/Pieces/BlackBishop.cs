using static ChessChampion.Shared.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class BlackBishop() : ChessPiece("♝", false)
{
    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetBishopMovableSquares(gameState, x, y, this);
    }

    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetBishopThreatSquares(gameState, x, y, this);
    }
}
