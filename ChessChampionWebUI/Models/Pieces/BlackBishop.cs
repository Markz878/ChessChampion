using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackBishop : ChessPiece
    {
        public BlackBishop()
        {
            Marker = "♝";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBishopSquares(gameState, square);
        }
    }
}
