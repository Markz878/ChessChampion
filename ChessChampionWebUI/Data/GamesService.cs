using ChessChampionWebUI.Models;
using System.Collections.Generic;

namespace ChessChampionWebUI.Data
{
    public class GamesService
    {
        public Dictionary<string, GameModel> Games { get; set; } = new Dictionary<string, GameModel>();

        public GameModel CreateGameVsComputer(bool ChooseWhitePieces, PlayerModel User, int level)
        {
            return new GameModel()
            {
                BlackPlayer = ChooseWhitePieces ? new AIPlayerModel(level) : User,
                WhitePlayer = ChooseWhitePieces ? User : new AIPlayerModel(level),
                IsPlayerWhite = ChooseWhitePieces,
            };
        }
    }
}
