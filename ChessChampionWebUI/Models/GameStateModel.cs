using ChessChampionWebUI.Models.Pieces;
using System;
using System.Collections.Generic;

namespace ChessChampionWebUI.Models
{
    public class GameStateModel
    {
        public GameSquare[][] State { get; } = new GameSquare[8][];
        public string Moves { get; set; }
        public bool CanWhiteKingCastleRight { get; set; } = true;
        public bool CanWhiteKingCastleLeft { get; set; } = true;
        public bool CanBlackKingCastleRight { get; set; } = true;
        public bool CanBlackKingCastleLeft { get; set; } = true;

        public GameStateModel()
        {
            State[0] =
            [
                new GameSquare(){ Y=0, X=0, Piece = new BlackTower() },
                new GameSquare(){ Y=0, X=1, Piece = new BlackKnight() },
                new GameSquare(){ Y=0, X=2, Piece = new BlackBishop() },
                new GameSquare(){ Y=0, X=3, Piece = new BlackQueen() },
                new GameSquare(){ Y=0, X=4, Piece = new BlackKing() },
                new GameSquare(){ Y=0, X=5, Piece = new BlackBishop() },
                new GameSquare(){ Y=0, X=6, Piece = new BlackKnight() },
                new GameSquare(){ Y=0, X=7, Piece = new BlackTower() },
            ];
            State[1] = new GameSquare[8];
            for (int i = 0; i < State[1].Length; i++)
            {
                State[1][i] = new GameSquare() { Y = 1, X = i, Piece = new BlackPawn() };
            }
            for (int j = 2; j < 6; j++)
            {
                State[j] = new GameSquare[8];
                for (int i = 0; i < State[j].Length; i++)
                {
                    State[j][i] = new GameSquare() { Y = j, X = i };
                }
            }
            State[6] = new GameSquare[8];
            for (int i = 0; i < State[6].Length; i++)
            {
                State[6][i] = new GameSquare() { Y = 6, X = i, Piece = new WhitePawn() };
            }
            State[7] =
            [
                new GameSquare(){ Y=7, X=0, Piece = new WhiteTower() },
                new GameSquare(){ Y=7, X=1, Piece = new WhiteKnight() },
                new GameSquare(){ Y=7, X=2, Piece = new WhiteBishop() },
                new GameSquare(){ Y=7, X=3, Piece = new WhiteQueen() },
                new GameSquare(){ Y=7, X=4, Piece = new WhiteKing() },
                new GameSquare(){ Y=7, X=5, Piece = new WhiteBishop() },
                new GameSquare(){ Y=7, X=6, Piece = new WhiteKnight() },
                new GameSquare(){ Y=7, X=7, Piece = new WhiteTower() },
            ];
        }

        public IEnumerable<GameSquare> GetSquares()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    yield return State[i][j];
                }
            }
        }

        public GameSquare[] this[int row]
        {
            get => State[row];
        }

        public GameSquare this[string coordinate]
        {
            get => GetSquareFromCoordinates(coordinate);
        }

        public GameSquare GetSquareFromCoordinates(string coordinate)
        {
            int column = char.ConvertToUtf32(coordinate, 0) - 97;
            int row = 8 - int.Parse(coordinate[1].ToString());
            return State[row][column];
        }

        internal GameSquare GetPieceSquare<T>()
        {
            foreach (var square in GetSquares())
            {
                if (square.Piece != null && square.Piece is T)
                {
                    return square;
                }
            }
            throw new InvalidOperationException("Piece not found");
        }
    }
}