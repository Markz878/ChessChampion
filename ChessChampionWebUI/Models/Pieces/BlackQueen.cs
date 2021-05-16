using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackQueen : ChessPiece
    {
        public BlackQueen()
        {
            Marker = "♛";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetQueenSquares(gameState, square);
        }
    }
}