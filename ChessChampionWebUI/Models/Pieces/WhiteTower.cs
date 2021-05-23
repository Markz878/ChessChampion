using System.Collections.Generic;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models.Pieces
{
    public class WhiteTower : ChessPiece
    {
        public WhiteTower()
        {
            Marker = "♖";
            IsWhite = true;
        }

        public override IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerSquares(gameState, square);
        }

        public override void HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            if (startSquare.ChessCoordinate=="a1")
            {
                gameState.CanWhiteKingCastleLeft = false;
            }
            else if (startSquare.ChessCoordinate == "h1")
            {
                gameState.CanWhiteKingCastleRight = false;
            }
            base.HandleMove(gameState, startSquare, endSquare);
        }
    }
}