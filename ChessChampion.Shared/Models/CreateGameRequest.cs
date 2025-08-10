namespace ChessChampion.Shared.Models;

public sealed record CreateGameRequest
{
    public string? UserName { get; set; }
    public bool IsWhites { get; set; }
    public bool VersusAI { get; set; }
    public int? AISkillLevel { get; set; }
}