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

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBlackPawnThreatSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetBlackPawnMovableSquares(gameState, square);
        }

        public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (endSquare.Y == 7)
            {
                foreach (var square in gameState.GetSquares())
                {
                    square.WasPreviousMove = false;
                }
                startSquare.Piece = null;
                endSquare.Piece = new BlackQueen();
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
