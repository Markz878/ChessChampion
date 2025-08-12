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
        RouteGroupBuilder apiGroup = app.MapGroup("api");
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

    public static async Task<Results<Ok<JoinGameResponse>, BadRequest<string>, NotFound<string>>> JoinGame(JoinGameRequest joinGameRequest, GamesService gamesService, IHubContext<ChessHub> hub)
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
            await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.PlayerJoined), joinGameRequest.UserName);
            return TypedResults.Ok(new JoinGameResponse(game.Id, game.GameState, playerIsWhites.Value ? game.BlackPlayer! : game.WhitePlayer!));
        }

        return TypedResults.NotFound("Game not found.");
    }

    public static async Task<Results<NoContent, BadRequest<string>, NotFound<string>>> SubmitMove(SubmitMoveRequest moveRequest, GamesService gamesService, IHubContext<ChessHub> hub, ILogger<AIPlayerModel> logger)
    {
        if (!gamesService.TryGetGame(moveRequest.GameId, out GameModel? game) || game is null)
        {
            return TypedResults.NotFound("Game not found.");
        }
        PlayerModel? currentPlayer = game.GameState.IsWhitePlayerTurn ? game.WhitePlayer : game.BlackPlayer;
        if (currentPlayer?.Name != moveRequest.UserName)
        {
            return TypedResults.BadRequest("It's not your turn.");
        }
        if (string.IsNullOrEmpty(moveRequest.Move))
        {
            return TypedResults.BadRequest("Move cannot be empty.");
        }
        PlayerModel? otherPlayer = game.GameState.IsWhitePlayerTurn ? game.BlackPlayer : game.WhitePlayer;
        if (otherPlayer is null)
        {
            return TypedResults.BadRequest("There is no other player yet.");
        }

        MoveError? moveError = game.TryMakeMove(moveRequest.Move);
        if (moveError.HasValue)
        {
            return TypedResults.BadRequest(moveError.Value.ToString());
        }
        PlayerModel? winner = game.CheckForWinner(currentPlayer.IsWhite);
        if (winner is not null)
        {
            gamesService.DeleteGame(game.Id);
            await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.Name);
        }
        if (otherPlayer is AIPlayerModel ai)
        {
            logger.LogInformation("Given moves to AI are {Moves}", game.GameState.Moves);
            Result<string, MoveError> aiMoveResult = await ai.Move(game);

            return await aiMoveResult.MatchAsync<Results<NoContent, BadRequest<string>, NotFound<string>>>(
                async x =>
                {
                    logger.LogInformation("AI returned move {AiMove}", x);
                    await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.MoveReceived), x);
                    PlayerModel? winner = game.CheckForWinner(otherPlayer.IsWhite);
                    if (winner is not null)
                    {
                        gamesService.DeleteGame(game.Id);
                        await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.Name);
                    }
                    return TypedResults.NoContent();
                },
                async e =>
                {
                    logger.LogError("AI move error: {Error}", e);
                    return await Task.FromResult(TypedResults.BadRequest(e.ToString()));
                }
            );
        }
        else
        {
            await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.MoveReceived), moveRequest.Move);
            if (winner is not null)
            {
                await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.Name);
            }
            return TypedResults.NoContent();
        }
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
