using ChessChampion.Core.Models;
using ChessChampion.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ChessChampion.Core.Data;

public sealed class GamesRepository(IConfiguration configuration)
{
    private readonly Dictionary<string, GameModel> availableGames = [];
    private readonly Dictionary<Guid, GameModel> games = [];

    public GameModel CreateGame(PlayerModel player)
    {
        GameModel game = new();
        string code = CreateGameCode();
        while (!availableGames.TryAdd(code.ToLower(), game))
        {
            code = CreateGameCode();
        }
        game.Code = code;
        if (player.IsWhite)
        {
            game.WhitePlayer = player;
        }
        else
        {
            game.BlackPlayer = player;
        }
        if (!games.TryAdd(game.Id, game))
        {
            availableGames.Remove(code.ToLower()); // Remove the game code if the game ID already exists
            return CreateGame(player); // Retry if the game ID already exists
        }
        return game;
    }

    public GameModel CreateGameWithAI(PlayerModel player, int skillLevel)
    {
        GameModel game = new();
        AIPlayerModel ai = new(skillLevel, configuration["EngineFileName"] ?? throw new Exception("No engine file name found in config."));
        if (player.IsWhite)
        {
            game.WhitePlayer = player;
            game.BlackPlayer = ai;
        }
        else
        {
            game.BlackPlayer = player;
            game.WhitePlayer = ai;
        }
        if (!games.TryAdd(game.Id, game))
        {
            return CreateGame(player); // Retry if the game ID already exists
        }
        return game;
    }

    private static string CreateGameCode()
    {
        StringBuilder builder = new();
        for (int i = 0; i < 4; i++)
        {
            builder.Append(char.ConvertFromUtf32(Random.Shared.Next(65, 91)));
        }
        return builder.ToString();
    }

    public bool TryToJoinGame(string code, string player2Name, [NotNullWhen(true)] out bool? playerIsWhites, [NotNullWhen(true)] out GameModel? game)
    {
        if (availableGames.TryGetValue(code.ToLower(), out game))
        {
            if (game.WhitePlayer is null)
            {
                playerIsWhites = true;
                game.WhitePlayer = new PlayerModel(player2Name, true);
            }
            else if (game.BlackPlayer is null)
            {
                playerIsWhites = false;
                game.BlackPlayer = new PlayerModel(player2Name, false);
            }
            else
            {
                playerIsWhites = null; // Game is full
                return false;
            }
            availableGames.Remove(code.ToLower());
            return true;
        }
        else
        {
            playerIsWhites = null; // Game is full
            return false;
        }
    }

    public bool TryGetGame(Guid id, out GameModel? game)
    {
        return games.TryGetValue(id, out game);
    }

    public bool DeleteGame(Guid id)
    {
        return games.Remove(id);
    }
}
