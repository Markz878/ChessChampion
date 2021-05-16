using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Components
{
    public partial class ChessBoardComponent
    {
        [Parameter] public GameModel Game { get; set; }

        public async Task HandleClick(GameSquare square)
        {
            await Game.HandleSquareSelect(square);
        }
    }
}
