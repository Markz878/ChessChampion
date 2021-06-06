using ChessChampionWebUI.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel, IDisposable
    {
        private readonly ChessAIEngine chessAI;
        private ushort calculationTime = 3000;

        public AIPlayerModel()
        {
            Name = "Computer";
            chessAI = new();
        }

        public async Task SetParameters(int level, ushort calculationTime)
        {
            await chessAI.SetParameters(level);
            this.calculationTime = calculationTime;
        }

        public void Dispose()
        {
            chessAI.Dispose();
        }

        public async Task<string> Move(GameStateModel gameState, ILogger logger)
        {
            string aiMove = await chessAI.GetNextMove(gameState, calculationTime, logger);
            GameSquare startSquare = gameState[aiMove[..2]];
            GameSquare endSquare = gameState[aiMove[2..4]];
            try
            {
                return startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"AI move failed, was {aiMove}. Exception: {ex}");
            }
        }
    }
}
