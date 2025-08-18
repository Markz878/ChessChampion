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
        if(ViewModel.PlayVsMode == PlayVsMode.PlayVsComputer)
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
        await ChessService.CreateGame(request);
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
        await ChessService.JoinGame(joinGameRequest);
    }

    private async Task LeaveGame()
    {
        if (ViewModel.GameId.HasValue && ViewModel.Player is not null && await JS.InvokeAsync<bool>("confirm", "Leave game?"))
        {
            await ChessService.LeaveGame(new LeaveGameRequest(ViewModel.GameId.Value, ViewModel.Player.Name));
        }
    }
}

