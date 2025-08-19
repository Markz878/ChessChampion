using ChessChampion.Client.Models;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace ChessChampion.Client.Components;

public partial class ChessBoardComponent
{
    [Inject] public required IChessService ChessService { get; set; }
    [Inject] public required MainViewModel ViewModel { get; init; }
    public GameStateModel GameState => ViewModel.GameState ?? throw new ArgumentNullException(nameof(GameState));

    private GameSquare? selectedSquare;
    private List<GameSquare> movableSquares = [];

    protected override void OnInitialized()
    {
        if (RendererInfo.IsInteractive)
        {
            ViewModel.StateHasChanged += StateHasChanged;
        }
    }

    public async Task HandleClick(GameSquare square)
    {
        try
        {
            await HandleSquareSelect(square, ViewModel.Player);
        }
        catch (Exception ex)
        {
            ViewModel.StatusMessage = ex.Message;
        }
    }

    public async Task HandleSquareSelect(GameSquare square, PlayerModel player)
    {
        if (ViewModel.Winner is not null)
        {
            return;
        }

        if (player.IsWhite != GameState.IsWhitePlayerTurn)
        {
            return;
        }

        if (square == selectedSquare)
        {
            selectedSquare = null;
            movableSquares = [];
        }
        else if (square.Piece is not null && RulesService.IsPlayerPiece(square.Piece, player.IsWhite))
        {
            selectedSquare = square;
            movableSquares = square.Piece.GetMovableSquares(GameState, square.X, square.Y).ToList();
        }
        else if (selectedSquare is not null && movableSquares.Contains(square))
        {
            BaseError? error = await HandleMove(selectedSquare, square, player);
            if (error.HasValue)
            {
                ViewModel.StatusMessage = error.Value.Error;
            }
        }
    }

    private async Task<BaseError?> HandleMove(GameSquare startSquare, GameSquare endSquare, PlayerModel player)
    {
        if (startSquare.Piece is null)
        {
            return new BaseError("No piece at start square");
        }
        string move = startSquare.Piece.HandleMove(GameState, startSquare, endSquare);
        selectedSquare = null;
        movableSquares = [];
        GameState.Moves += $" {move}";
        ViewModel.StatusMessage = MainViewModel.OtherPlayerTurnText;
        GameState.IsWhitePlayerTurn = !GameState.IsWhitePlayerTurn;
        BaseError? error = await ChessService.SubmitMove(new SubmitMoveRequest(ViewModel.GameId!.Value, move, player.Name));
        return error;
    }
}
