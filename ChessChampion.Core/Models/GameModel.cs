using ChessChampion.Shared.Models;
using ChessChampion.Shared.Models.Pieces;
using static ChessChampion.Shared.Services.RulesService;

namespace ChessChampion.Core.Models;

public class GameModel
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Code { get; set; }
    public GameStateModel GameState { get; set; }
    public PlayerModel? WhitePlayer { get; set; }
    public PlayerModel? BlackPlayer { get; set; }

    public GameModel()
    {
        GameSquare[][] initialState = new GameSquare[8][];
        initialState[0] =
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
        initialState[1] = new GameSquare[8];
        for (int i = 0; i < initialState[1].Length; i++)
        {
            initialState[1][i] = new GameSquare() { Y = 1, X = i, Piece = new WhitePawn() };
        }
        for (int j = 2; j < 6; j++)
        {
            initialState[j] = new GameSquare[8];
            for (int i = 0; i < initialState[j].Length; i++)
            {
                initialState[j][i] = new GameSquare() { Y = j, X = i };
            }
        }
        initialState[6] = new GameSquare[8];
        for (int i = 0; i < initialState[6].Length; i++)
        {
            initialState[6][i] = new GameSquare() { Y = 6, X = i, Piece = new BlackPawn() };
        }
        initialState[7] =
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
        GameState = new(initialState);
    }

    public BaseError? TryMakeMove(string move)
    {
        GameSquare startSquare = GameState[move[..2]];
        GameSquare endSquare = GameState[move[2..4]];
        if (startSquare.Piece is null)
        {
            return new BaseError($"Cannot move empty square {startSquare.ChessCoordinate}.");
        }
        if (!IsPlayerPiece(startSquare.Piece, GameState.IsWhitePlayerTurn))
        {
            return new BaseError($"Cannot move opponent piece {startSquare.ChessCoordinate}.");
        }
        if (endSquare.Piece is not null && IsPlayerPiece(endSquare.Piece, GameState.IsWhitePlayerTurn))
        {
            return new BaseError($"Cannot eat own piece {endSquare.ChessCoordinate}.");
        }
        if (startSquare.Piece.GetMovableSquares(GameState, startSquare.X, startSquare.Y).Contains(endSquare) == false)
        {
            return new BaseError($"Cannot move {startSquare.ChessCoordinate} to {endSquare.ChessCoordinate}.");
        }
        startSquare.Piece.HandleMove(GameState, startSquare, endSquare);
        GameState.Moves += $" {move}";
        GameState.IsWhitePlayerTurn = !GameState.IsWhitePlayerTurn;
        return null;
    }

    public PlayerModel? CheckForWinner(bool whiteMoved)
    {
        if (whiteMoved)
        {
            GameSquare blackKingSquare = GameState.GetPieceSquare<BlackKing>();
            if (IsInOpponentThreatSquare(GameState, blackKingSquare.X, blackKingSquare.Y, false))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, whiteMoved))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return null;
                    }
                }
                return WhitePlayer;
            }
        }
        else
        {
            GameSquare whiteKingSquare = GameState.GetPieceSquare<WhiteKing>();
            if (IsInOpponentThreatSquare(GameState, whiteKingSquare.X, whiteKingSquare.Y, true))
            {
                foreach (GameSquare opponentSquare in GetAllOpponentPieces(GameState, whiteMoved))
                {
                    if ((opponentSquare.Piece?.GetMovableSquares(GameState, opponentSquare.X, opponentSquare.Y) ?? []).Any())
                    {
                        return null;
                    }
                }
                return BlackPlayer;
            }
        }
        return null;
    }
}
