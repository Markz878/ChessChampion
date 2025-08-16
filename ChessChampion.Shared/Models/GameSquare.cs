using ChessChampion.Shared.Models.Pieces;

namespace ChessChampion.Shared.Models;

public class GameSquare
{
    public int X { get; set; }
    public int Y { get; set; }
    public ChessPiece? Piece { get; set; }
    public bool WasPreviousMove { get; set; }
    public bool IsEmpty => Piece == null;
    public string ChessCoordinate => char.ConvertFromUtf32(97 + X) + (Y + 1).ToString();
}
