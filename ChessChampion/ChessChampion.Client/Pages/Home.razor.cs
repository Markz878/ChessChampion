using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ChessChampion.Client.Pages;

public sealed partial class Home : ComponentBase
{
    [Inject] public required IJSRuntime JS { get; init; }
    [Inject] public required IConfiguration Configuration { get; init; }
    [Inject] public required ILogger<Index> Logger { get; init; }
    [Inject] public required MainViewModel ViewModel { get; init; }
    [Inject] public required APIService API { get; init; }
    [Inject] public required HubConnectionService Hub { get; init; }

    protected override void OnInitialized()
    {
        ViewModel.StateHasChanged += StateHasChanged;
    }

    private async Task LeaveGame()
    {
        if (ViewModel.GameId.HasValue && ViewModel.Player is not null && await JS.InvokeAsync<bool>("confirm", "Leave game?"))
        {
            string? response = await API.LeaveGame(new LeaveGameRequest(ViewModel.GameId.Value, ViewModel.Player.Name));
            if (string.IsNullOrEmpty(response))
            {
                ViewModel.GameState = null;
                ViewModel.GameId = null;
                ViewModel.StatusMessage = "You have left the game";
            }
            else
            {
                ViewModel.StatusMessage = response;
            }
        }
    }
    private void ChoosePlayVsHuman(bool playVsHuman)
    {
        ViewModel.PlayVsMode = playVsHuman ? PlayVsMode.PlayVsHuman : PlayVsMode.PlayVsComputer;
    }

    private void CreateGameSelectionHandler(bool chooseCreateGame)
    {
        ViewModel.JoinGameMode = chooseCreateGame ? JoinGameMode.CreateGame : JoinGameMode.JoinGame;
    }

    private void SelectWhitePieces(bool selectedWhites)
    {
        ViewModel.ChooseWhitePieces = selectedWhites;
    }

    private async Task CreateGame()
    {
        if (ViewModel.CreateGameForm.UserName is null || ViewModel.CreateGameForm.UserName.Length < 3)
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        if (ViewModel.CreateGameForm.VersusAI is true && ViewModel.CreateGameForm.AISkillLevel.HasValue is false)
        {
            ViewModel.StatusMessage = "Please select a valid AI skill level (1-20)";
            return;
        }
        Result<CreateGameResponse, string> response = await API.CreateGame(ViewModel.CreateGameForm);
        response.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.Player = new PlayerModel(ViewModel.CreateGameForm.UserName, ViewModel.CreateGameForm.IsWhites);
            ViewModel.StatusMessage = $"Your game code is {x.GameCode}";
        },
        e => ViewModel.StatusMessage = e);

        //gameCode = CreateGameCode();
        //string? userName = CreateGameForm.UserName;
        //if (string.IsNullOrEmpty(userName))
        //{
        //    StatusMessage = "Please enter your name";
        //    return;
        //}
        //Game = new();
        //Game.StateChanged += Game_OnStateChanged;
        //Game.GameEnded += Game_OnGameEnded;
        //if (ChooseWhitePieces)
        //{
        //    Player = new PlayerModel(userName, true);
        //    Game.WhitePlayer = Player;
        //}
        //else
        //{
        //    Player = new PlayerModel(userName, false);
        //    Game.BlackPlayer = Player;
        //}
        //while (!GamesService.CreateGame(gameCode.ToLower(), Game))
        //{
        //    gameCode = CreateGameCode();
        //}
        //CreateGameForm.GameCode = gameCode;
        //StatusMessage = "Waiting for the other player..";
    }



    private string GetStatusText()
    {
        //if (Game is null)
        //{
        //    return "No game in progress";
        //}
        //if (Player is null)
        //{
        //    return "No player selected";
        //}
        //if (Game.Winner == Game.WhitePlayer)
        //{
        //    return "Winner is white!";
        //}
        //else if (Game.Winner == Game.BlackPlayer)
        //{
        //    return "Winner is black!";
        //}
        //else
        //{
        //    return Player.IsWhite == Game.IsWhitePlayerTurn ? "It is your turn!" : "It is opponent's turn...";
        //}
        return "";
    }

    private string GetStatusTextColor()
    {
        //if (Player is not null && Game is not null && Game.Winner is null)
        //{
        //    return Player.IsWhite == Game.IsWhitePlayerTurn ? "player-turn-color" : "opponent-turn-color";
        //}
        //else
        //{
        return "";
        //}
    }

    private async Task JoinGame()
    {
        if (ViewModel.JoinGameForm.GameCode is null || ViewModel.JoinGameForm.GameCode.Length != 4)
        {
            ViewModel.StatusMessage = "Please enter a valid game code (4 characters)";
            return;
        }
        if (ViewModel.JoinGameForm.UserName is null || ViewModel.JoinGameForm.UserName.Length < 3)
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        Result<JoinGameResponse, string> response = await API.JoinGame(ViewModel.JoinGameForm);
        response.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.Player = new PlayerModel(ViewModel.JoinGameForm.UserName, !x.Player1.IsWhite);
            ViewModel.OtherPlayer = new PlayerModel(x.Player1.Name, x.Player1.IsWhite);
            ViewModel.StatusMessage = "Game started!";
        },
        e => ViewModel.StatusMessage = e);
    }

    private async Task StartGameVsComputer()
    {
        if (ViewModel.CreateGameForm.UserName is null || ViewModel.CreateGameForm.UserName.Length < 3)
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        Result<CreateGameResponse, string> game = await API.CreateGame(ViewModel.CreateGameForm);
        game.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.Player = new PlayerModel(ViewModel.CreateGameForm.UserName, ViewModel.ChooseWhitePieces);
            ViewModel.OtherPlayer = new PlayerModel("Stockfish", !ViewModel.ChooseWhitePieces);
            ViewModel.StatusMessage = "Game started!";
        }, e =>
        {
            ViewModel.StatusMessage = e;
        });
        //gameCode = null;
        //Player = new PlayerModel("Player", ChooseWhitePieces);
        //AIPlayerModel ai = new(SkillLevel, Configuration["EngineFileName"] ?? throw new ArgumentNullException("EngineFileName"));
        //ai.SetParameters(ushort.Parse(Configuration["AICalculationTime"] ?? throw new ArgumentNullException("AICalculationTime")));
        //Game = new GameModel()
        //{
        //    BlackPlayer = ChooseWhitePieces ? ai : Player,
        //    WhitePlayer = ChooseWhitePieces ? Player : ai
        //};
        //Game.StateChanged += Game_OnStateChanged;
        //Game.GameEnded += Game_OnGameEnded;
        //if (!ChooseWhitePieces)
        //{
        //    string? aimove = await ai.Move(Game.GameState, Logger);
        //    if (string.IsNullOrEmpty(aimove))
        //    {
        //        StatusMessage = "AI could not find a valid move";
        //        return;
        //    }
        //    Game.GameState.Moves += $" {aimove}";
        //    Game.IsWhitePlayerTurn = !Game.IsWhitePlayerTurn;
        //}
    }
}

