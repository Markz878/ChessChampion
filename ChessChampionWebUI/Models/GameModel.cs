using ChessChampionWebUI.Data;
using System;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class GameModel
    {
        public PlayerModel WhitePlayer { get; set; }
        public PlayerModel BlackPlayer { get; set; }
        public GameStateModel GameState { get; set; } = new GameStateModel();
        public bool IsWhitePlayerTurn { get; set; } = true;
        
        public event EventHandler OnStateChanged;

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

        public async Task HandleSquareSelect(GameSquare square, PlayerModel player)
        {
            if (player.IsWhite != IsWhitePlayerTurn)
            {
                return;
            }
            GameSquare selectedSquare = GetSelectedSquare();
            /// Different paths:
            /// 1) Nothing is selected, and user selects own piece
            /// 2) Own piece is selected, and user selects the same piece
            /// 3) Own piece is selected, and user selects another own piece
            /// 4) Own piece is selected, and user selects a movable square
            if (selectedSquare == null && square.Piece != null && RulesService.IsPlayerPiece(square.Piece.Marker, player.IsWhite))
            {
                HandlePieceSelect(square);
            }
            else if (selectedSquare == square)
            {
                HandleSameSquareSelect(selectedSquare);
            }
            else if (!square.IsEmpty && RulesService.IsPlayerPiece(square.Piece.Marker, player.IsWhite))
            {
                HandleOtherPieceSelect(square, selectedSquare);
            }
            else if (square.State == SquareState.Movable)
            {
                await HandleMove(square, selectedSquare, player);
                NotifyOfChange();
            }
        }

        private void HandlePieceSelect(GameSquare square)
        {
            square.State = SquareState.Selected;
            foreach (var availableSquare in square.Piece.GetAvailableSquares(GameState, square))
            {
                availableSquare.State = SquareState.Movable;
            }
        }

        private void HandleSameSquareSelect(GameSquare selectedSquare)
        {
            selectedSquare.State = SquareState.Normal;
            foreach (var availableSquare in selectedSquare.Piece.GetAvailableSquares(GameState, selectedSquare))
            {
                availableSquare.State = SquareState.Normal;
            }
        }

        private void HandleOtherPieceSelect(GameSquare square, GameSquare selectedSquare)
        {
            selectedSquare.State = SquareState.Normal;
            foreach (var availableSquare in selectedSquare.Piece.GetAvailableSquares(GameState, selectedSquare))
            {
                availableSquare.State = SquareState.Normal;
            }
            square.State = SquareState.Selected;
            foreach (var availableSquare in square.Piece.GetAvailableSquares(GameState, square))
            {
                availableSquare.State = SquareState.Movable;
            }
        }

        private async Task HandleMove(GameSquare endSquare, GameSquare startSquare, PlayerModel player)
        {
            startSquare.Piece.HandleMove(GameState, startSquare, endSquare);
            ResetBoardStates();
            IsWhitePlayerTurn = !IsWhitePlayerTurn;
            NotifyOfChange();
            PlayerModel opponent = player.IsWhite ? BlackPlayer : WhitePlayer;
            if (opponent is AIPlayerModel ai)
            {
                string move = startSquare.ChessCoordinate + endSquare.ChessCoordinate;
                await ai.Move(GameState, move);
                IsWhitePlayerTurn = !IsWhitePlayerTurn;
            }
        }

        public void ResetBoardStates()
        {
            foreach (var square in GameState.GetSquares())
            {
                square.State = SquareState.Normal;
            }
        }

        public void NotifyOfChange()
        {
            OnStateChanged?.Invoke(this, null);
        }
    }
}
