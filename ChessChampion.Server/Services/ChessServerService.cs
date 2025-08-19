using ChessChampion.Core.Data;
using ChessChampion.Core.Models;
using ChessChampion.Server.Hubs;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChessChampion.Server.Services;

public sealed class ChessServerService(GamesRepository gamesRepository, IHubContext<ChessHub> hub, ILogger<ChessServerService> logger, IHttpContextAccessor httpContextAccessor) : IChessService
{
    public Task<Result<CreateGameResponse, BaseError>> CreateGame(CreateGameRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName))
        {
            return Task.FromResult<Result<CreateGameResponse, BaseError>>(new BaseError("User name is required."));
        }
        PlayerModel player = new(request.UserName, request.IsWhites);
        GameModel game = request.VersusAI
            ? gamesRepository.CreateGameWithAI(player, request.AISkillLevel.GetValueOrDefault(10))
            : gamesRepository.CreateGame(player);
        return Task.FromResult<Result<CreateGameResponse, BaseError>>(new CreateGameResponse(game.Id, game.Code ?? "", game.GameState));
    }

    public async Task<Result<JoinGameResponse, BaseError>> JoinGame(JoinGameRequest request)
    {
        if (string.IsNullOrEmpty(request.UserName))
        {
            return new BaseError("User name is required.");
        }
        if (string.IsNullOrEmpty(request.GameCode))
        {
            return new BaseError("Game code is required.");
        }
        if (gamesRepository.TryToJoinGame(request.GameCode, request.UserName, out bool? playerIsWhites, out GameModel? game) && playerIsWhites.HasValue && game is not null)
        {
            await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.PlayerJoined), request.UserName);
            return new JoinGameResponse(game.Id, game.GameState, playerIsWhites.Value ? game.BlackPlayer! : game.WhitePlayer!);
        }
        else
        {
            return new BaseError("Game not found.");
        }
    }

    public async Task<BaseError?> LeaveGame(LeaveGameRequest request)
    {
        if (gamesRepository.TryGetGame(request.GameId, out GameModel? game) && game is not null)
        {
            if (game.WhitePlayer?.Name == request.UserName || game.BlackPlayer?.Name == request.UserName)
            {
                await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.PlayerLeft), request.UserName);
                gamesRepository.DeleteGame(request.GameId);
                return null;
            }
            return new BaseError("You are not part of the game.");
        }
        return new BaseError("Game not found.");
    }

    public async Task<BaseError?> SubmitMove(SubmitMoveRequest request)
    {
        if (!gamesRepository.TryGetGame(request.GameId, out GameModel? game) || game is null)
        {
            return new BaseError("Game not found.");
        }
        PlayerModel? currentPlayer = game.GameState.IsWhitePlayerTurn ? game.WhitePlayer : game.BlackPlayer;
        if (currentPlayer?.Name != request.UserName)
        {
            return new BaseError("It's not your turn.");
        }
        if (string.IsNullOrEmpty(request.Move))
        {
            return new BaseError("Move cannot be empty.");
        }
        PlayerModel? otherPlayer = game.GameState.IsWhitePlayerTurn ? game.BlackPlayer : game.WhitePlayer;
        if (otherPlayer is null)
        {
            return new BaseError("There is no other player yet.");
        }
        BaseError? moveError = game.TryMakeMove(request.Move);
        if (moveError.HasValue)
        {
            return new BaseError(moveError.Value.ToString());
        }
        if (otherPlayer is not AIPlayerModel)
        {
            HttpContext? httpContext = httpContextAccessor.HttpContext;
            string? connectionId = httpContext?.Request.Headers["connectionId"];
            await hub.Clients.GroupExcept(game.Id.ToString(), connectionId ?? "").SendAsync(nameof(IChessHubNotifications.MoveReceived), request.Move);
        }
        PlayerModel? winner = game.CheckForWinner(currentPlayer.IsWhite);
        if (winner is not null)
        {
            gamesRepository.DeleteGame(game.Id);
            await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.IsWhite);
        }
        if (otherPlayer is AIPlayerModel ai)
        {
            logger.LogInformation("Given moves to AI are{Moves}", game.GameState.Moves);
            Result<string, BaseError> aiMoveResult = await ai.Move(game);

            BaseError? error = await aiMoveResult.MatchAsync(
                async move =>
                {
                    logger.LogInformation("AI returned move {AiMove}", move);
                    await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.MoveReceived), move);
                    PlayerModel? winner = game.CheckForWinner(otherPlayer.IsWhite);
                    if (winner is not null)
                    {
                        gamesRepository.DeleteGame(game.Id);
                        await hub.Clients.Group(game.Id.ToString()).SendAsync(nameof(IChessHubNotifications.GameOver), winner.IsWhite);
                    }
                    return null;
                },
                error =>
                {
                    logger.LogError("AI move error: {Error}", error);
                    return Task.FromResult<BaseError?>(new BaseError(error.ToString()));
                }
            );
            if (error is not null)
            {
                return error;
            }
        }
        return null;
    }
}
