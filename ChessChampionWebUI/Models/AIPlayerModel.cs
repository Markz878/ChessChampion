using ChessChampionWebUI.Data;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel, IDisposable
    {
        private readonly ChessAIEngine chessAI = new();
        private ushort calculationTime = 3000;
        public AIPlayerModel()
        {
            Name = "Computer";
        }

        public async Task SetParameters(int level, ushort calculationTime)
        {
            await chessAI.SetDifficulty(level);
            this.calculationTime = calculationTime;
        }

        public void Dispose()
        {
            chessAI.Dispose();
        }

        public async Task Move(GameStateModel gameState, string playerMove)
        {
            string aiMove = await chessAI.GetNextMove(playerMove, calculationTime);
            GameSquare startSquare = gameState[aiMove[..2]];
            GameSquare endSquare = gameState[aiMove[2..4]];
            System.Diagnostics.Trace.TraceError($"AI move is {aiMove}.");
            try
            {
                startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
            }
            catch
            {
                System.Diagnostics.Trace.TraceError($"AI move failed, was {aiMove}");
                throw;
            }
        }
    }
}
