using System.ComponentModel.DataAnnotations;

namespace ChessChampionWebUI.Models;

public class CreateGameFormModel
{
    [Required]
    [MinLength(3)]
    public string? UserName { get; set; }
    public string? GameCode { get; set; }
}
