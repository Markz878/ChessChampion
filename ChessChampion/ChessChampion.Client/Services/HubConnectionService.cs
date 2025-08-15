using ChessChampion.Client.Models;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace ChessChampion.Client.Services;

public sealed class HubConnectionService(MainViewModel viewModel)
{
    public HubConnection BuildHubConnection(Uri hubUri, Action<HttpConnectionOptions>? configureHttpConnection = null)
    {
        HubConnection hub = new HubConnectionBuilder()
            .WithUrl(hubUri, configureHttpConnection ?? (_ => { }))
            .WithAutomaticReconnect()
            .Build();

        hub.On<string>(nameof(IChessHubNotifications.PlayerJoined), (playerName) =>
        {
            bool isWhite = !viewModel.Player.IsWhite;
            viewModel.StatusMessage = $"{playerName} has joined the game as {(isWhite ? "White" : "Black")}.";
            viewModel.OtherPlayer = new PlayerModel(playerName, isWhite);
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
            viewModel.StateHasChanged?.Invoke();
        });

        hub.On(nameof(IChessHubNotifications.GameOver), (string winner) =>
        {
            viewModel.StatusMessage = $"Game over! {winner} wins!";
        });

        hub.On(nameof(IChessHubNotifications.PlayerLeft), (string leaverName) =>
        {
            viewModel.StatusMessage = $"{leaverName} has left the game.";
            viewModel.Winner = viewModel.Player;
        });

        return hub;
    }
}
