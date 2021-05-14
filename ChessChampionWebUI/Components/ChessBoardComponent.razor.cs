using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace ChessChampionWebUI.Components
{
    public partial class ChessBoardComponent
    {
        [Parameter] public GameModel Game { get; set; }

        public void HandleClick(GameSquare square)
        {
            Game.HandleSquareSelect(square);

        }
    }
}
