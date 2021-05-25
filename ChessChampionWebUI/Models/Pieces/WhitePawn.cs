using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhitePawn : ChessPiece
    {
        public WhitePawn()
        {
            Marker = "♙";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetWhitePawnThreatSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetWhitePawnMovableSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (endSquare.Y == 0)
            {
                startSquare.Piece = null;
                endSquare.Piece = new WhiteQueen();
            }
            else
            {
                base.HandleMove(gameState, startSquare, endSquare);
            }
        }
    }
}
