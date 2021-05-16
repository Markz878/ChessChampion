using ChessChampionWebUI.Data;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class AIPlayerModel : PlayerModel
    {
        private readonly ChessAIEngine chessAI = new();

        public AIPlayerModel()
        {
            Name = "Computer";
        }

        public async Task<string> Move(string playerMove)
        {
            string aiMove = await chessAI.GetNextMove(playerMove, 3000);
            return aiMove;
        }
    }
}
