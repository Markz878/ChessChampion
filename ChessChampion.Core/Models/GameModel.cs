using ChessChampion.Shared.Models;
using ChessChampion.Shared.Models.Pieces;
using static ChessChampion.Shared.RulesService;

namespace ChessChampion.Core.Models;

public class GameModel
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Code { get; set; }
    public GameStateModel GameState { get; set; }
    public PlayerModel? WhitePlayer { get; set; }
    public PlayerModel? BlackPlayer { get; set; }
    public bool IsWhitePlayerTurn { get; set; } = true;

    public GameModel()
    {
        GameStateModel gameState = new();
        gameState.State[0] =
        [
            new GameSquare(){ Y=0, X=0, Piece = new WhiteTower() },
            new GameSquare(){ Y=0, X=1, Piece = new WhiteKnight() },
            new GameSquare(){ Y=0, X=2, Piece = new WhiteBishop() },
            new GameSquare(){ Y=0, X=3, Piece = new WhiteQueen() },
            new GameSquare(){ Y=0, X=4, Piece = new WhiteKing() },
            new GameSquare(){ Y=0, X=5, Piece = new WhiteBishop() },
            new GameSquare(){ Y=0, X=6, Piece = new WhiteKnight() },
            new GameSquare(){ Y=0, X=7, Piece = new WhiteTower() },
        ];
        gameState.State[1] = new GameSquare[8];
        for (int i = 0; i < gameState.State[1].Length; i++)
        {
            gameState.State[1][i] = new GameSquare() { Y = 1, X = i, Piece = new WhitePawn() };
        }
        for (int j = 2; j < 6; j++)
        {
            gameState.State[j] = new GameSquare[8];
            for (int i = 0; i < gameState.State[j].Length; i++)
            {
                gameState.State[j][i] = new GameSquare() { Y = j, X = i };
            }
        }
        gameState.State[6] = new GameSquare[8];
        for (int i = 0; i < gameState.State[6].Length; i++)
        {
            gameState.State[6][i] = new GameSquare() { Y = 6, X = i, Piece = new BlackPawn() };
        }
        gameState.State[7] =
        [
            new GameSquare(){ Y=7, X=0, Piece = new BlackTower() },
            new GameSquare(){ Y=7, X=1, Piece = new BlackKnight() },
            new GameSquare(){ Y=7, X=2, Piece = new BlackBishop() },
            new GameSquare(){ Y=7, X=3, Piece = new BlackQueen() },
            new GameSquare(){ Y=7, X=4, Piece = new BlackKing() },
            new GameSquare(){ Y=7, X=5, Piece = new BlackBishop() },
            new GameSquare(){ Y=7, X=6, Piece = new BlackKnight() },
            new GameSquare(){ Y=7, X=7, Piece = new BlackTower() },
        ];
        GameState = gameState;
    }

    public bool TryMakeMove(string move, out PlayerModel? winner)
    {
        throw new NotImplementedException();
    }

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

    public async Task HandleSquareSelect(GameSquare square, PlayerModel player)
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
            if (selectedSquare is null && square.Piece is not null && IsPlayerPiece(square.Piece, player.IsWhite))
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
                bool winnerFound = await HandleMove(square, selectedSquare, player);
                if (winnerFound)
                {
                    //GameEnded?.Invoke();
                }
                OnStateChanged();
            }
        }
        catch
        {
            //GameEnded?.Invoke();
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

    private async Task<bool> HandleMove(GameSquare endSquare, GameSquare startSquare, PlayerModel player)
    {
        string? move = startSquare.Piece?.HandleMove(GameState, startSquare, endSquare);
        GameState.Moves += $" {move}";
        ResetBoardStates();
        CheckForWin(player.IsWhite);
        //if (Winner != null)
        //{
        //    return true;
        //}
        IsWhitePlayerTurn = !IsWhitePlayerTurn;
        OnStateChanged();
        PlayerModel? opponent = player.IsWhite ? BlackPlayer : WhitePlayer;
        if (opponent is AIPlayerModel ai)
        {

            Result<string, AIMoveError> aimove = await ai.Move(GameState);
            GameState.Moves += $" {aimove}";
            CheckForWin(!player.IsWhite);
            //if (Winner != null)
            //{
            //    return true;
            //}
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
            if (IsInOpponentThreatSquare(GameState, blackKingSquare.X, blackKingSquare.Y, isWhite))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return;
                    }
                }
                //Winner = WhitePlayer;
            }
        }
        else
        {
            GameSquare whiteKingSquare = GameState.GetPieceSquare<WhiteKing>();
            if (IsInOpponentThreatSquare(GameState, whiteKingSquare.X, whiteKingSquare.Y, isWhite))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return;
                    }
                }
                //Winner = BlackPlayer;
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
        //GameEnded?.Invoke();
    }

    public void OnStateChanged()
    {
        //StateChanged?.Invoke();
    }


}
