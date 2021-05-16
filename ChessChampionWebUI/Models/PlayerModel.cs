using System.ComponentModel.DataAnnotations;

namespace ChessChampionWebUI.Models
{
    public class PlayerModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
        [Required]
        [MinLength(4)]
        public string GameCode { get; set; }
    }
}
