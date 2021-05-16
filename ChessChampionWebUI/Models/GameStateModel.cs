using System.Collections.Generic;

namespace ChessChampionWebUI.Models
{
    public class GameStateModel
    {
        internal GameSquare[][] State { get; } = new GameSquare[8][];
        public GameStateModel()
        {
            State[0] = new GameSquare[]
            {
                new GameSquare(){ Row=0, Column=0, Piece = "♜" },
                new GameSquare(){ Row=0, Column=1, Piece = "♞" },
                new GameSquare(){ Row=0, Column=2, Piece = "♝" },
                new GameSquare(){ Row=0, Column=3, Piece = "♛" },
                new GameSquare(){ Row=0, Column=4, Piece = "♚" },
                new GameSquare(){ Row=0, Column=5, Piece = "♝" },
                new GameSquare(){ Row=0, Column=6, Piece = "♞" },
                new GameSquare(){ Row=0, Column=7, Piece = "♜" },
            };
            State[1] = new GameSquare[8];
            for (int i = 0; i < State[1].Length; i++)
            {
                State[1][i] = new GameSquare() { Row = 1, Column = i, Piece = "♟︎" };
            }
            for (int j = 2; j < 6; j++)
            {
                State[j] = new GameSquare[8];
                for (int i = 0; i < State[j].Length; i++)
                {
                    State[j][i] = new GameSquare() { Row = j, Column = i, Piece = "" };
                }
            }
            State[6] = new GameSquare[8];
            for (int i = 0; i < State[6].Length; i++)
            {
                State[6][i] = new GameSquare() { Row = 6, Column = i, Piece = "♙" };
            }
            State[7] = new GameSquare[]
            {
                new GameSquare(){ Row=7, Column=0, Piece = "♖" },
                new GameSquare(){ Row=7, Column=1, Piece = "♘" },
                new GameSquare(){ Row=7, Column=2, Piece = "♗" },
                new GameSquare(){ Row=7, Column=3, Piece = "♕" },
                new GameSquare(){ Row=7, Column=4, Piece = "♔" },
                new GameSquare(){ Row=7, Column=5, Piece = "♗" },
                new GameSquare(){ Row=7, Column=6, Piece = "♘" },
                new GameSquare(){ Row=7, Column=7, Piece = "♖" },
            };
        }

        internal IEnumerable<GameSquare> GetSquares()
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

        public GameSquare GetSquareFromCoordinates(string coordinate)
        {
            int column = char.ConvertToUtf32(coordinate, 0) - 97;
            int row = 8 - int.Parse(coordinate[1].ToString());
            return State[row][column];
        }
    }
}