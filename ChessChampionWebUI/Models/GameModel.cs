using ChessChampionWebUI.Data;
using System;
using System.Linq;

namespace ChessChampionWebUI.Models
{
    public class GameModel
    {
        public UserModel WhitePlayer { get; set; } = new UserModel();
        public UserModel BlackPlayer { get; set; } = new UserModel();
        public GameStateModel GameState { get; set; } = new GameStateModel();
        public GameSquare GetSelectedSquare()
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

        internal void HandleSquareSelect(GameSquare square)
        {
            GameSquare selectedSquare = GetSelectedSquare();

            if (selectedSquare == null)
            {
                if (square.IsEmpty)
                {
                    ResetBoardStates();
                }
                else
                {
                    square.State = SquareState.Selected;
                    foreach (var availableSquare in RulesService.GetAvailableSquares(GameState, square))
                    {
                        availableSquare.State = SquareState.Movable;
                    }
                }
            }
            else if (selectedSquare == square)
            {
                selectedSquare.State = SquareState.Normal;
                foreach (var availableSquare in RulesService.GetAvailableSquares(GameState, square))
                {
                    availableSquare.State = SquareState.Normal;
                }
            }
            else if (square.State == SquareState.Movable)
            {
                square.Piece = selectedSquare.Piece;
                selectedSquare.Piece = "";
                ResetBoardStates();
            }
            else if (RulesService.WhitePieces.Contains(selectedSquare.Piece))
            {

            }

        }

        public void ResetBoardStates()
        {
            foreach (var square in GameState.GetSquares())
            {
                square.State = SquareState.Normal;
            }
        }
    }
}
