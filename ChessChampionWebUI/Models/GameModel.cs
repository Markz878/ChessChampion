using ChessChampionWebUI.Data;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Models
{
    public class GameModel
    {
        public PlayerModel WhitePlayer { get; set; } = new PlayerModel();
        public PlayerModel BlackPlayer { get; set; } = new PlayerModel();
        public GameStateModel GameState { get; set; } = new GameStateModel();
        public bool IsPlayerWhite { get; set; }
        public bool IsWhitePlayerTurn { get; set; } = true;
        public bool IsPlayerTurn => (IsPlayerWhite && IsWhitePlayerTurn) || (!IsPlayerWhite && !IsWhitePlayerTurn);
        public PlayerModel Opponent => IsPlayerWhite ? BlackPlayer : WhitePlayer;
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

        public async Task HandleSquareSelect(GameSquare square)
        {
            if (!IsPlayerTurn)
            {
                return;
            }
            GameSquare selectedSquare = GetSelectedSquare();
            string move;
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
                move = selectedSquare.ChessCoordinate + square.ChessCoordinate;
                square.Piece = selectedSquare.Piece;
                selectedSquare.Piece = "";
                ResetBoardStates();
                IsWhitePlayerTurn = !IsWhitePlayerTurn;
                if (Opponent is AIPlayerModel ai)
                {
                    string aiMove = await ai.Move(move);
                    GameSquare startSquare = GameState.GetSquareFromCoordinates(aiMove[..2]);
                    GameSquare endSquare = GameState.GetSquareFromCoordinates(aiMove[2..4]);
                    endSquare.Piece = startSquare.Piece;
                    startSquare.Piece = "";
                    IsWhitePlayerTurn = !IsWhitePlayerTurn;
                }
            }
            else if ((RulesService.IsWhitePiece(selectedSquare.Piece) && RulesService.IsWhitePiece(square.Piece)) ||
                (RulesService.IsBlackPiece(selectedSquare.Piece) && RulesService.IsBlackPiece(square.Piece)))
            {
                selectedSquare.State = SquareState.Normal;
                foreach (var availableSquare in RulesService.GetAvailableSquares(GameState, selectedSquare))
                {
                    availableSquare.State = SquareState.Normal;
                }
                square.State = SquareState.Selected;
                foreach (var availableSquare in RulesService.GetAvailableSquares(GameState, square))
                {
                    availableSquare.State = SquareState.Movable;
                }
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
