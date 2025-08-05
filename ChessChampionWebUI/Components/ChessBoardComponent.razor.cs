using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;

namespace ChessChampionWebUI.Components;

public partial class ChessBoardComponent
{
    [Parameter][EditorRequired] public required GameModel Game { get; set; }
    [Parameter][EditorRequired] public required PlayerModel Player { get; set; }
    [Inject] public required ILogger<ChessBoardComponent> Logger { get; set; }
    public async Task HandleClick(GameSquare square)
    {
        if (Game.Winner == null)
        {
            try
            {
                await Game.HandleSquareSelect(square, Player, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
