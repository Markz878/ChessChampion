using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class BlackTower : ChessPiece
    {
        public BlackTower()
        {
            Marker = "♜";
            IsWhite = false;
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (startSquare.ChessCoordinate == "a8")
            {
                gameState.CanBlackKingCastleLeft = false;
            }
            else if (startSquare.ChessCoordinate == "h8")
            {
                gameState.CanBlackKingCastleRight = false;
            }
            base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}