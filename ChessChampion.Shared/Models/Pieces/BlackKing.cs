using ChessChampion.Shared.Models;
using static ChessChampion.Shared.Services.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class BlackKing() : ChessPiece("♚", false)
{
    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetKingThreatSquares(gameState, x, y, this);
    }

    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetKingMovableSquares(gameState, x, y, this);
    }

    public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
    {
        if (startSquare.ChessCoordinate == "e8" && endSquare.ChessCoordinate == "g8")
        {
            gameState["h8"].Piece?.HandleMove(gameState, gameState["h8"], gameState["f8"]);
        }
        else if (startSquare.ChessCoordinate == "e8" && endSquare.ChessCoordinate == "c8")
        {
            gameState["a8"].Piece?.HandleMove(gameState, gameState["a8"], gameState["d8"]);
        }
        gameState.CanBlackKingCastleRight = false;
        gameState.CanBlackKingCastleLeft = false;
        return base.HandleMove(gameState, startSquare, endSquare);
    }
}
