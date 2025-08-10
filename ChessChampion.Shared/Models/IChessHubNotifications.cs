namespace ChessChampion.Shared.Models;

public interface IChessHubNotifications
{
    Task MoveReceived(string move);
    Task PlayerLeft(string leaverName);
    Task GameOver(string winner);
}
