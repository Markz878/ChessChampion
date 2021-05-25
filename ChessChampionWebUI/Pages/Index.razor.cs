using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] public GamesService GamesService { get; set; }
        public GameModel Game { get; set; }
        public CreateGameFormModel CreateGameForm { get; set; } = new();
        public JoinGameFormModel JoinGameForm { get; set; } = new();
        public PlayerModel Player { get; set; }
        public PlayVsMode PlayVsMode { get; set; } = PlayVsMode.PlayVsHuman;
        public JoinGameMode JoinGameMode { get; set; }
        public bool ChooseWhitePieces { get; set; } = true;
        public int SkillLevel { get; set; } = 10;
        public string StatusMessage { get; set; }
        public PlayerModel Opponent => Player.IsWhite ? Game.BlackPlayer : Game.WhitePlayer;

        private readonly Random random = new();

        protected override void OnInitialized()
        {
            if (!File.Exists("stockfish_13_win_x64.exe"))
            {
                throw new FileNotFoundException("Stockfish engine not found");
            }
        }
        private void ResetState()
        {
            Dispose();
            Game = null;
        }

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
            Game = new();
            Game.OnStateChanged += Game_OnStateChanged;
            if (ChooseWhitePieces)
            {
                Player = new PlayerModel() { Name = CreateGameForm.UserName, IsWhite = true };
                Game.WhitePlayer = Player;
            }
            else
            {
                Player = new PlayerModel() { Name = CreateGameForm.UserName, IsWhite = false };
                Game.BlackPlayer = Player;
            }
            while (!GamesService.Games.TryAdd(gameCode, Game))
            {
                gameCode = CreateGameCode();
            }
            CreateGameForm.GameCode = gameCode;
            StatusMessage = "Waiting for the other player..";
        }

        private async void Game_OnStateChanged(object sender, EventArgs e)
        {
            await InvokeAsync(() => StateHasChanged());
        }

        private void ChoosePlayVsHuman(bool playVsHuman)
        {
            PlayVsMode = playVsHuman ? PlayVsMode.PlayVsHuman : PlayVsMode.PlayVsComputer;
        }

        private string GetStatusText()
        {
            if (Game.Winner == Game.WhitePlayer)
            {
                return "Winner is white!";
            }
            else if (Game.Winner == Game.BlackPlayer)
            {
                return "Winner is black!";
            }
            else
            {
                return Player.IsWhite == Game.IsWhitePlayerTurn ? "It is your turn!" : "It is opponent's turn...";
            }
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
                    Player = new PlayerModel() { Name = JoinGameForm.UserName, IsWhite = true };
                    game.WhitePlayer = Player;
                }
                else if (game.BlackPlayer == null)
                {
                    Player = new PlayerModel() { Name = JoinGameForm.UserName, IsWhite = false };
                    game.BlackPlayer = Player;
                }
                Game = game;
                Game.OnStateChanged += Game_OnStateChanged;
                Game.NotifyOfChange();
            }
            else
            {
                StatusMessage = "No game with that code found";
            }
        }

        private async Task StartGameVsComputer()
        {
            Player = new PlayerModel() { Name = "Player", IsWhite = ChooseWhitePieces };
            AIPlayerModel ai = new(SkillLevel);
            Game = new GameModel()
            {
                BlackPlayer = ChooseWhitePieces ? ai : Player,
                WhitePlayer = ChooseWhitePieces ? Player : ai
            };
            Game.OnStateChanged += Game_OnStateChanged;
            if (!ChooseWhitePieces)
            {
                await ai.Move(Game.GameState, "");
                Game.IsWhitePlayerTurn = !Game.IsWhitePlayerTurn;
            }
        }

        public void Dispose()
        {
            if (Game!=null)
            {
                Game.OnStateChanged -= Game_OnStateChanged;
            }
        }
    }
}
