using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteBishop : ChessPiece
    {
        public WhiteBishop()
        {
            Marker = "♗";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBishopSquares(gameState, square);
        }
    }
}
