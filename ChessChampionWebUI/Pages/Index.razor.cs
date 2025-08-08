using ChessChampionWebUI.Data;
using ChessChampionWebUI.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text;

namespace ChessChampionWebUI.Pages;

public sealed partial class Index : IDisposable
{
    [Inject] public required GamesService GamesService { get; init; }
    [Inject] public required IJSRuntime JS { get; init; }
    [Inject] public required IConfiguration Configuration { get; init; }
    [Inject] public required ILogger<Index> Logger { get; init; }
    public GameModel? Game { get; set; }
    public CreateGameFormModel CreateGameForm { get; set; } = new();
    public JoinGameFormModel JoinGameForm { get; set; } = new();
    public PlayerModel? Player { get; set; }
    public PlayVsMode PlayVsMode { get; set; } = PlayVsMode.PlayVsHuman;
    public JoinGameMode JoinGameMode { get; set; }
    public bool ChooseWhitePieces { get; set; } = true;
    public int SkillLevel { get; set; } = 10;
    public string? StatusMessage { get; set; }

    private readonly Random random = new();

    private string? gameCode;

    protected override void OnInitialized()
    {
        if (!File.Exists(Configuration["EngineFileName"]))
        {
            throw new FileNotFoundException("Stockfish engine not found");
        }
    }

    private async Task ResetState()
    {
        if (Game is not null && Player is not null && await JS.InvokeAsync<bool>("confirm", "Leave game?"))
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
        string? userName = CreateGameForm.UserName;
        if (string.IsNullOrEmpty(userName))
        {
            StatusMessage = "Please enter your name";
            return;
        }
        Game = new();
        Game.StateChanged += Game_OnStateChanged;
        Game.GameEnded += Game_OnGameEnded;
        if (ChooseWhitePieces)
        {
            Player = new PlayerModel(userName, true);
            Game.WhitePlayer = Player;
        }
        else
        {
            Player = new PlayerModel(userName, false);
            Game.BlackPlayer = Player;
        }
        while (!GamesService.CreateGame(gameCode.ToLower(), Game))
        {
            gameCode = CreateGameCode();
        }
        CreateGameForm.GameCode = gameCode;
        StatusMessage = "Waiting for the other player..";
    }

    private void Game_OnStateChanged()
    {
        Task.Run(() => InvokeAsync(StateHasChanged));
    }

    private void ChoosePlayVsHuman(bool playVsHuman)
    {
        PlayVsMode = playVsHuman ? PlayVsMode.PlayVsHuman : PlayVsMode.PlayVsComputer;
    }

    private string GetStatusText()
    {
        if (Game is null)
        {
            return "No game in progress";
        }
        if (Player is null)
        {
            return "No player selected";
        }
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
        if (Player is not null && Game is not null && Game.Winner is null)
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
        if (gameCode is not null && JoinGameForm.UserName is not null && GamesService.TryGetGame(gameCode, out GameModel? game) && game is not null)
        {
            if (game.WhitePlayer == null)
            {
                Player = new PlayerModel(JoinGameForm.UserName, true);
                game.WhitePlayer = Player;
            }
            else if (game.BlackPlayer == null)
            {
                Player = new PlayerModel(JoinGameForm.UserName, false);
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

    private void Game_OnGameEnded()
    {
        if (!string.IsNullOrEmpty(gameCode))
        {
            GamesService.DeleteGame(gameCode);
        }
    }

    private async Task StartGameVsComputer()
    {
        gameCode = null;
        Player = new PlayerModel("Player", ChooseWhitePieces);
        AIPlayerModel ai = new(SkillLevel, Configuration["EngineFileName"] ?? throw new ArgumentNullException("EngineFileName"));
        ai.SetParameters(ushort.Parse(Configuration["AICalculationTime"] ?? throw new ArgumentNullException("AICalculationTime")));
        Game = new GameModel()
        {
            BlackPlayer = ChooseWhitePieces ? ai : Player,
            WhitePlayer = ChooseWhitePieces ? Player : ai
        };
        Game.StateChanged += Game_OnStateChanged;
        Game.GameEnded += Game_OnGameEnded;
        if (!ChooseWhitePieces)
        {
            string? aimove = await ai.Move(Game.GameState, Logger);
            if (string.IsNullOrEmpty(aimove))
            {
                throw new InvalidOperationException("AI could not make a move");
            }
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
