using ChessChampion.Client.Models;
using ChessChampion.Client.Services;
using ChessChampion.Shared;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace ChessChampion.Client.Components;

public partial class ChessBoardComponent
{
    [Inject] public required APIService API { get; init; }
    [Inject] public required MainViewModel ViewModel { get; init; }
    public GameStateModel GameState => ViewModel.GameState ?? throw new ArgumentNullException(nameof(GameState));

    public async Task HandleClick(GameSquare square)
    {
        if (ViewModel.Winner is not null) return;
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
        if (player.IsWhite != GameState.IsWhitePlayerTurn)
        {
            return;
        }
        try
        {
            GameSquare? selectedSquare = GetSelectedSquare();
            /// Different paths:
            /// 1) Nothing is selected, and user selects own piece
            /// 2) Own piece is selected, and user selects the same piece
            /// 3) Own piece is selected, and user selects another own piece
            /// 4) Own piece is selected, and user selects a movable square
            if (selectedSquare is null && square.Piece is not null && RulesService.IsPlayerPiece(square.Piece, player.IsWhite))
            {
                HandlePieceSelect(square);
            }
            else if (selectedSquare == square)
            {
                HandleSameSquareSelect(selectedSquare);
            }
            else if (selectedSquare is not null && !square.IsEmpty && RulesService.IsPlayerPiece(square.Piece, player.IsWhite))
            {
                HandleOtherPieceSelect(square, selectedSquare);
            }
            else if (selectedSquare is not null && square.State == SquareState.Movable)
            {
                bool winnerFound = await HandleMove(square, selectedSquare, player);
                if (winnerFound)
                {
                    //GameEnded?.Invoke();
                }
            }
        }
        catch
        {
            //GameEnded?.Invoke();
            throw;
        }
    }

    private GameSquare? GetSelectedSquare()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (GameState.State[i][j].State == SquareState.Selected)
                {
                    return GameState.State[i][j];
                }
            }
        }
        return null;
    }

    private void HandlePieceSelect(GameSquare square)
    {
        square.State = SquareState.Selected;
        foreach (GameSquare availableSquare in square.Piece?.GetMovableSquares(GameState, square.X, square.Y) ?? [])
        {
            availableSquare.State = SquareState.Movable;
        }
    }

    private void HandleSameSquareSelect(GameSquare selectedSquare)
    {
        selectedSquare.State = SquareState.Normal;
        foreach (GameSquare availableSquare in selectedSquare.Piece?.GetMovableSquares(GameState, selectedSquare.X, selectedSquare.Y) ?? [])
        {
            availableSquare.State = SquareState.Normal;
        }
    }

    private void HandleOtherPieceSelect(GameSquare square, GameSquare selectedSquare)
    {
        selectedSquare.State = SquareState.Normal;
        foreach (GameSquare availableSquare in selectedSquare.Piece?.GetMovableSquares(GameState, selectedSquare.X, selectedSquare.Y) ?? [])
        {
            availableSquare.State = SquareState.Normal;
        }
        square.State = SquareState.Selected;
        foreach (GameSquare availableSquare in square.Piece?.GetMovableSquares(GameState, square.X, square.Y) ?? [])
        {
            availableSquare.State = SquareState.Movable;
        }
    }

    private async Task<bool> HandleMove(GameSquare endSquare, GameSquare startSquare, PlayerModel player)
    {
        if(startSquare.Piece is null)
        {
            return false;
        }
        string? move = startSquare.Piece.HandleMove(GameState, startSquare, endSquare);
        GameState.Moves += $" {move}";
        ResetBoardStates();
        //CheckForWin(player.IsWhite);
        ////if (Winner != null)
        ////{
        ////    return true;
        ////}
        GameState.IsWhitePlayerTurn = !GameState.IsWhitePlayerTurn;
        await API.SubmitMove(new SubmitMoveRequest(ViewModel.GameId!.Value, move, player.Name));
        //PlayerModel? opponent = player.IsWhite ? BlackPlayer : WhitePlayer;
        //if (opponent is AIPlayerModel ai)
        //{

        //    Result<string, AIMoveError> aimove = await ai.Move(GameState);
        //    GameState.Moves += $" {aimove}";
        //    CheckForWin(!player.IsWhite);
        //    //if (Winner != null)
        //    //{
        //    //    return true;
        //    //}
        //    IsWhitePlayerTurn = !IsWhitePlayerTurn;
        //    OnStateChanged();
        //}
        return false;
    }

    public void ResetBoardStates()
    {
        foreach (GameSquare square in GameState.GetSquares())
        {
            square.State = SquareState.Normal;
        }
    }
}
