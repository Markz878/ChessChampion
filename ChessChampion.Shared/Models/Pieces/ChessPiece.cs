using System.Text.Json.Serialization;

namespace ChessChampion.Shared.Models.Pieces;

[JsonDerivedType(typeof(BlackBishop), nameof(BlackBishop))]
[JsonDerivedType(typeof(BlackKing), nameof(BlackKing))]
[JsonDerivedType(typeof(BlackKnight), nameof(BlackKnight))]
[JsonDerivedType(typeof(BlackPawn), nameof(BlackPawn))]
[JsonDerivedType(typeof(BlackQueen), nameof(BlackQueen))]
[JsonDerivedType(typeof(BlackTower), nameof(BlackTower))]
[JsonDerivedType(typeof(WhiteBishop), nameof(WhiteBishop))]
[JsonDerivedType(typeof(WhiteKing), nameof(WhiteKing))]
[JsonDerivedType(typeof(WhiteKnight), nameof(WhiteKnight))]
[JsonDerivedType(typeof(WhitePawn), nameof(WhitePawn))]
[JsonDerivedType(typeof(WhiteQueen), nameof(WhiteQueen))]
[JsonDerivedType(typeof(WhiteTower), nameof(WhiteTower))]
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