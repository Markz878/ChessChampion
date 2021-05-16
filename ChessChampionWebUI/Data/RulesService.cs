using ChessChampionWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessChampionWebUI.Data
{
    public static class RulesService
    {
        public static string[] BlackPieces { get; } = new[] { "♜", "♞", "♝", "♛", "♚", "♟︎" };
        public static string[] WhitePieces { get; } = new[] { "♖", "♘", "♗", "♕", "♔", "♙" };



        public static bool IsWhitePiece(string piece) => WhitePieces.Contains(piece);
        public static bool IsBlackPiece(string piece) => BlackPieces.Contains(piece);

        //public static IEnumerable<GameSquare> GetAvailableSquares(GameStateModel gameState, GameSquare square) => square.Piece switch
        //{
        //    "♜" => GetTowerSquares(gameState, square, WhitePieces),
        //    "♞" => GetKnightSquares(gameState, square, WhitePieces),
        //    "♝" => GetBishopSquares(gameState, square, WhitePieces),
        //    "♛" => GetQueenSquares(gameState, square, WhitePieces),
        //    "♚" => GetKingSquares(gameState, square, WhitePieces),
        //    "♟︎" => GetBlackPawnSquares(gameState, square),
        //    "♖" => GetTowerSquares(gameState, square, BlackPieces),
        //    "♘" => GetKnightSquares(gameState, square, BlackPieces),
        //    "♗" => GetBishopSquares(gameState, square, BlackPieces),
        //    "♕" => GetQueenSquares(gameState, square, BlackPieces),
        //    "♔" => GetKingSquares(gameState, square, BlackPieces),
        //    "♙" => GetWhitePawnSquares(gameState, square),
        //    _ => throw new ArgumentOutOfRangeException()
        //};

        public static bool IsPlayerPiece(string piece, bool isPlayerWhite)
        {
            return (IsWhitePiece(piece) && isPlayerWhite) || (IsBlackPiece(piece) && !isPlayerWhite);
        }

        public static IEnumerable<GameSquare> GetKingSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            string[] opponentPieces = square.Piece.IsWhite ? BlackPieces : WhitePieces;
            if (GetSquareBlockState(gameState, x + 1, y, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y][x + 1], opponentPieces))
            {
                yield return gameState[y][x + 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y + 1][x + 1], opponentPieces))
            {
                yield return gameState[y + 1][x + 1];
            }
            if (GetSquareBlockState(gameState, x, y + 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y + 1][x], opponentPieces))
            {
                yield return gameState[y + 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y + 1][x - 1], opponentPieces))
            {
                yield return gameState[y + 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y + 0][x - 1], opponentPieces))
            {
                yield return gameState[y + 0][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y - 1][x - 1], opponentPieces))
            {
                yield return gameState[y - 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x, y - 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y - 1][x], opponentPieces))
            {
                yield return gameState[y - 1][x + 0];
            }
            if (GetSquareBlockState(gameState, x + 0, y + 1, opponentPieces).CanMoveTo() && !IsInOpponentThreatSquare(gameState, gameState[y + 1][x], opponentPieces))
            {
                yield return gameState[y + 1][x];
            }
        }

        private static bool IsInOpponentThreatSquare(GameStateModel gameState, GameSquare square, string[] opponentPieces)
        {
            return false;
        }

        public static IEnumerable<GameSquare> GetBlackPawnSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x, y + 1, WhitePieces) == SquareBlockState.Available)
            {
                yield return gameState[y + 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 1, WhitePieces) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y + 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, WhitePieces) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y + 1][x + 1];
            }
            if (y == 1 && GetSquareBlockState(gameState, x, y + 1, WhitePieces) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y + 2, WhitePieces) == SquareBlockState.Available)
            {
                yield return gameState[y + 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetWhitePawnSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            string[] opponentPieces = square.Piece.IsWhite ? BlackPieces : WhitePieces;
            if (GetSquareBlockState(gameState, x, y - 1, BlackPieces) == SquareBlockState.Available)
            {
                yield return gameState[y - 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 1, BlackPieces) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y - 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y - 1, BlackPieces) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y - 1][x + 1];
            }
            if (y == 6 && GetSquareBlockState(gameState, x, y - 1, BlackPieces) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y - 2, BlackPieces) == SquareBlockState.Available)
            {
                yield return gameState[y - 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetQueenSquares(GameStateModel gameState, GameSquare square)
        {
            return GetTowerSquares(gameState, square).Concat(GetBishopSquares(gameState, square));
        }

        public static IEnumerable<GameSquare> GetTowerSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            string[] opponentPieces = square.Piece.IsWhite ? BlackPieces : WhitePieces;
            // right
            for (int i = x + 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, i, y, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, i, y, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, x, j, opponentPieces);
                if (state.CanMoveTo())
                {
                    yield return gameState[j][x];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }

        public static IEnumerable<GameSquare> GetBishopSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            string[] opponentPieces = square.Piece.IsWhite ? BlackPieces : WhitePieces;
            // upper right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y + i, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y + i, opponentPieces);
                if (state.CanMoveTo())
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
                SquareBlockState state = GetSquareBlockState(gameState, x - i, y - i, opponentPieces);
                if (state.CanMoveTo())
                {
                    yield return gameState[y - i][x - i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }

        public static IEnumerable<GameSquare> GetKnightSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            string[] opponentPieces = square.Piece.IsWhite ? BlackPieces : WhitePieces;
            if (GetSquareBlockState(gameState, x + 1, y - 2, opponentPieces).CanMoveTo())
            {
                yield return gameState[y - 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x + 2, y - 1, opponentPieces).CanMoveTo())
            {
                yield return gameState[y - 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 2, y + 1, opponentPieces).CanMoveTo())
            {
                yield return gameState[y + 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 2, opponentPieces).CanMoveTo())
            {
                yield return gameState[y + 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 2, opponentPieces).CanMoveTo())
            {
                yield return gameState[y + 2][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 2, y + 1, opponentPieces).CanMoveTo())
            {
                yield return gameState[y + 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 2, y - 1, opponentPieces).CanMoveTo())
            {
                yield return gameState[y - 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 2, opponentPieces).CanMoveTo())
            {
                yield return gameState[y - 2][x - 1];
            }
        }

        public static SquareBlockState GetSquareBlockState(GameStateModel gameState, int x, int y, string[] opponentPieces)
        {
            if (x < 0 || x > 7 || y < 0 || y > 7)
            {
                return SquareBlockState.OutOfBounds;
            }
            if (gameState[y][x].IsEmpty)
            {
                return SquareBlockState.Available;
            }
            else if (opponentPieces.Contains(gameState[y][x].Piece.Marker))
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
    }
}
