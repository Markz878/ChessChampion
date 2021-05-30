using ChessChampionWebUI.Data;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel, IDisposable
    {
        private readonly ChessAIEngine chessAI = new();

        public AIPlayerModel()
        {
            Name = "Computer";
        }

        public async Task SetDifficultyLevel(int level)
        {
            await chessAI.SetDifficulty(level);
        }

        public void Dispose()
        {
            chessAI.Dispose();
        }

        public async Task Move(GameStateModel gameState, string playerMove)
        {
            string aiMove = await chessAI.GetNextMove(playerMove, 3000);
            GameSquare startSquare = gameState[aiMove[..2]];
            GameSquare endSquare = gameState[aiMove[2..4]];
            try
            {
                startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex.Message}, startSquare was {startSquare.ChessCoordinate}, endSquare was {endSquare.ChessCoordinate} ");
                throw;
            }
        }
    }
}
