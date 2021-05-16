using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteTower : ChessPiece
    {
        public WhiteTower()
        {
            Marker = "♖";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerSquares(gameState, square);
        }
    }
}