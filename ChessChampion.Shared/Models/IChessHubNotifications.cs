namespace ChessChampion.Shared.Models;

public interface IChessHubNotifications
{
    Task PlayerJoined(string playerName);
    Task MoveReceived(string move);
    Task PlayerLeft(string leaverName);
    Task GameOver(string winner);
}
