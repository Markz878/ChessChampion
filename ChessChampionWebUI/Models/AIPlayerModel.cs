using ChessChampionWebUI.Data;

namespace ChessChampionWebUI.Models;

public class AIPlayerModel(int skillLevel, string engineFileName) : PlayerModel("Computer", false)
{
    private readonly ChessAIEngine chessAI = new(skillLevel, engineFileName);
    private ushort calculationTime = 3000;

    public void SetParameters(ushort calculationTime)
    {
        this.calculationTime = calculationTime;
    }

    public async Task<string?> Move(GameStateModel gameState, ILogger logger)
    {
        string? aiMove = null;
        int retries = 0;
        while (string.IsNullOrEmpty(aiMove))
        {
            logger.LogInformation("Given moves to AI are {Moves}", gameState.Moves);
            aiMove = await chessAI.GetNextMove(gameState.Moves, calculationTime);
            logger.LogInformation("AI returned move {AiMove}", aiMove);

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
                logger.LogError("AI tried to move empty square {ChessCoordinate}", startSquare.ChessCoordinate);
                aiMove = null;
                retries++;
                continue;
            }
            else if (!RulesService.IsPlayerPiece(startSquare.Piece, IsWhite))
            {
                logger.LogError("AI tried to move opponent square {ChessCoordinate}", startSquare.ChessCoordinate);
                aiMove = null;
                retries++;
                continue;
            }
            else if (endSquare.Piece != null && RulesService.IsPlayerPiece(endSquare.Piece, IsWhite))
            {
                logger.LogError("AI tried to eat it's own piece at {ChessCoordinate}", endSquare.ChessCoordinate);
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
                return startSquare.Piece?.HandleMove(gameState, startSquare, endSquare);
            }
        }
        return null;
    }
}
