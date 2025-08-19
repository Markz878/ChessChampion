using ChessChampion.Client.Models;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ChessChampion.Client.Pages;

public sealed partial class Home
{
    [Inject] public required IJSRuntime JS { get; init; }
    [Inject] public required MainViewModel ViewModel { get; init; }
    [Inject] public required IChessService ChessService { get; set; }
    [Inject] public required IHubConnectionService Hub { get; set; }

    protected override void OnInitialized()
    {
        if (RendererInfo.IsInteractive)
        {
            ViewModel.StateHasChanged += StateHasChanged;
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
        if (ViewModel.PlayVsMode == PlayVsMode.PlayVsHuman && (ViewModel.Player.Name is null || ViewModel.Player.Name.Length < 3))
        {
            ViewModel.StatusMessage = "Please enter a valid username (at least 3 characters)";
            return;
        }
        if (ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer && ViewModel.SkillLevel is < 1 or > 20)
        {
            ViewModel.StatusMessage = "Please select a valid AI skill level (1-20)";
            return;
        }
        if (ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer)
        {
            ViewModel.Player.Name = "Player";
        }
        CreateGameRequest request = new()
        {
            UserName = ViewModel.Player.Name,
            AISkillLevel = ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer ? ViewModel.SkillLevel : null,
            IsWhites = ViewModel.Player.IsWhite,
            VersusAI = ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer
        };

        Result<CreateGameResponse, BaseError> result = await ChessService.CreateGame(request);
        await result.HandleAsync(
            async createGameResponse =>
            {
                ViewModel.GameId = createGameResponse.Id;
                ViewModel.Winner = null;
                ViewModel.GameCode = createGameResponse.GameCode;
                ViewModel.GameState = createGameResponse.GameState;
                ViewModel.OtherPlayer = request.VersusAI ? new PlayerModel("Stockfish", !ViewModel.Player.IsWhite) : null;
                ViewModel.StatusMessage = request.VersusAI ?
                    ViewModel.Player.IsWhite ? MainViewModel.PlayerTurnText : MainViewModel.OtherPlayerTurnText :
                    "Waiting for the other player...";
                await Hub.JoinGame(createGameResponse.Id);
            },
            error =>
            {
                ViewModel.StatusMessage = error.Error;
                return Task.CompletedTask;
            }
        );
    }

    private string GetStatusTextColor()
    {
        if (ViewModel.Player is { } player && ViewModel.OtherPlayer is { } && ViewModel.GameState is { } game && ViewModel.Winner is null)
        {
            return player.IsWhite == game.IsWhitePlayerTurn ? "player-turn-color" : "opponent-turn-color";
        }
        else
        {
            return "";
        }
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

        Result<JoinGameResponse, BaseError> result = await ChessService.JoinGame(joinGameRequest);
        await result.HandleAsync(
            async joinGameResponse =>
            {
                ViewModel.GameId = joinGameResponse.Id;
                ViewModel.GameState = joinGameResponse.GameState;
                ViewModel.Winner = null;
                ViewModel.Player.IsWhite = !joinGameResponse.Player1.IsWhite;
                ViewModel.OtherPlayer = joinGameResponse.Player1;
                ViewModel.StatusMessage = ViewModel.Player.IsWhite ? MainViewModel.PlayerTurnText : MainViewModel.OtherPlayerTurnText;
                await Hub.JoinGame(joinGameResponse.Id);
            },
            error =>
            {
                ViewModel.StatusMessage = error.Error;
                return Task.CompletedTask;
            }
        );
    }

    private async Task LeaveGame()
    {
        if (ViewModel.GameId.HasValue && ViewModel.Player is not null && await JS.InvokeAsync<bool>("confirm", "Leave game?"))
        {
            Guid gameIdToLeave = ViewModel.GameId.Value;
            BaseError? error = await ChessService.LeaveGame(new LeaveGameRequest(gameIdToLeave, ViewModel.Player.Name));
            ViewModel.GameState = null;
            ViewModel.GameId = null;
            ViewModel.Winner = null;
            ViewModel.OtherPlayer = null;
            ViewModel.GameCode = "";
            ViewModel.StatusMessage = error.HasValue ? error.Value.Error : "You have left the game";
            await Hub.LeaveGame(gameIdToLeave);
        }
    }
}

