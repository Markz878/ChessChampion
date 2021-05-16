using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteKing : ChessPiece
    {
        public WhiteKing()
        {
            Marker = "♔";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKingSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (gameState.CanWhiteKingCastleRight && endSquare.ChessCoordinate == "g1")
            {
                gameState[7][7].Piece.HandleMove(gameState, gameState[7][7], gameState[7][5]);
            }
            else if (gameState.CanWhiteKingCastleLeft && endSquare.ChessCoordinate == "c1")
            {
                gameState[7][0].Piece.HandleMove(gameState, gameState[7][0], gameState[7][3]);
            }
            base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}
