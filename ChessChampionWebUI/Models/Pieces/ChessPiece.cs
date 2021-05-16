﻿using System.Collections.Generic;

namespace ChessChampionWebUI.Models.Pieces
{
    public abstract class ChessPiece
    {
        public string Marker { get; set; }
        public bool IsWhite { get; set; }
        public abstract IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square);
        public virtual void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            endSquare.Piece = this;
            startSquare.Piece = null;
        }
    }
}