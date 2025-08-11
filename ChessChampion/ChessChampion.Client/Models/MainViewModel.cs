using ChessChampion.Shared.Models;

namespace ChessChampion.Client.Models;

public sealed class MainViewModel : BaseViewModel
{
    public Guid? GameId { get => gameId; set => SetProperty(ref gameId, value); }
    private Guid? gameId;
    public GameStateModel? GameState { get => gameState; set => SetProperty(ref gameState, value); }
    private GameStateModel? gameState;
    public PlayerModel Player { get => player; set => SetProperty(ref player, value); }
    private PlayerModel player = new("", true);
    public PlayerModel? OtherPlayer { get => otherPlayer; set => SetProperty(ref otherPlayer, value); }
    private PlayerModel? otherPlayer;
    public PlayerModel? WhitePlayer => Player is null ? null : Player.IsWhite ? Player : OtherPlayer;
    public PlayerModel? BlackPlayer => Player is null ? null : Player.IsWhite ? OtherPlayer : Player;
    public PlayVsMode PlayVsMode { get => playVsMode; set => SetProperty(ref playVsMode, value); }
    private PlayVsMode playVsMode = PlayVsMode.PlayVsHuman;
    public JoinGameMode JoinGameMode { get => joinGameMode; set => SetProperty(ref joinGameMode, value); }
    private JoinGameMode joinGameMode;
    public int SkillLevel { get => skillLevel; set => SetProperty(ref skillLevel, value); }
    private int skillLevel = 10;
    public string StatusMessage { get => statusMessage; set => SetProperty(ref statusMessage, value); }
    private string statusMessage = "";
    public string GameCode { get => gameCode; set => SetProperty(ref gameCode, value); }
    private string gameCode = "";
}
