using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackTower : ChessPiece
    {
        public BlackTower()
        {
            Marker = "♜";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerSquares(gameState, square);
        }
    }
}