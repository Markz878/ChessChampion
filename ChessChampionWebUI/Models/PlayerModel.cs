﻿using System.ComponentModel.DataAnnotations;

namespace ChessChampionWebUI.Models
{
    public class PlayerModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }
    }
}
