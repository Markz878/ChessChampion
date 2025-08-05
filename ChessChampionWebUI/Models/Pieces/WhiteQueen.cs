using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces;

public class WhiteQueen() : ChessPiece("♕", true)
{
    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetQueenMovableSquares(gameState, x, y, this);
    }

    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetQueenThreatSquares(gameState, x, y, this);
    }
}