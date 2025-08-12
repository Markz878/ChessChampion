using ChessChampion.Core.Data;
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

    public async Task<Result<string, MoveError>> Move(GameModel game)
    {
        string aiMove = await chessAI.GetNextMove(game.GameState.Moves, calculationTime);

        MoveError? error = game.TryMakeMove(aiMove);

        return error is null ? aiMove : error.Value;
    }
}
