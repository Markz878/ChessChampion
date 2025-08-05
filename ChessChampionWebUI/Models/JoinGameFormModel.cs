using System.ComponentModel.DataAnnotations;

namespace ChessChampionWebUI.Models;

public class JoinGameFormModel
{
    [Required]
    [MinLength(3)]
    public string? UserName { get; set; }
    [Required]
    [MinLength(4)]
    public string? GameCode { get; set; }
}
