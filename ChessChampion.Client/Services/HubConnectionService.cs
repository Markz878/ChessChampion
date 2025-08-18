using ChessChampion.Client.Models;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessChampion.Client.Services;

public sealed class HubConnectionService(MainViewModel viewModel, NavigationManager navigation) : IHubConnectionService
{
    private HubConnection? _hubConnection;

    public async Task JoinGame(Guid gameId)
    {
        _hubConnection ??= BuildHubConnection();
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
        await _hubConnection.InvokeAsync(nameof(IChessHubClientActions.JoinGame), gameId);
    }

    public async Task LeaveGame(Guid gameId)
    {
        if (_hubConnection is not null && _hubConnection.State != HubConnectionState.Disconnected)
        {
            await _hubConnection.InvokeAsync(nameof(IChessHubClientActions.LeaveGame), gameId);
            await _hubConnection.StopAsync();
        }
    }

    public string? ConnectionId() => _hubConnection?.ConnectionId;

    private HubConnection BuildHubConnection()
    {
        HubConnection hub = new HubConnectionBuilder()
            .WithUrl(navigation.ToAbsoluteUri("/chesshub"))
            .WithAutomaticReconnect()
            .Build();

        hub.On<string>(nameof(IChessHubNotifications.PlayerJoined), (playerName) =>
        {
            bool otherPlayerIsWhite = !viewModel.Player.IsWhite;
            viewModel.StatusMessage = otherPlayerIsWhite ? MainViewModel.OtherPlayerTurnText : MainViewModel.PlayerTurnText;
            viewModel.OtherPlayer = new PlayerModel(playerName, otherPlayerIsWhite);
        });

        hub.On<string>(nameof(IChessHubNotifications.MoveReceived), (move) =>
        {
            if (viewModel.GameState is null)
            {
                return;
            }
            GameSquare startSquare = viewModel.GameState[move[..2]];
            GameSquare endSquare = viewModel.GameState[move[2..4]];
            if (startSquare.Piece is null)
            {
                viewModel.StatusMessage = "Invalid move received.";
                return;
            }
            startSquare.Piece.HandleMove(viewModel.GameState, startSquare, endSquare);
            viewModel.GameState.Moves += " " + move;
            viewModel.GameState.IsWhitePlayerTurn = !viewModel.GameState.IsWhitePlayerTurn;
            viewModel.StatusMessage = MainViewModel.PlayerTurnText;
            viewModel.StateHasChanged?.Invoke();
        });

        hub.On(nameof(IChessHubNotifications.GameOver), (bool whiteWon) =>
        {
            viewModel.Winner = whiteWon ? viewModel.WhitePlayer : viewModel.BlackPlayer;
            viewModel.StatusMessage = $"Game over! {viewModel.Winner?.Name} wins!";
        });

        hub.On(nameof(IChessHubNotifications.PlayerLeft), (string leaverName) =>
        {
            viewModel.StatusMessage = $"{leaverName} has left the game.";
            viewModel.Winner = viewModel.Player;
        });

        return hub;
    }
}
