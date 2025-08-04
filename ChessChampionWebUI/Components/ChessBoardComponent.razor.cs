using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Components
{
    public partial class ChessBoardComponent
    {
        [Parameter] public GameModel Game { get; set; }
        [Parameter] public PlayerModel Player { get; set; }
        [Inject] public ILogger<ChessBoardComponent> Logger { get; set; }
        public async Task HandleClick(GameSquare square)
        {
            if (Game.Winner == null)
            {
                try
                {
                    await Game.HandleSquareSelect(square, Player, Logger, 10);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                    throw;
                }
            }
        }
    }
}
