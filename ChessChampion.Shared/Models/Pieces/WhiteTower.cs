using ChessChampion.Shared.Models;
using static ChessChampion.Shared.Services.RulesService;

namespace ChessChampion.Shared.Models.Pieces;

public class WhiteTower() : ChessPiece("♖", true)
{
    public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y)
    {
        return GetTowerMovableSquares(gameState, x, y, this);
    }

    public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y)
    {
        return GetTowerThreatSquares(gameState, x, y, this);
    }

    public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
    {
        if (startSquare.ChessCoordinate == "a1")
        {
            gameState.CanWhiteKingCastleLeft = false;
        }
        else if (startSquare.ChessCoordinate == "h1")
        {
            gameState.CanWhiteKingCastleRight = false;
        }
        return base.HandleMove(gameState, startSquare, endSquare);
    }
}