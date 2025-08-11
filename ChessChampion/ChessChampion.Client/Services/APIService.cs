using ChessChampion.Shared.Models;
using System.Net.Http.Json;

namespace ChessChampion.Client.Services;

public sealed class APIService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient httpClient = httpClientFactory.CreateClient("api");
    public async Task<Result<CreateGameResponse, string>> CreateGame(CreateGameRequest createGameRequest)
    {
        string x = httpClient.BaseAddress?.ToString() ?? string.Empty;
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/create", createGameRequest);
        if (response.IsSuccessStatusCode)
        {
            CreateGameResponse? gameStateResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
            if (gameStateResponse is not null)
            {
                return gameStateResponse;
            }
        }
        string errorMessage = await response.Content.ReadAsStringAsync();
        return errorMessage;
    }

    public async Task<Result<JoinGameResponse, string>> JoinGame(JoinGameRequest joinGameRequest)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/join", joinGameRequest);
        if (response.IsSuccessStatusCode)
        {
            JoinGameResponse? gameResponse = await response.Content.ReadFromJsonAsync<JoinGameResponse>();
            if (gameResponse is not null)
            {
                return gameResponse;
            }
        }
        string errorMessage = await response.Content.ReadAsStringAsync();
        return errorMessage;
    }

    public async Task<string?> SubmitMove(SubmitMoveRequest moveRequest)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/move", moveRequest);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        string errorMessage = await response.Content.ReadAsStringAsync();
        return errorMessage;
    }

    public async Task<string?> LeaveGame(LeaveGameRequest leaveGameRequest)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/leave", leaveGameRequest);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        string errorMessage = await response.Content.ReadAsStringAsync();
        return errorMessage;
    }
}
