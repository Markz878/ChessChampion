using ChessChampionWebUI.Components;
using ChessChampionWebUI.Models.Pieces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ChessChampionWebUI.Data.RulesService;

namespace ChessChampionWebUI.Models
{
    public class GameModel
    {
        public PlayerModel WhitePlayer { get; set; }
        public PlayerModel BlackPlayer { get; set; }
        public GameStateModel GameState { get; set; } = new GameStateModel();
        public bool IsWhitePlayerTurn { get; set; } = true;
        public PlayerModel Winner { get; set; }

        public event EventHandler StateChanged;

        public event EventHandler GameEnded;

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

        public async Task HandleSquareSelect(GameSquare square, PlayerModel player, ILogger<ChessBoardComponent> logger, int difficultyLevel)
        {
            if (player.IsWhite != IsWhitePlayerTurn)
            {
                return;
            }
            try
            {
                GameSquare selectedSquare = GetSelectedSquare();
                /// Different paths:
                /// 1) Nothing is selected, and user selects own piece
                /// 2) Own piece is selected, and user selects the same piece
                /// 3) Own piece is selected, and user selects another own piece
                /// 4) Own piece is selected, and user selects a movable square
                if (selectedSquare == null && square.Piece != null && IsPlayerPiece(square.Piece.Marker, player.IsWhite))
                {
                    HandlePieceSelect(square);
                }
                else if (selectedSquare == square)
                {
                    HandleSameSquareSelect(selectedSquare);
                }
                else if (!square.IsEmpty && IsPlayerPiece(square.Piece.Marker, player.IsWhite))
                {
                    HandleOtherPieceSelect(square, selectedSquare);
                }
                else if (square.State == SquareState.Movable)
                {
                    bool winnerFound = await HandleMove(square, selectedSquare, player, difficultyLevel, logger);
                    if (winnerFound)
                    {
                        GameEnded?.Invoke(this, EventArgs.Empty);
                    }
                    OnStateChanged();
                }
            }
            catch
            {
                GameEnded?.Invoke(this, EventArgs.Empty);
                throw;
            }
        }

        private void HandlePieceSelect(GameSquare square)
        {
            square.State = SquareState.Selected;
            foreach (var availableSquare in square.Piece.GetMovableSquares(GameState, square))
            {
                availableSquare.State = SquareState.Movable;
            }
        }

        private void HandleSameSquareSelect(GameSquare selectedSquare)
        {
            selectedSquare.State = SquareState.Normal;
            foreach (var availableSquare in selectedSquare.Piece.GetMovableSquares(GameState, selectedSquare))
            {
                availableSquare.State = SquareState.Normal;
            }
        }

        private void HandleOtherPieceSelect(GameSquare square, GameSquare selectedSquare)
        {
            selectedSquare.State = SquareState.Normal;
            foreach (var availableSquare in selectedSquare.Piece.GetMovableSquares(GameState, selectedSquare))
            {
                availableSquare.State = SquareState.Normal;
            }
            square.State = SquareState.Selected;
            foreach (var availableSquare in square.Piece.GetMovableSquares(GameState, square))
            {
                availableSquare.State = SquareState.Movable;
            }
        }

        private async Task<bool> HandleMove(GameSquare endSquare, GameSquare startSquare, PlayerModel player, int difficultyLevel, ILogger logger)
        {
            string move = startSquare.Piece.HandleMove(GameState, startSquare, endSquare);
            GameState.Moves += $" {move}";
            ResetBoardStates();
            CheckForWin(player.IsWhite);
            if (Winner != null)
            {
                return true;
            }
            IsWhitePlayerTurn = !IsWhitePlayerTurn;
            OnStateChanged();
            PlayerModel opponent = player.IsWhite ? BlackPlayer : WhitePlayer;
            if (opponent is AIPlayerModel ai)
            {
                string aimove = await ai.Move(GameState, logger);
                GameState.Moves += $" {aimove}";
                CheckForWin(!player.IsWhite);
                if (Winner != null)
                {
                    return true;
                }
                IsWhitePlayerTurn = !IsWhitePlayerTurn;
                OnStateChanged();
            }
            return false;
        }

        private void CheckForWin(bool isWhite)
        {
            if (isWhite)
            {
                GameSquare blackKingSquare = GameState.GetPieceSquare<BlackKing>();
                if (IsInOpponentThreatSquare(GameState, blackKingSquare, false))
                {
                    foreach (var opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                    {
                        if (opponentSquare.Piece.GetMovableSquares(GameState, opponentSquare).Any())
                        {
                            return;
                        }
                    }
                    Winner = WhitePlayer;
                }
            }
            else
            {
                GameSquare whiteKingSquare = GameState.GetPieceSquare<WhiteKing>();
                if (IsInOpponentThreatSquare(GameState, whiteKingSquare, false))
                {
                    foreach (var opponentSquare in GetAllOpponentPieces(GameState, isWhite))
                    {
                        if (opponentSquare.Piece.GetMovableSquares(GameState, opponentSquare).Any())
                        {
                            return;
                        }
                    }
                    Winner = BlackPlayer;
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

        public void OnGameEnded()
        {
            GameEnded?.Invoke(this, EventArgs.Empty);
        }

        public void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
