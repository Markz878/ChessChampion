using ChessChampion.Core.Data;
using ChessChampion.Core.Models;
using ChessChampion.Hubs;
using ChessChampion.Installers;
using ChessChampion.Shared.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace ChessChampion.Endpoints;

public static class EndpointsMapper
{
    public static void MapEndpoints(this WebApplication app)
    {
        RouteGroupBuilder apiGroup = app.MapGroup("api").RequireAuthorization();
        apiGroup.RequireRateLimiting(RateLimitInstaller.PolicyName);
        apiGroup.MapPost("/create", CreateGame);
        apiGroup.MapPost("/join", JoinGame);
        apiGroup.MapPost("/leave", LeaveGame);
        apiGroup.MapPost("/move", SubmitMove);
    }

    public static Results<Ok<CreateGameResponse>, BadRequest<string>> CreateGame(CreateGameRequest createGameRequest, GamesService gamesService, IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(createGameRequest.UserName))
        {
            return TypedResults.BadRequest("User name is required.");
        }
        PlayerModel player = new(createGameRequest.UserName, createGameRequest.IsWhites);
        GameModel game = createGameRequest.VersusAI
            ? gamesService.CreateGameWithAI(player, createGameRequest.AISkillLevel.GetValueOrDefault(10))
            : gamesService.CreateGame(player);
        return TypedResults.Ok(new CreateGameResponse(game.Id, game.Code ?? "", game.GameState));
    }

    public static Results<Ok<JoinGameResponse>, BadRequest<string>, NotFound<string>> JoinGame(JoinGameRequest joinGameRequest, GamesService gamesService)
    {
        if (string.IsNullOrEmpty(joinGameRequest.UserName))
        {
            return TypedResults.BadRequest("User name is required.");
        }
        if (string.IsNullOrEmpty(joinGameRequest.GameCode))
        {
            return TypedResults.BadRequest("Game code is required.");
        }
        if (gamesService.TryToJoinGame(joinGameRequest.GameCode, joinGameRequest.UserName, out bool? playerIsWhites, out GameModel? game) && playerIsWhites.HasValue && game is not null)
        {
            return TypedResults.Ok(new JoinGameResponse(game.Id, game.GameState, playerIsWhites.Value ? game.BlackPlayer! : game.WhitePlayer!));
        }

        return TypedResults.NotFound("Game not found.");
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> SubmitMove(SubmitMoveRequest moveRequest, GamesService gamesService, IHubContext<ChessHub> hub)
    {
        if (gamesService.TryGetGame(moveRequest.GameId, out GameModel? game) && game is not null)
        {
            if (game.TryMakeMove(moveRequest.Move, out PlayerModel? winner))
            {
                await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.MoveReceived), moveRequest.Move);
                if (winner is not null)
                {
                    await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.Name);
                }
                return TypedResults.NoContent();
            }
            return TypedResults.BadRequest("Invalid move.");
        }
        return TypedResults.NotFound("Game not found.");
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound>> LeaveGame(LeaveGameRequest leaveRequest, GamesService gamesService, IHubContext<ChessHub> hub)
    {
        if (gamesService.TryGetGame(leaveRequest.GameId, out GameModel? game) && game is not null)
        {
            if (game.WhitePlayer?.Name == leaveRequest.UserName || game.BlackPlayer?.Name == leaveRequest.UserName)
            {
                await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.PlayerLeft), leaveRequest.UserName);
                gamesService.DeleteGame(leaveRequest.GameId);
                return TypedResults.NoContent();
            }
            return TypedResults.BadRequest("You are not part of the game.");
        }
        return TypedResults.NotFound();
    }
}
