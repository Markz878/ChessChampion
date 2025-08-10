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

        hub.On<string>(nameof(IChessHubNotifications.MoveReceived), (move) =>
        {
            if (viewModel.GameState is null)
            {
                return;
            }
            viewModel.GameState.Moves += " " + move;
        });

        hub.On(nameof(IChessHubNotifications.GameOver), (string winner) =>
        {
            viewModel.StatusMessage = $"Game over! {winner} wins!";
        });

        hub.On(nameof(IChessHubNotifications.PlayerLeft), (string leaverName) =>
        {
            if (viewModel.Player?.Name == leaverName)
            {
                viewModel.StatusMessage = "You have left the game.";
                viewModel.GameState = null;
                viewModel.Player = null;
                viewModel.OtherPlayer = null;
            }
            else
            {
                viewModel.StatusMessage = $"{leaverName} has left the game.";
            }
        });

        return hub;
    }
}
