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
            return GetKnightMovableSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKnightThreatSquares(gameState, square);
        }
    }
}