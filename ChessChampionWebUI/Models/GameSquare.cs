namespace ChessChampionWebUI.Models
{
    public class GameSquare
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Piece { get; set; }
        public SquareState State { get; set; }
        public bool IsEmpty => string.IsNullOrEmpty(Piece);
    }

    public enum SquareState
    {
        Normal,
        Selected,
        Movable
    }
}
