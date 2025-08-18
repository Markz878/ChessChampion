using ChessChampion.Shared.Services;

namespace ChessChampion.Services;

public sealed class MockHubConnectionService : IHubConnectionService
{
    public string? ConnectionId()
    {
        throw new NotImplementedException();
    }

    public Task JoinGame(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task LeaveGame(Guid gameId)
    {
        throw new NotImplementedException();
    }
}
