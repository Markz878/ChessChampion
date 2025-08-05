using ChessChampionWebUI.Components;
using ChessChampionWebUI.Models.Pieces;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models;

public class GameModel
{
    public PlayerModel? WhitePlayer { get; set; }
    public PlayerModel? BlackPlayer { get; set; }
    public GameStateModel GameState { get; set; } = new GameStateModel();
    public bool IsWhitePlayerTurn { get; set; } = true;
    public PlayerModel? Winner { get; set; }

    public Action? StateChanged;

    public Action? GameEnded;

    public GameSquare? GetSelectedSquare()
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

    public async Task HandleSquareSelect(GameSquare square, PlayerModel player, ILogger<ChessBoardComponent> logger)
    {
        if (player.IsWhite != IsWhitePlayerTurn)
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
            if (selectedSquare is null && square.Piece != null && IsPlayerPiece(square.Piece, player.IsWhite))
            {
                HandlePieceSelect(square);
            }
            else if (selectedSquare == square)
            {
                HandleSameSquareSelect(selectedSquare);
            }
            else if (selectedSquare is not null && !square.IsEmpty && IsPlayerPiece(square.Piece, player.IsWhite))
            {
                HandleOtherPieceSelect(square, selectedSquare);
            }
            else if (selectedSquare is not null && square.State == SquareState.Movable)
            {
                bool winnerFound = await HandleMove(square, selectedSquare, player, logger);
                if (winnerFound)
                {
                    GameEnded?.Invoke();
                }
                OnStateChanged();
            }
        }
        catch
        {
            GameEnded?.Invoke();
            throw;
        }
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

    private async Task<bool> HandleMove(GameSquare endSquare, GameSquare startSquare, PlayerModel player, ILogger logger)
    {
        string? move = startSquare.Piece?.HandleMove(GameState, startSquare, endSquare);
        GameState.Moves += $" {move}";
        ResetBoardStates();
        CheckForWin(player.IsWhite);
        if (Winner != null)
        {
            return true;
        }
        IsWhitePlayerTurn = !IsWhitePlayerTurn;
        OnStateChanged();
        PlayerModel? opponent = player.IsWhite ? BlackPlayer : WhitePlayer;
        if (opponent is AIPlayerModel ai)
        {
            string? aimove = await ai.Move(GameState, logger);
            GameState.Moves += $" {aimove}";
            CheckForWin(!player.IsWhite);
            if (Winner != null)
            {
                return true;
            }
            IsWhitePlayerTurn = !IsWhitePlayerTurn;
            OnStateChanged();
        }
        return false;
    }

    private void CheckForWin(bool isWhite)
    {
        if (isWhite)
        {
            GameSquare blackKingSquare = GameState.GetPieceSquare<BlackKing>();
            if (IsInOpponentThreatSquare(GameState, blackKingSquare.X, blackKingSquare.Y, false))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return;
                    }
                }
                Winner = WhitePlayer;
            }
        }
        else
        {
            GameSquare whiteKingSquare = GameState.GetPieceSquare<WhiteKing>();
            if (IsInOpponentThreatSquare(GameState, whiteKingSquare.X, whiteKingSquare.Y, false))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return;
                    }
                }
                Winner = BlackPlayer;
            }
        }
    }

    public void ResetBoardStates()
    {
        foreach (GameSquare square in GameState.GetSquares())
        {
            square.State = SquareState.Normal;
        }
    }

    public void OnGameEnded()
    {
        GameEnded?.Invoke();
    }

    public void OnStateChanged()
    {
        StateChanged?.Invoke();
    }
}
