using ChessChampionWebUI.Models;
using ChessChampionWebUI.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessChampionWebUI.Data
{
    public static class RulesService
    {
        private static readonly string[] BlackPieces = ["♜", "♞", "♝", "♛", "♚", "♟︎"];
        private static readonly string[] WhitePieces = ["♖", "♘", "♗", "♕", "♔", "♙"];

        public static bool IsWhitePiece(string piece) => WhitePieces.Contains(piece);
        public static bool IsBlackPiece(string piece) => BlackPieces.Contains(piece);

        public static bool IsPlayerPiece(string piece, bool isPlayerWhite)
        {
            return (IsWhitePiece(piece) && isPlayerWhite) || (IsBlackPiece(piece) && !isPlayerWhite);
        }

        public static IEnumerable<GameSquare> GetKingThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(i == 0 && j == 0) && GetSquareBlockState(gameState, x + i, y + j, square.Piece.IsWhite).CanThreaten())
                    {
                        yield return gameState[y + j][x + i];
                    }
                }
            }
        }

        public static IEnumerable<GameSquare> GetKingMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!(i == 0 && j == 0) && GetSquareBlockState(gameState, x + i, y + j, square.Piece.IsWhite).CanMoveTo() && !IsKingMovingToOpponentThreatSquare(gameState, gameState[y][x], gameState[y + j][x + i]))
                    {
                        yield return gameState[y + j][x + i];
                    }
                }
            }
            if (square.Piece.IsWhite && !IsInOpponentThreatSquare(gameState, square, square.Piece.IsWhite))
            {
                if (gameState.CanWhiteKingCastleLeft && gameState["d1"].IsEmpty && gameState["c1"].IsEmpty && gameState["b1"].IsEmpty && !IsInOpponentThreatSquare(gameState, gameState["d1"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["c1"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["b1"], square.Piece.IsWhite))
                {
                    yield return gameState[y][x - 2];
                }
                if (gameState.CanWhiteKingCastleRight && gameState["f1"].IsEmpty && gameState["g1"].IsEmpty && !IsInOpponentThreatSquare(gameState, gameState["f1"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["g1"], square.Piece.IsWhite))
                {
                    yield return gameState[y][x + 2];
                }
            }
            if (!square.Piece.IsWhite && !IsInOpponentThreatSquare(gameState, square, square.Piece.IsWhite))
            {
                if (gameState.CanBlackKingCastleLeft && gameState["d8"].IsEmpty && gameState["c8"].IsEmpty && gameState["b8"].IsEmpty && !IsInOpponentThreatSquare(gameState, gameState["d8"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["c8"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["b8"], square.Piece.IsWhite))
                {
                    yield return gameState[y][x - 2];
                }
                if (gameState.CanBlackKingCastleRight && gameState["f8"].IsEmpty && gameState["g8"].IsEmpty && !IsInOpponentThreatSquare(gameState, gameState["f8"], square.Piece.IsWhite) && !IsInOpponentThreatSquare(gameState, gameState["g8"], square.Piece.IsWhite))
                {
                    yield return gameState[y][x + 2];
                }
            }
        }

        public static bool IsKingMovingToOpponentThreatSquare(GameStateModel gameState, GameSquare startSquare, GameSquare targetSquare)
        {
            ChessPiece kingPiece = startSquare.Piece;
            startSquare.Piece = null;
            foreach (var opponentSquare in GetAllOpponentPieces(gameState, kingPiece.IsWhite))
            {
                if (opponentSquare.Piece.GetThreatSquares(gameState, opponentSquare).Contains(targetSquare))
                {
                    startSquare.Piece = kingPiece;
                    return true;
                }
            }
            startSquare.Piece = kingPiece;
            return false;
        }

        public static bool IsInOpponentThreatSquare(GameStateModel gameState, GameSquare targetSquare, bool isWhite)
        {
            foreach (var opponentSquare in GetAllOpponentPieces(gameState, isWhite))
            {
                if (opponentSquare.Piece.GetThreatSquares(gameState, opponentSquare).Contains(targetSquare))
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<GameSquare> GetAllOpponentPieces(GameStateModel gameState, bool isWhite)
        {
            foreach (var square in gameState.GetSquares())
            {
                if (square.Piece != null && square.Piece.IsWhite != isWhite)
                {
                    yield return square;
                }
            }
        }

        public static IEnumerable<GameSquare> GetBlackPawnThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x - 1, y + 1, false).CanThreaten())
            {
                yield return gameState[y + 1][x - 1];

            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, false).CanThreaten())
            {
                yield return gameState[y + 1][x + 1];
            }
        }

        public static IEnumerable<GameSquare> GetBlackPawnMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x, y + 1, false) == SquareBlockState.Available && !IsKingThreatened(gameState, square, gameState[y + 1][x]))
            {
                yield return gameState[y + 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 1, false) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, square, gameState[y + 1][x - 1]))
            {
                yield return gameState[y + 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, false) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, square, gameState[y + 1][x + 1]))
            {
                yield return gameState[y + 1][x + 1];
            }
            if (y == 1 && GetSquareBlockState(gameState, x, y + 1, false) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y + 2, false) == SquareBlockState.Available && !IsKingThreatened(gameState, square, gameState[y + 2][x]))
            {
                yield return gameState[y + 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetWhitePawnThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x - 1, y - 1, true).CanThreaten())
            {
                yield return gameState[y - 1][x - 1];

            }
            if (GetSquareBlockState(gameState, x + 1, y - 1, true).CanThreaten())
            {
                yield return gameState[y - 1][x + 1];
            }
        }

        public static IEnumerable<GameSquare> GetWhitePawnMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x, y - 1, true) == SquareBlockState.Available && !IsKingThreatened(gameState, square, gameState[y - 1][x]))
            {
                yield return gameState[y - 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 1, true) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, square, gameState[y - 1][x - 1]))
            {
                yield return gameState[y - 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y - 1, true) == SquareBlockState.OpponentPiece && !IsKingThreatened(gameState, square, gameState[y - 1][x + 1]))
            {
                yield return gameState[y - 1][x + 1];
            }
            if (y == 6 && GetSquareBlockState(gameState, x, y - 1, true) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y - 2, true) == SquareBlockState.Available && !IsKingThreatened(gameState, square, gameState[y - 2][x]))
            {
                yield return gameState[y - 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetQueenThreatSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerThreatSquares(gameState, square).Concat(GetBishopThreatSquares(gameState, square));
        }

        public static IEnumerable<GameSquare> GetQueenMovableSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerMovableSquares(gameState, square).Concat(GetBishopMovableSquares(gameState, square));
        }

        public static IEnumerable<GameSquare> GetTowerMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            // right
            for (int i = x + 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, i, y, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y][i]))
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
                SquareBlockState state = GetSquareBlockState(gameState, i, y, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y][i]))
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[j][x]))
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[j][x]))
                {
                    yield return gameState[j][x];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }

        public static IEnumerable<GameSquare> GetTowerThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            // right
            for (int i = x + 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, i, y, square.Piece.IsWhite);
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
                SquareBlockState state = GetSquareBlockState(gameState, i, y, square.Piece.IsWhite);
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, square.Piece.IsWhite);
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, square.Piece.IsWhite);
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

        private static bool IsKingThreatened(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
        {
            ChessPiece startChessPiece = startSquare.Piece;
            ChessPiece endChessPiece = endSquare.Piece;
            endSquare.Piece = startChessPiece;
            startSquare.Piece = null;
            GameSquare kingSquare = startChessPiece.IsWhite ? gameState.GetPieceSquare<WhiteKing>() : gameState.GetPieceSquare<BlackKing>();
            bool isInThreat = IsInOpponentThreatSquare(gameState, kingSquare, startChessPiece.IsWhite);
            startSquare.Piece = startChessPiece;
            endSquare.Piece = endChessPiece;
            return isInThreat;
        }

        public static IEnumerable<GameSquare> GetBishopMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            // upper right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - i][x + i]))
                {
                    yield return gameState[y - i][x + i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // lower right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y + i, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + i][x + i]))
                {
                    yield return gameState[y + i][x + i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // lower left
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y + i, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + i][x - i]))
                {
                    yield return gameState[y + i][x - i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // upper left
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y - i, square.Piece.IsWhite);
                if (state.CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - i][x - i]))
                {
                    yield return gameState[y - i][x - i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }

        public static IEnumerable<GameSquare> GetBishopThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            // upper right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, square.Piece.IsWhite);
                if (state.CanThreaten())
                {
                    yield return gameState[y - i][x + i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // lower right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y + i, square.Piece.IsWhite);
                if (state.CanThreaten())
                {
                    yield return gameState[y + i][x + i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // lower left
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y + i, square.Piece.IsWhite);
                if (state.CanThreaten())
                {
                    yield return gameState[y + i][x - i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
            // upper left
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y - i, square.Piece.IsWhite);
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

        public static IEnumerable<GameSquare> GetKnightThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x + 1, y - 2, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y - 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x + 2, y - 1, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y - 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 2, y + 1, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y + 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 2, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y + 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 2, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y + 2][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 2, y + 1, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y + 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 2, y - 1, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y - 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 2, square.Piece.IsWhite).CanThreaten())
            {
                yield return gameState[y - 2][x - 1];
            }
        }

        public static IEnumerable<GameSquare> GetKnightMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x + 1, y - 2, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - 2][x + 1]))
            {
                yield return gameState[y - 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x + 2, y - 1, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - 1][x + 2]))
            {
                yield return gameState[y - 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 2, y + 1, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + 1][x + 2]))
            {
                yield return gameState[y + 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 2, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + 2][x + 1]))
            {
                yield return gameState[y + 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 2, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + 2][x - 1]))
            {
                yield return gameState[y + 2][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 2, y + 1, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y + 1][x - 2]))
            {
                yield return gameState[y + 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 2, y - 1, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - 1][x - 2]))
            {
                yield return gameState[y - 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 2, square.Piece.IsWhite).CanMoveTo() && !IsKingThreatened(gameState, square, gameState[y - 2][x - 1]))
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
            else if (gameState[y][x].Piece.IsWhite != isWhite)
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
            return blockState == SquareBlockState.Available || blockState == SquareBlockState.OpponentPiece;
        }

        public static bool CanThreaten(this SquareBlockState blockState)
        {
            return blockState == SquareBlockState.Available || blockState == SquareBlockState.OpponentPiece || blockState == SquareBlockState.OwnPiece;
        }
    }
}
