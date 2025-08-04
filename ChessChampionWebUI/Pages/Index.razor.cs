using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Pages
{
    public sealed partial class Index : IDisposable
    {
        [Inject] public GamesService GamesService { get; set; }
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public IConfiguration Configuration { get; set; }
        [Inject] public ILogger<Index> Logger { get; set; }
        public GameModel Game { get; set; }
        public CreateGameFormModel CreateGameForm { get; set; } = new();
        public JoinGameFormModel JoinGameForm { get; set; } = new();
        public PlayerModel Player { get; set; }
        public PlayVsMode PlayVsMode { get; set; } = PlayVsMode.PlayVsHuman;
        public JoinGameMode JoinGameMode { get; set; }
        public bool ChooseWhitePieces { get; set; } = true;
        public int SkillLevel { get; set; } = 10;
        public string StatusMessage { get; set; }

        private readonly Random random = new();

        private string gameCode;

        protected override void OnInitialized()
        {
            if (!File.Exists(Configuration["EngineFileName"]))
            {
                throw new FileNotFoundException("Stockfish engine not found");
            }
        }

        private async Task ResetState()
        {
            if (Game != null && await JS.InvokeAsync<bool>("confirm", "Leave game?"))
            {
                Game.Winner = Player.IsWhite ? Game.BlackPlayer : Game.WhitePlayer;
                Game.OnGameEnded();
                Game.OnStateChanged();
                Dispose();
                Game = null;
            }
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
            gameCode = CreateGameCode();
            Game = new();
            Game.StateChanged += Game_OnStateChanged;
            Game.GameEnded += Game_OnGameEnded;
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
            while (!GamesService.CreateGame(gameCode.ToLower(), Game))
            {
                gameCode = CreateGameCode();
            }
            CreateGameForm.GameCode = gameCode;
            StatusMessage = "Waiting for the other player..";
        }

        private async void Game_OnStateChanged(object sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
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

        private string GetStatusTextColor()
        {
            if (Game.Winner == null)
            {
                return Player.IsWhite == Game.IsWhitePlayerTurn ? "player-turn-color" : "opponent-turn-color";
            }
            else
            {
                return "";
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
            gameCode = JoinGameForm.GameCode;
            if (GamesService.TryGetGame(gameCode, out GameModel game))
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
                Game.StateChanged += Game_OnStateChanged;
                Game.GameEnded += Game_OnGameEnded;
                Game.OnStateChanged();
            }
            else
            {
                StatusMessage = "No game with that code found";
            }
        }

        private void Game_OnGameEnded(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(gameCode))
            {
                GamesService.DeleteGame(gameCode);
            }
        }

        private async Task StartGameVsComputer()
        {
            gameCode = null;
            Player = new PlayerModel() { Name = "Player", IsWhite = ChooseWhitePieces };
            AIPlayerModel ai = new(SkillLevel, Configuration["EngineFileName"]);
            ai.SetParameters(ushort.Parse(Configuration["AICalculationTime"]));
            Game = new GameModel()
            {
                BlackPlayer = ChooseWhitePieces ? ai : Player,
                WhitePlayer = ChooseWhitePieces ? Player : ai
            };
            Game.StateChanged += Game_OnStateChanged;
            Game.GameEnded += Game_OnGameEnded;
            if (!ChooseWhitePieces)
            {
                string aimove = await ai.Move(Game.GameState, Logger);
                Game.GameState.Moves += $" {aimove}";
                Game.IsWhitePlayerTurn = !Game.IsWhitePlayerTurn;
            }
        }

        public void Dispose()
        {
            if (Game != null)
            {
                Game.StateChanged -= Game_OnStateChanged;
                Game.GameEnded -= Game_OnGameEnded;
            }
        }
    }
}
