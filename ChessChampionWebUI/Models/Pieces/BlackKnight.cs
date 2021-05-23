using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackKnight : ChessPiece
    {
        public BlackKnight()
        {
            Marker = "♞";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKnightSquares(gameState, square, false);
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKnightSquares(gameState, square, true);
        }
    }
}