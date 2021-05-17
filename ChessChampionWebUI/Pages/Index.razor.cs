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
        public CreateGameFormModel CreateGameForm { get; set; } = new();
        public JoinGameFormModel JoinGameForm { get; set; } = new();
        public PlayVsMode PlayVsMode { get; set; } = PlayVsMode.PlayVsHuman;
        public JoinGameMode JoinGameMode { get; set; }
        public bool ChooseWhitePieces { get; set; } = true;
        public int SkillLevel { get; set; } = 10;
        public string StatusMessage { get; set; }

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
            Game = new GameModel() { IsPlayerWhite = ChooseWhitePieces, WhitePlayer = ChooseWhitePieces ? new PlayerModel() { Name = CreateGameForm.UserName } : null, BlackPlayer = !ChooseWhitePieces ? new PlayerModel() { Name = CreateGameForm.UserName } : null };
            while (!GamesService.Games.TryAdd(gameCode, Game))
            {
                gameCode = CreateGameCode();
            }
            CreateGameForm.GameCode = gameCode;
            StatusMessage = "Waiting for the other player..";
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
            if (GamesService.Games.TryGetValue(JoinGameForm.GameCode, out GameModel game))
            {
                if (game.WhitePlayer == null)
                {
                    game.WhitePlayer = new PlayerModel() { Name = JoinGameForm.UserName };
                    Game = game;
                }
                else if(game.BlackPlayer == null)
                {
                    game.BlackPlayer = new PlayerModel() { Name = JoinGameForm.UserName };
                    Game = game;
                }
                else
                {
                    StatusMessage = "No game with that code found";
                }
            }
        }

        private async Task StartGameVsComputer()
        {
            Game = GamesService.CreateGameVsComputer(ChooseWhitePieces, new PlayerModel() { Name = CreateGameForm.UserName }, SkillLevel);
            if (!ChooseWhitePieces)
            {
                await (Game.Opponent as AIPlayerModel).Move(Game.GameState, "");
                Game.IsWhitePlayerTurn = !Game.IsWhitePlayerTurn;
            }
        }

        private void Test()
        {
            StateHasChanged();
        }
    }
}
