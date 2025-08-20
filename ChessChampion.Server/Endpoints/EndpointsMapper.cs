using ChessChampion.Server.Installers;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ChessChampion.Server.Endpoints;

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

    public static async Task<Results<Ok<CreateGameResponse>, BadRequest<string>>> CreateGame(CreateGameRequest createGameRequest, IChessService chessService)
    {
        Result<CreateGameResponse, BaseError> response = await chessService.CreateGame(createGameRequest);
        return response.Match<Results<Ok<CreateGameResponse>, BadRequest<string>>>(
            ok => TypedResults.Ok(ok),
            error => TypedResults.BadRequest(error.Error)
        );
    }

    public static async Task<Results<Ok<JoinGameResponse>, BadRequest<string>>> JoinGame(JoinGameRequest joinGameRequest, IChessService chessService)
    {
        Result<JoinGameResponse, BaseError> response = await chessService.JoinGame(joinGameRequest);
        return response.Match<Results<Ok<JoinGameResponse>, BadRequest<string>>>(
            ok => TypedResults.Ok(ok),
            error => TypedResults.BadRequest(error.Error)
        );
    }

    public static async Task<Results<NoContent, BadRequest<string>>> SubmitMove(SubmitMoveRequest moveRequest, IChessService chessService)
    {
        BaseError? error = await chessService.SubmitMove(moveRequest);
        return error.HasValue ? TypedResults.BadRequest(error.Value.Error) : TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, BadRequest<string>>> LeaveGame(LeaveGameRequest leaveRequest, IChessService chessService)
    {
        BaseError? error = await chessService.LeaveGame(leaveRequest);
        return error.HasValue ? TypedResults.BadRequest(error.Value.Error) : TypedResults.NoContent();
    }
}
