using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackPawn : ChessPiece
    {
        public BlackPawn()
        {
            Marker = "♟︎";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBlackPawnSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (endSquare.Y == 7)
            {
                startSquare.Piece = null;
                endSquare.Piece = new BlackQueen();
            }
            else
            {
                base.HandleMove(gameState, startSquare, endSquare);
            }
        }
    }
}
