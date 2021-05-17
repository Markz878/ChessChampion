using ChessChampionWebUI.Data;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel
    {
        private readonly ChessAIEngine chessAI = new();

        public AIPlayerModel(int level)
        {
            Name = "Computer";
            chessAI.SetDifficulty(level);
        }

        public async Task Move(GameStateModel gameState, string playerMove)
        {
            string aiMove = await chessAI.GetNextMove(playerMove, 3000);
            GameSquare startSquare = gameState[aiMove[..2]];
            GameSquare endSquare = gameState[aiMove[2..4]];
            startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
        }
    }
}
