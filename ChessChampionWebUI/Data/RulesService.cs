using ChessChampionWebUI.Models;
using ChessChampionWebUI.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessChampionWebUI.Data
{
    public static class RulesService
    {
        private static readonly string[] BlackPieces = new[] { "♜", "♞", "♝", "♛", "♚", "♟︎" };
        private static readonly string[] WhitePieces = new[] { "♖", "♘", "♗", "♕", "♔", "♙" };

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

        public static IEnumerable<GameSquare> GetKingThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != j && GetSquareBlockState(gameState, x + i, y + j, square.Piece.IsWhite).CanMoveTo(true))
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
                    if (i != j && GetSquareBlockState(gameState, x + i, y + j, square.Piece.IsWhite).CanMoveTo(false) && !IsInOpponentThreatSquare(gameState, gameState[y + j][x + i], square.Piece.IsWhite))
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

        public static bool IsInOpponentThreatSquare(GameStateModel gameState, GameSquare square, bool isWhite)
        {
            foreach (var opponentSquare in GetAllOpponentPieces(gameState, isWhite))
            {
                List<GameSquare> threatSquares = opponentSquare.Piece.GetThreatSquares(gameState, opponentSquare).ToList();
                if (threatSquares.Contains(square))
                {
                    return true;
                }
            }
            return false;
        }

        private static IEnumerable<GameSquare> GetAllOpponentPieces(GameStateModel gameState, bool isWhite)
        {
            foreach (var square in gameState.GetSquares())
            {
                if (square.Piece != null && square.Piece.IsWhite != isWhite && square.Piece.GetType() != typeof(BlackKing) && square.Piece.GetType() != typeof(WhiteKing))
                {
                    yield return square;
                }
            }
        }

        public static IEnumerable<GameSquare> GetBlackPawnThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x - 1, y + 1, false).CanMoveTo(true))
            {
                yield return gameState[y + 1][x - 1];

            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, false).CanMoveTo(true))
            {
                yield return gameState[y + 1][x + 1];
            }
        }

        public static IEnumerable<GameSquare> GetBlackPawnMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x, y + 1, false) == SquareBlockState.Available)
            {
                yield return gameState[y + 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 1, false) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y + 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 1, false) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y + 1][x + 1];
            }
            if (y == 1 && GetSquareBlockState(gameState, x, y + 1, false) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y + 2, false) == SquareBlockState.Available)
            {
                yield return gameState[y + 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetWhitePawnThreatSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x - 1, y - 1, true).CanMoveTo(true))
            {
                yield return gameState[y - 1][x - 1];

            }
            if (GetSquareBlockState(gameState, x + 1, y - 1, true).CanMoveTo(true))
            {
                yield return gameState[y - 1][x + 1];
            }
        }

        public static IEnumerable<GameSquare> GetWhitePawnMovableSquares(GameStateModel gameState, GameSquare square)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x, y - 1, true) == SquareBlockState.Available)
            {
                yield return gameState[y - 1][x];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 1, true) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y - 1][x - 1];
            }
            if (GetSquareBlockState(gameState, x + 1, y - 1, true) == SquareBlockState.OpponentPiece)
            {
                yield return gameState[y - 1][x + 1];
            }
            if (y == 6 && GetSquareBlockState(gameState, x, y - 1, true) == SquareBlockState.Available && GetSquareBlockState(gameState, x, y - 2, true) == SquareBlockState.Available)
            {
                yield return gameState[y - 2][x];
            }
        }

        public static IEnumerable<GameSquare> GetQueenSquares(GameStateModel gameState, GameSquare square, bool threaten)
        {
            return GetTowerSquares(gameState, square, threaten).Concat(GetBishopSquares(gameState, square, threaten));
        }

        #region Tower
        public static IEnumerable<GameSquare> GetTowerSquares(GameStateModel gameState, GameSquare square, bool threaten)
        {
            int x = square.X;
            int y = square.Y;
            // right
            for (int i = x + 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, i, y, square.Piece.IsWhite);
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
                {
                    yield return gameState[j][x];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }
        #endregion


        public static IEnumerable<GameSquare> GetBishopSquares(GameStateModel gameState, GameSquare square, bool threaten)
        {
            int x = square.X;
            int y = square.Y;
            // upper right
            for (int i = 1; i < 8; i++)
            {
                SquareBlockState state = GetSquareBlockState(gameState, x + i, y - i, square.Piece.IsWhite);
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
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
                if (state.CanMoveTo(threaten))
                {
                    yield return gameState[y - i][x - i];
                }
                if (state != SquareBlockState.Available)
                {
                    break;
                }
            }
        }

        public static IEnumerable<GameSquare> GetKnightSquares(GameStateModel gameState, GameSquare square, bool threaten)
        {
            int x = square.X;
            int y = square.Y;
            if (GetSquareBlockState(gameState, x + 1, y - 2, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y - 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x + 2, y - 1, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y - 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 2, y + 1, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y + 1][x + 2];
            }
            if (GetSquareBlockState(gameState, x + 1, y + 2, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y + 2][x + 1];
            }
            if (GetSquareBlockState(gameState, x - 1, y + 2, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y + 2][x - 1];
            }
            if (GetSquareBlockState(gameState, x - 2, y + 1, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y + 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 2, y - 1, square.Piece.IsWhite).CanMoveTo(threaten))
            {
                yield return gameState[y - 1][x - 2];
            }
            if (GetSquareBlockState(gameState, x - 1, y - 2, square.Piece.IsWhite).CanMoveTo(threaten))
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

        public static bool CanMoveTo(this SquareBlockState blockState, bool includeOwnPieces)
        {
            if (includeOwnPieces)
            {
                return blockState == SquareBlockState.Available || blockState == SquareBlockState.OpponentPiece || blockState == SquareBlockState.OwnPiece;
            }
            else
            {
                return blockState == SquareBlockState.Available || blockState == SquareBlockState.OpponentPiece;
            }
        }
    }
}
