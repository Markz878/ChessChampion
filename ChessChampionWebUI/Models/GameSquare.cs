namespace ChessChampionWebUI.Models
{
    public class GameSquare
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Piece { get; set; }
        public SquareState State { get; set; }
        public bool IsEmpty => string.IsNullOrEmpty(Piece);
        public string ChessCoordinate => char.ConvertFromUtf32(97 + Column) + (8 - Row).ToString();
    }



    public enum SquareState
    {
        Normal,
        Selected,
        Movable
    }
}
