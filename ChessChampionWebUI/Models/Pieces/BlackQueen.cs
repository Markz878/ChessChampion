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

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetQueenMovableSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetQueenThreatSquares(gameState, square);
        }
    }
}