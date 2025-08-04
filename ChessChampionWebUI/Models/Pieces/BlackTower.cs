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

        public override IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerMovableSquares(gameState, square);
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerThreatSquares(gameState, square);
        }

        public override string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (startSquare.ChessCoordinate == "a8")
            {
                gameState.CanBlackKingCastleLeft = false;
            }
            else if (startSquare.ChessCoordinate == "h8")
            {
                gameState.CanBlackKingCastleRight = false;
            }
            return base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}