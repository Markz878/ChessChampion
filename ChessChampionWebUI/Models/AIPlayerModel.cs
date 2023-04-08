using ChessChampionWebUI.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel
    {
        private readonly ChessAIEngine chessAI;
        private ushort calculationTime = 3000;

        public AIPlayerModel(int skillLevel, string engineFileName)
        {
            Name = "Computer";
            chessAI = new(skillLevel, engineFileName);
        }

        public void SetParameters(ushort calculationTime)
        {
            this.calculationTime = calculationTime;
        }

        public async Task<string> Move(GameStateModel gameState, ILogger logger)
        {
            string aiMove = "";
            int retries = 0;
            while (string.IsNullOrEmpty(aiMove))
            {
                logger.LogInformation("Given moves to AI are {0}", gameState.Moves);
                aiMove = await chessAI.GetNextMove(gameState.Moves, calculationTime);
                logger.LogInformation("AI returned move {0}", aiMove);

                if (string.IsNullOrEmpty(aiMove))
                {
                    logger.LogError("Could not find best move");
                    retries++;
                    continue;
                }
                GameSquare startSquare = gameState[aiMove[..2]];
                GameSquare endSquare = gameState[aiMove[2..4]];
                if (startSquare.IsEmpty)
                {
                    logger.LogError("AI tried to move empty square {0}", startSquare.ChessCoordinate);
                    aiMove = null;
                    retries++;
                    continue;
                }
                else if (!RulesService.IsPlayerPiece(startSquare.Piece.Marker, IsWhite))
                {
                    logger.LogError("AI tried to move opponent square {0}", startSquare.ChessCoordinate);
                    aiMove = null;
                    retries++;
                    continue;
                }
                else if (endSquare.Piece != null && RulesService.IsPlayerPiece(endSquare.Piece.Marker, IsWhite))
                {
                    logger.LogError("AI tried to eat it's own piece at {0}", endSquare.ChessCoordinate);
                    aiMove = null;
                    retries++;
                    continue;
                }
                else if (retries > 5)
                {
                    throw new ArgumentException("Could not find move in the given response:");
                }
                else
                {
                    return startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
                }
            }
            return null;
        }
    }
}
