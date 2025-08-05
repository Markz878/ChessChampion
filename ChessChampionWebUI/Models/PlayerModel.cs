namespace ChessChampionWebUI.Models;

public class PlayerModel(string name, bool isWhite)
{
    public string Name { get; set; } = name;
    public bool IsWhite { get; set; } = isWhite;
}
