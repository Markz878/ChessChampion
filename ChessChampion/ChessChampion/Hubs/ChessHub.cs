using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChessChampion.Hubs;

public class ChessHub : Hub<IChessHubNotifications>, IChessHubClientActions
{
    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
    }

    public async Task LeaveGame(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
    }
}
