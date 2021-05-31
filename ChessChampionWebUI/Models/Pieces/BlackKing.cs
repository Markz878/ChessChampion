using System;
using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackKing : ChessPiece
    {
        public BlackKing()
        {
            Marker = "♚";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKingThreatSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetKingMovableSquares(gameState, square);
        }

        public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (startSquare.ChessCoordinate == "e8" && endSquare.ChessCoordinate == "g8")
            {
                gameState["h8"].Piece.HandleMove(gameState, gameState["h8"], gameState["f8"]);
            }
            else if (startSquare.ChessCoordinate == "e8" && endSquare.ChessCoordinate == "c8")
            {
                gameState["a8"].Piece.HandleMove(gameState, gameState["a8"], gameState["d8"]);
            }
            gameState.CanBlackKingCastleRight = false;
            gameState.CanBlackKingCastleLeft = false;
            return base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}
