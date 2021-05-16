﻿using System.Collections.Generic;
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
            if (startSquare.ChessCoordinate == "e1" && endSquare.ChessCoordinate == "g1")
            {
                gameState["h1"].Piece.HandleMove(gameState, gameState["h1"], gameState["f1"]);
            }
            else if (startSquare.ChessCoordinate == "e1" && endSquare.ChessCoordinate == "c1")
            {
                gameState["a1"].Piece.HandleMove(gameState, gameState["a1"], gameState["d1"]);
            }
            gameState.CanWhiteKingCastleRight = false;
            gameState.CanWhiteKingCastleLeft = false;
            base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}
