using static ChessChampion.Shared.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class BlackPawn() : ChessPiece("♟︎", false)
{
    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetBlackPawnThreatSquares(gameState, x, y);
    }

    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetBlackPawnMovableSquares(gameState, x, y, this);
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
            endSquare.Piece = new BlackQueen();
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
