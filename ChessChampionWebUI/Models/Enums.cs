namespace ChessChampionWebUI.Models
{
    public enum PlayVsMode
    {
        PlayVsComputer,
        PlayVsHuman
    }
    
    public enum JoinGameMode
    {
        CreateGame,
        JoinGame
    }

    public enum SquareBlockState
    {
        Available,
        OutOfBounds,
        OwnPiece,
        OpponentPiece
    }

    public enum SquareState
    {
        Normal,
        Selected,
        Movable
    }
}
