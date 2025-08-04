using ChessChampionWebUI.Models;
using System.Collections.Generic;

namespace ChessChampionWebUI.Data
{
    public class GamesService
    {
        private readonly Dictionary<string, GameModel> games = new();

        public bool CreateGame(string code, GameModel game)
        {
            return games.TryAdd(code.ToLower(), game);
        }

        public bool TryGetGame(string code, out GameModel game)
        {
            return games.TryGetValue(code.ToLower(), out game);
        }

        public bool DeleteGame(string code)
        {
            return games.Remove(code.ToLower());
        }
    }
}
