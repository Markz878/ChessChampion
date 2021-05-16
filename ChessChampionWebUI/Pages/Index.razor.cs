using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Pages
{
    public partial class Index
    {
        [Inject] public GamesService GamesService { get; set; }
        public GameModel Game { get; set; }
        public PlayerModel User { get; set; } = new PlayerModel();
        public PlayVsMode PlayVsMode { get; set; } = PlayVsMode.PlayVsHuman;
        public JoinGameMode JoinGameMode { get; set; } = JoinGameMode.CreateGame;
        public bool ChooseWhitePieces { get; set; } = true;

        private readonly Random random = new();

        private void CreateGameSelectionHandler(bool chooseCreateGame)
        {
            JoinGameMode = chooseCreateGame ? JoinGameMode.CreateGame : JoinGameMode.JoinGame;
        }

        private void SelectWhitePieces(bool selectedWhites)
        {
            ChooseWhitePieces = selectedWhites;
        }

        private void CreateGame()
        {
            string gameCode = CreateGameCode();
            Game = new GameModel() { IsPlayerWhite = ChooseWhitePieces, WhitePlayer = ChooseWhitePieces ? User : null, BlackPlayer = !ChooseWhitePieces ? User : null };
            while (!GamesService.Games.TryAdd(gameCode, Game))
            {
                gameCode = CreateGameCode();
            }
            User.GameCode = gameCode;
        }

        private void ChoosePlayVsHuman(bool playVsHuman)
        {
            PlayVsMode = playVsHuman ? PlayVsMode.PlayVsHuman : PlayVsMode.PlayVsComputer;
        }

        private string CreateGameCode()
        {
            StringBuilder builder = new();
            for (int i = 0; i < 4; i++)
            {
                builder.Append(char.ConvertFromUtf32(random.Next(65, 91)));
            }
            return builder.ToString();
        }

        private void JoinGame()
        {
            if (GamesService.Games.TryGetValue(User.GameCode, out GameModel game))
            {
                Game = game;
            }
        }

        private async Task StartGameVsComputer()
        {
            Game = GamesService.CreateGameVsComputer(ChooseWhitePieces, User);
            if (!ChooseWhitePieces)
            {
                await (Game.Opponent as AIPlayerModel).Move("");
            }
        }
    }
}
