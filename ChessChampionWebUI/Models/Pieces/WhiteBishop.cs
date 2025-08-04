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

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBishopMovableSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBishopThreatSquares(gameState, square);
        }
    }
}
