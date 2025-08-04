using System.Collections.Generic;

namespace ChessChampionWebUI.Models.Pieces
{
    public abstract class ChessPiece
    {
        public string Marker { get; set; }
        public bool IsWhite { get; set; }

        public abstract IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square);

        public abstract IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square);

        public virtual string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            foreach (var square in gameState.GetSquares())
            {
                square.WasPreviousMove = false;
            }
            endSquare.Piece = this;
            startSquare.Piece = null;
            startSquare.WasPreviousMove = true;
            endSquare.WasPreviousMove = true;
            return startSquare.ChessCoordinate + endSquare.ChessCoordinate;
        }
    }
}