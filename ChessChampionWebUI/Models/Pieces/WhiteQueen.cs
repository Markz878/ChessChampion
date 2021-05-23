using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteQueen : ChessPiece
    {
        public WhiteQueen()
        {
            Marker = "♕";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetQueenSquares(gameState, square);
        }
    }
}