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

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetWhitePawnSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (endSquare.Y == 7)
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
