using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces;

public class WhitePawn() : ChessPiece("♙", true)
{
    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetWhitePawnThreatSquares(gameState, x, y);
    }

    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetWhitePawnMovableSquares(gameState, x, y, this);
    }

    public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
    {
        if (endSquare.Y == 0)
        {
            foreach (GameSquare square in gameState.GetSquares())
            {
                square.WasPreviousMove = false;
            }
            startSquare.Piece = null;
            endSquare.Piece = new WhiteQueen();
            startSquare.WasPreviousMove = true;
            endSquare.WasPreviousMove = true;
            return startSquare.ChessCoordinate + endSquare.ChessCoordinate + "q";
        }
        else
        {
            return base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}
