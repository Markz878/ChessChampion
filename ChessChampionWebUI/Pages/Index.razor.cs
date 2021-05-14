using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;

namespace ChessChampionWebUI.Pages
{
    public partial class Index
    {
        [Inject] public GamesService GamesService { get; set; }
        public GameModel SelectedGame { get; set; }

        protected override void OnInitialized()
        {
            GamesService.Games.TryAdd("Test", new GameModel());
            SelectedGame = GamesService.Games["Test"];
            base.OnInitialized();
        }
    }
}
