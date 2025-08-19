using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using System.Net.Http.Json;

namespace ChessChampion.Client.Services;

public class ChessClientService(IHttpClientFactory httpClientFactory, IHubConnectionService hub) : IChessService
{
    public async Task<Result<CreateGameResponse, BaseError>> CreateGame(CreateGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/create", request);
        if (response.IsSuccessStatusCode)
        {
            CreateGameResponse? createGameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
            ArgumentNullException.ThrowIfNull(createGameResponse);
            return createGameResponse;
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return new BaseError(errorMessage);
        }
    }

    public async Task<Result<JoinGameResponse, BaseError>> JoinGame(JoinGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/join", request);
        if (response.IsSuccessStatusCode)
        {
            JoinGameResponse? joinGameResponse = await response.Content.ReadFromJsonAsync<JoinGameResponse>();
            ArgumentNullException.ThrowIfNull(joinGameResponse);
            return joinGameResponse;
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return new BaseError(errorMessage);
        }
    }

    public async Task<BaseError?> LeaveGame(LeaveGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/leave", request);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return new BaseError(errorMessage);
        }
    }

    public async Task<BaseError?> SubmitMove(SubmitMoveRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        httpClient.DefaultRequestHeaders.Add("connectionId", hub.ConnectionId());
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/move", request);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            return new BaseError(errorMessage);
        }
    }
}
