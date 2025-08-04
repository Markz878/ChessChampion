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

        public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (endSquare.Y == 0)
            {
                foreach (var square in gameState.GetSquares())
                {
                    square.WasPreviousMove = false;
                }
                startSquare.Piece = null;
                endSquare.Piece = new WhiteQueen();
                startSquare.WasPreviousMove = true;
                endSquare.WasPreviousMove = true;
                return startSquare.ChessCoordinate + endSquare.ChessCoordinate + "q";
            }
            else
            {
                return base.HandleMove(gameState, startSquare, endSquare);
            }
        }
    }
}
