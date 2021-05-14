using ChessChampionWebUI.Models;
using System.Collections.Generic;

namespace ChessChampionWebUI.Data
{
    public class GamesService
    {
        public Dictionary<string, GameModel> Games { get; set; } = new Dictionary<string, GameModel>();
    }
}
