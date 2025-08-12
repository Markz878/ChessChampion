using ChessChampion.Shared.Models;
using ChessChampion.Shared.Models.Pieces;

namespace ChessChampion.Shared;

public static class RulesService
{
    private static readonly string[] BlackPieces = ["♜", "♞", "♝", "♛", "♚", "♟︎"];
    private static readonly string[] WhitePieces = ["♖", "♘", "♗", "♕", "♔", "♙"];

    public static bool IsWhitePiece(string piece)
    {
        return WhitePieces.Contains(piece);
    }

    public static bool IsBlackPiece(string piece)
    {
        return BlackPieces.Contains(piece);
    }

    public static bool IsPlayerPiece(ChessPiece chessPiece, bool isPlayerWhite)
    {
        return chessPiece is not null && (IsWhitePiece(chessPiece.Marker) && isPlayerWhite || IsBlackPiece(chessPiece.Marker) && !isPlayerWhite);
    }

    public static IEnumerable<GameSquare> GetKingThreatSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (!(i == 0 && j == 0) && GetSquareBlockState(gameState, x + i, y + j, piece.IsWhite).CanThreaten())
                {
                    yield return gameState[y + j][x + i];
                }
            }
        }
    }

    public static IEnumerable<GameSquare> GetKingMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (!(i == 0 && j == 0) && GetSquareBlockState(gameState, x + i, y + j, piece.IsWhite).CanMoveTo() && !IsKingMovingToOpponentThreatSquare(gameState, gameState[y][x], gameState[y + j][x + i]))
                {
                    yield return gameState[y + j][x + i];
                }
            }
        }
        if (piece.IsWhite && !IsInOpponentThreatSquare(gameState, x, y, piece.IsWhite))
        {
            if (gameState.CanWhiteKingCastleLeft && gameState["d1"].IsEmpty && gameState["c1"].IsEmpty && gameState["b1"].IsEmpty && !IsInOpponentThreatSquare(gameState, 3, 0, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 2, 0, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 1, 0, piece.IsWhite))
            {
                yield return gameState[y][x - 2];
            }
            if (gameState.CanWhiteKingCastleRight && gameState["f1"].IsEmpty && gameState["g1"].IsEmpty && !IsInOpponentThreatSquare(gameState, 5, 0, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 6, 0, piece.IsWhite))
            {
                yield return gameState[y][x + 2];
            }
        }
        if (!piece.IsWhite && !IsInOpponentThreatSquare(gameState, x, y, piece.IsWhite))
        {
            if (gameState.CanBlackKingCastleLeft && gameState["d8"].IsEmpty && gameState["c8"].IsEmpty && gameState["b8"].IsEmpty && !IsInOpponentThreatSquare(gameState, 3, 7, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 2, 7, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 1, 7, piece.IsWhite))
            {
                yield return gameState[y][x - 2];
            }
            if (gameState.CanBlackKingCastleRight && gameState["f8"].IsEmpty && gameState["g8"].IsEmpty && !IsInOpponentThreatSquare(gameState, 5, 7, piece.IsWhite) && !IsInOpponentThreatSquare(gameState, 6, 7, piece.IsWhite))
            {
                yield return gameState[y][x + 2];
            }
        }
    }

    public static bool IsKingMovingToOpponentThreatSquare(GameStateModel gameState, GameSquare startSquare, GameSquare targetSquare)
    {
        if (startSquare.Piece is null)
        {
            return false;
        }
        ChessPiece kingPiece = startSquare.Piece;
        startSquare.Piece = null;
        foreach (GameSquare opponentSquare in GetAllOpponentPieces(gameState, kingPiece.IsWhite))
        {
            if (opponentSquare.Piece is not null && opponentSquare.Piece.GetThreatSquares(gameState, opponentSquare.X, opponentSquare.Y).Contains(targetSquare))
            {
                startSquare.Piece = kingPiece;
                return true;
            }
        }
        startSquare.Piece = kingPiece;
        return false;
    }

    public static bool IsInOpponentThreatSquare(GameStateModel gameState, int x, int y, bool isWhite)
    {
        foreach (GameSquare opponentSquare in GetAllOpponentPieces(gameState, isWhite))
        {
            if (opponentSquare.Piece is not null && opponentSquare.Piece.GetThreatSquares(gameState, opponentSquare.X, opponentSquare.Y).Contains(gameState[y][x]))
            {
                return true;
            }
        }
        return false;
    }

    public static IEnumerable<GameSquare> GetAllOpponentPieces(GameStateModel gameState, bool isWhite)
    {
        foreach (GameSquare square in gameState.GetSquares())
        {
            if (square.Piece != null && square.Piece.IsWhite != isWhite)
            {
                yield return square;
            }
        }
    }

    public static IEnumerable<GameSquare> GetBlackPawnThreatSquares(GameStateModel gameState, int x, int y)
    {
        if (GetSquareBlockState(gameState, x - 1, y - 1, false).CanThreaten())
        {
            yield return gameState[y - 1][x - 1];

        }
        if (GetSquareBlockState(gameState, x + 1, y - 1, false).CanThreaten())
        {
            yield return gameState[y - 1][x + 1];
        }
    }

    public static IEnumerable<GameSquare> GetBlackPawnMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        if (GetSquareBlockState(gameState, x, y - 1, false) == SquareBlockState.Available && !IsKingThreatened(gameState, x, y, piece, x, y - 1))
        {
            yield return gameState[y - 1][x];
        }
        if (GetSquareBlockState(gameState, x - 1, y - 1, false) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, x, y, piece, x - 1, y - 1))
        {
            yield return gameState[y - 1][x - 1];
        }
        if (GetSquareBlockState(gameState, x + 1, y - 1, false) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, x, y, piece, x + 1, y - 1))
        {
            yield return gameState[y - 1][x + 1];
        }
        if (y == 6 && GetSquareBlockState(gameState, x, y - 1, false) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y - 2, false) == SquareBlockState.Available && !IsKingThreatened(gameState, x, y, piece, x, y - 2))
        {
            yield return gameState[y - 2][x];
        }
    }

    public static IEnumerable<GameSquare> GetWhitePawnThreatSquares(GameStateModel gameState, int x, int y)
    {
        if (GetSquareBlockState(gameState, x - 1, y + 1, true).CanThreaten())
        {
            yield return gameState[y + 1][x - 1];

        }
        if (GetSquareBlockState(gameState, x + 1, y + 1, true).CanThreaten())
        {
            yield return gameState[y + 1][x + 1];
        }
    }

    public static IEnumerable<GameSquare> GetWhitePawnMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        if (GetSquareBlockState(gameState, x, y + 1, true) == SquareBlockState.Available && !IsKingThreatened(gameState, x, y, piece, x, y + 1))
        {
            yield return gameState[y + 1][x];
        }
        if (GetSquareBlockState(gameState, x - 1, y + 1, true) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, x, y, piece, x - 1, y + 1))
        {
            yield return gameState[y + 1][x - 1];
        }
        if (GetSquareBlockState(gameState, x + 1, y + 1, true) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, x, y, piece, x + 1, y + 1))
        {
            yield return gameState[y + 1][x + 1];
        }
        if (y == 1 && GetSquareBlockState(gameState, x, y + 1, true) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y + 2, true) == SquareBlockState.Available && !IsKingThreatened(gameState, x, y, piece, x, y + 2))
        {
            yield return gameState[y + 2][x];
        }
    }

    public static IEnumerable<GameSquare> GetQueenThreatSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        return GetTowerThreatSquares(gameState, x, y, piece).Concat(GetBishopThreatSquares(gameState, x, y, piece));
    }

    public static IEnumerable<GameSquare> GetQueenMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        return GetTowerMovableSquares(gameState, x, y, piece).Concat(GetBishopMovableSquares(gameState, x, y, piece));
    }

    public static IEnumerable<GameSquare> GetTowerMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        // right
        for (int i = x + 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, i, y, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, i, y))
            {
                yield return gameState[y][i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // left
        for (int i = x - 1; i >= 0; i--)
        {
            SquareBlockState state = GetSquareBlockState(gameState, i, y, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, i, y))
            {
                yield return gameState[y][i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // up
        for (int j = y - 1; j >= 0; j--)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x, j, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x, j))
            {
                yield return gameState[j][x];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // down
        for (int j = y + 1; j < 8; j++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x, j, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x, j))
            {
                yield return gameState[j][x];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
    }

    public static IEnumerable<GameSquare> GetTowerThreatSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        // right
        for (int i = x + 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, i, y, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y][i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // left
        for (int i = x - 1; i >= 0; i--)
        {
            SquareBlockState state = GetSquareBlockState(gameState, i, y, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y][i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // up
        for (int j = y - 1; j >= 0; j--)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x, j, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[j][x];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        // down
        for (int j = y + 1; j < 8; j++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x, j, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[j][x];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
    }

    private static bool IsKingThreatened(GameStateModel gameState, int xStart, int yStart, ChessPiece startChessPiece, int xEnd, int yEnd)
    {
        ChessPiece? endChessPiece = gameState[yEnd][xEnd].Piece;
        gameState[yEnd][xEnd].Piece = startChessPiece;
        gameState[yStart][xStart].Piece = null;
        GameSquare kingSquare = startChessPiece.IsWhite ? gameState.GetPieceSquare<WhiteKing>() : gameState.GetPieceSquare<BlackKing>();
        bool isInThreat = IsInOpponentThreatSquare(gameState, kingSquare.X, kingSquare.Y, startChessPiece.IsWhite);
        gameState[yStart][xStart].Piece = startChessPiece;
        gameState[yEnd][xEnd].Piece = endChessPiece;
        return isInThreat;
    }

    public static IEnumerable<GameSquare> GetBishopMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x + i, y + i, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + i, y + i))
            {
                yield return gameState[y + i][x + i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + i, y - i))
            {
                yield return gameState[y - i][x + i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x - i, y + i, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - i, y + i))
            {
                yield return gameState[y + i][x - i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x - i, y - i, piece.IsWhite);
            if (state.CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - i, y - i))
            {
                yield return gameState[y - i][x - i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
    }

    public static IEnumerable<GameSquare> GetBishopThreatSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y - i][x + i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x + i, y + i, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y + i][x + i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x - i, y + i, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y + i][x - i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
        for (int i = 1; i < 8; i++)
        {
            SquareBlockState state = GetSquareBlockState(gameState, x - i, y - i, piece.IsWhite);
            if (state.CanThreaten())
            {
                yield return gameState[y - i][x - i];
            }
            if (state != SquareBlockState.Available)
            {
                break;
            }
        }
    }

    public static IEnumerable<GameSquare> GetKnightThreatSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        if (GetSquareBlockState(gameState, x + 1, y - 2, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y - 2][x + 1];
        }
        if (GetSquareBlockState(gameState, x + 2, y - 1, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y - 1][x + 2];
        }
        if (GetSquareBlockState(gameState, x + 2, y + 1, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y + 1][x + 2];
        }
        if (GetSquareBlockState(gameState, x + 1, y + 2, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y + 2][x + 1];
        }
        if (GetSquareBlockState(gameState, x - 1, y + 2, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y + 2][x - 1];
        }
        if (GetSquareBlockState(gameState, x - 2, y + 1, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y + 1][x - 2];
        }
        if (GetSquareBlockState(gameState, x - 2, y - 1, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y - 1][x - 2];
        }
        if (GetSquareBlockState(gameState, x - 1, y - 2, piece.IsWhite).CanThreaten())
        {
            yield return gameState[y - 2][x - 1];
        }
    }

    public static IEnumerable<GameSquare> GetKnightMovableSquares(GameStateModel gameState, int x, int y, ChessPiece piece)
    {
        if (GetSquareBlockState(gameState, x + 1, y - 2, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + 1, y - 2))
        {
            yield return gameState[y - 2][x + 1];
        }
        if (GetSquareBlockState(gameState, x + 2, y - 1, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + 2, y - 1))
        {
            yield return gameState[y - 1][x + 2];
        }
        if (GetSquareBlockState(gameState, x + 2, y + 1, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + 2, y + 1))
        {
            yield return gameState[y + 1][x + 2];
        }
        if (GetSquareBlockState(gameState, x + 1, y + 2, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x + 1, y + 2))
        {
            yield return gameState[y + 2][x + 1];
        }
        if (GetSquareBlockState(gameState, x - 1, y + 2, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - 1, y + 2))
        {
            yield return gameState[y + 2][x - 1];
        }
        if (GetSquareBlockState(gameState, x - 2, y + 1, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - 2, y + 1))
        {
            yield return gameState[y + 1][x - 2];
        }
        if (GetSquareBlockState(gameState, x - 2, y - 1, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - 2, y - 1))
        {
            yield return gameState[y - 1][x - 2];
        }
        if (GetSquareBlockState(gameState, x - 1, y - 2, piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, x, y, piece, x - 1, y - 2))
        {
            yield return gameState[y - 2][x - 1];
        }
    }

    public static SquareBlockState GetSquareBlockState(GameStateModel gameState, int x, int y, bool isWhite)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            return SquareBlockState.OutOfBounds;
        }
        if (gameState[y][x].IsEmpty)
        {
            return SquareBlockState.Available;
        }
        else if (gameState[y][x].Piece?.IsWhite != isWhite)
        {
            return SquareBlockState.OpponentPiece;
        }
        else
        {
            return SquareBlockState.OwnPiece;
        }
    }

    public static bool CanMoveTo(this SquareBlockState blockState)
    {
        return blockState is SquareBlockState.Available or SquareBlockState.OpponentPiece;
    }

    public static bool CanThreaten(this SquareBlockState blockState)
    {
        return blockState is SquareBlockState.Available or SquareBlockState.OpponentPiece or SquareBlockState.OwnPiece;
    }
}
