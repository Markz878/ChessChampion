using ChessChampion.Shared.Models;

namespace ChessChampion.Shared.Models.Pieces;

public abstract class ChessPiece(string marker, bool isWhite)
{
    public string Marker { get; set; } = marker;
    public bool IsWhite { get; set; } = isWhite;

    public abstract IEnumerable<GameSquare> GetThreatSquares(GameStateModel gameState, int x, int y);

    public abstract IEnumerable<GameSquare> GetMovableSquares(GameStateModel gameState, int x, int y);

    public virtual string HandleMove(GameStateModel gameState, GameSquare startSquare, GameSquare endSquare)
    {
        foreach (GameSquare square in gameState.GetSquares())
        {
            square.WasPreviousMove = false;
        }
        endSquare.Piece = this;
        startSquare.Piece = null;
        startSquare.WasPreviousMove = true;
        endSquare.WasPreviousMove = true;
        return startSquare.ChessCoordinate + endSquare.ChessCoordinate;
    }
}