using ChessChampion.Shared.Models;
using static ChessChampion.Shared.Services.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class WhiteKing() : ChessPiece("♔", true)
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
        if (startSquare.ChessCoordinate == "e1" && endSquare.ChessCoordinate == "g1")
        {
            gameState["h1"].Piece?.HandleMove(gameState, gameState["h1"], gameState["f1"]);
        }
        else if (startSquare.ChessCoordinate == "e1" && endSquare.ChessCoordinate == "c1")
        {
            gameState["a1"].Piece?.HandleMove(gameState, gameState["a1"], gameState["d1"]);
        }
        gameState.CanWhiteKingCastleRight = false;
        gameState.CanWhiteKingCastleLeft = false;
        return base.HandleMove(gameState, startSquare, endSquare);
    }
}
