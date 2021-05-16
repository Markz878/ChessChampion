using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteKnight : ChessPiece
    {
        public WhiteKnight()
        {
            Marker = "♘";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKnightSquares(gameState, square);
        }
    }
}