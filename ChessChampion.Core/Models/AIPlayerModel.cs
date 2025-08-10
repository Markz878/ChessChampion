using ChessChampion.Core.Data;
using ChessChampion.Shared;
using ChessChampion.Shared.Models;

namespace ChessChampion.Core.Models;

public class AIPlayerModel(int skillLevel, string engineFileName) : PlayerModel("Computer", false)
{
    private readonly ChessAIEngine chessAI = new(skillLevel, engineFileName);
    private ushort calculationTime = 3000;

    public void SetParameters(ushort calculationTime)
    {
        this.calculationTime = calculationTime;
    }

    public async Task<Result<string, AIMoveError>> Move(GameStateModel gameState)
    {
        string? aiMove = null;
        int retries = 0;
        string? errorMessage = null;
        while (string.IsNullOrEmpty(aiMove))
        {
            aiMove = await chessAI.GetNextMove(gameState.Moves, calculationTime);

            if (string.IsNullOrEmpty(aiMove))
            {
                retries++;
                continue;
            }
            GameSquare startSquare = gameState[aiMove[..2]];
            GameSquare endSquare = gameState[aiMove[2..4]];
            if (startSquare.Piece is null)
            {
                errorMessage = $"AI tried to move empty square {startSquare.ChessCoordinate}";
                aiMove = null;
                retries++;
                continue;
            }
            else if (!RulesService.IsPlayerPiece(startSquare.Piece, IsWhite))
            {
                errorMessage = $"AI tried to move opponent square {startSquare.ChessCoordinate}";
                aiMove = null;
                retries++;
                continue;
            }
            else if (endSquare.Piece != null && RulesService.IsPlayerPiece(endSquare.Piece, IsWhite))
            {
                errorMessage = $"AI tried to eat it's own piece at {endSquare.ChessCoordinate}";
                aiMove = null;
                retries++;
                continue;
            }
            else if (retries > 5)
            {
                return new AIMoveError(errorMessage); ;
            }
            else
            {
                return startSquare.Piece.HandleMove(gameState, startSquare, endSquare);
            }
        }
        return new AIMoveError(errorMessage);
    }
}

public readonly record struct AIMoveError(string? Error);
