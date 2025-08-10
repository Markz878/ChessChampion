using System.ComponentModel.DataAnnotations;

namespace ChessChampion.Shared.Models;

public sealed record SubmitMoveRequest(
    [Required] Guid GameId,
    [Required] string Move,
    [Required] string UserName
);
