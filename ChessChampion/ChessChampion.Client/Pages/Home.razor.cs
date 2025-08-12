using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
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
    [Inject] public required NavigationManager Navigation { get; init; }

    private HubConnection? _hubConnection;

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
        ViewModel.Player.IsWhite = selectedWhites;
    }

    private async Task CreateGame()
    {
        if (ViewModel.Player.Name is null || ViewModel.Player.Name.Length < 3)
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        if (ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer && ViewModel.SkillLevel is < 0 or > 20)
        {
            ViewModel.StatusMessage = "Please select a valid AI skill level (1-20)";
            return;
        }
        CreateGameRequest request = new()
        {
            UserName = ViewModel.Player.Name,
            AISkillLevel = ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer ? ViewModel.SkillLevel : null,
            IsWhites = ViewModel.Player.IsWhite,
            VersusAI = ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer
        };
        Result<CreateGameResponse, string> response = await API.CreateGame(request);
        response.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.StatusMessage = $"Your game code is {x.GameCode}";
        },
        e => ViewModel.StatusMessage = e);
        if (response.IsSuccess && ViewModel.GameId.HasValue)
        {
            _hubConnection = Hub.BuildHubConnection(Navigation.ToAbsoluteUri("/chesshub"));
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync(nameof(IChessHubClientActions.JoinGame), ViewModel.GameId.Value);
        }
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
        if (ViewModel.GameCode is null || ViewModel.GameCode.Length != 4)
        {
            ViewModel.StatusMessage = "Please enter a valid game code (4 characters)";
            return;
        }
        if (ViewModel.Player.Name is null || ViewModel.Player.Name.Length < 3)
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        JoinGameRequest joinGameRequest = new()
        {
            GameCode = ViewModel.GameCode.ToLower(),
            UserName = ViewModel.Player.Name,
        };
        Result<JoinGameResponse, string> response = await API.JoinGame(joinGameRequest);
        response.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.Player = new PlayerModel(ViewModel.Player.Name, !x.Player1.IsWhite);
            ViewModel.OtherPlayer = new PlayerModel(x.Player1.Name, x.Player1.IsWhite);
            ViewModel.StatusMessage = "Game started!";
        },
        e => ViewModel.StatusMessage = e);
    }

    private async Task StartGameVsComputer()
    {
        if (ViewModel.SkillLevel is < 0 or > 20)
        {
            ViewModel.StatusMessage = "Please select a valid AI skill level (1-20)";
            return;
        }
        ViewModel.Player.Name = "Player";
        CreateGameRequest request = new()
        {
            UserName = ViewModel.Player.Name,
            AISkillLevel = ViewModel.SkillLevel,
            IsWhites = ViewModel.Player.IsWhite,
            VersusAI = true
        };
        Result<CreateGameResponse, string> gameResponse = await API.CreateGame(request);
        gameResponse.Handle(x =>
        {
            ViewModel.GameId = x.Id;
            ViewModel.GameState = x.GameState;
            ViewModel.OtherPlayer = new PlayerModel("Stockfish", !ViewModel.Player.IsWhite);
            ViewModel.StatusMessage = "Game started!";
        }, e =>
        {
            ViewModel.StatusMessage = e;
        });
        if (gameResponse.IsSuccess && ViewModel.GameId.HasValue)
        {
            _hubConnection = Hub.BuildHubConnection(Navigation.ToAbsoluteUri("/chesshub"));
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync(nameof(IChessHubClientActions.JoinGame), ViewModel.GameId.Value);
        }
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

