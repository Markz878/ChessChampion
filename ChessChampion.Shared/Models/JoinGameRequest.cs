using System.ComponentModel.DataAnnotations;

namespace ChessChampion.Shared.Models;

public class JoinGameRequest
{
    [Required]
    [MinLength(3)]
    public string? UserName { get; set; }
    [Required]
    [MinLength(4)]
    public string? GameCode { get; set; }
}
