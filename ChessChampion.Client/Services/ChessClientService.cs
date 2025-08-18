using ChessChampion.Client.Models;
using ChessChampion.Shared.Models;
using ChessChampion.Shared.Services;
using System.Net.Http.Json;

namespace ChessChampion.Client.Services;

public class ChessClientService(IHttpClientFactory httpClientFactory, MainViewModel viewModel, IHubConnectionService hub) : IChessService
{
    public async Task CreateGame(CreateGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/create", request);
        if (response.IsSuccessStatusCode)
        {
            CreateGameResponse? createGameResponse = await response.Content.ReadFromJsonAsync<CreateGameResponse>();
            ArgumentNullException.ThrowIfNull(createGameResponse);
            viewModel.GameId = createGameResponse.Id;
            viewModel.Winner = null;
            viewModel.GameCode = createGameResponse.GameCode;
            viewModel.GameState = createGameResponse.GameState;
            viewModel.OtherPlayer = request.VersusAI ? new PlayerModel("Stockfish", !viewModel.Player.IsWhite) : null;
            viewModel.StatusMessage = request.VersusAI ?
                viewModel.Player.IsWhite ? MainViewModel.PlayerTurnText : MainViewModel.OtherPlayerTurnText :
                "Waiting for the other player...";
            await hub.JoinGame(createGameResponse.Id);
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            viewModel.StatusMessage = errorMessage;
        }
    }

    public async Task JoinGame(JoinGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/join", request);
        if (response.IsSuccessStatusCode)
        {
            JoinGameResponse? joinGameResponse = await response.Content.ReadFromJsonAsync<JoinGameResponse>();
            ArgumentNullException.ThrowIfNull(joinGameResponse);
            viewModel.GameId = joinGameResponse.Id;
            viewModel.GameState = joinGameResponse.GameState;
            viewModel.Winner = null;
            viewModel.Player.IsWhite = !joinGameResponse.Player1.IsWhite;
            viewModel.OtherPlayer = joinGameResponse.Player1;
            viewModel.StatusMessage = viewModel.Player.IsWhite ? MainViewModel.PlayerTurnText : MainViewModel.OtherPlayerTurnText;
            await hub.JoinGame(joinGameResponse.Id);
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            viewModel.StatusMessage = errorMessage;
        }
    }

    public async Task LeaveGame(LeaveGameRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/leave", request);
        viewModel.GameState = null;
        viewModel.GameId = null;
        viewModel.Winner = null;
        viewModel.OtherPlayer = null;
        viewModel.GameCode = "";
        if (response.IsSuccessStatusCode)
        {
            viewModel.StatusMessage = "You have left the game";
            await hub.LeaveGame(request.GameId);
        }
        else
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            viewModel.StatusMessage = errorMessage;
        }
    }

    public async Task<BaseError?> SubmitMove(SubmitMoveRequest request)
    {
        HttpClient httpClient = httpClientFactory.CreateClient("api");
        httpClient.DefaultRequestHeaders.Add("connectionId", hub.ConnectionId());
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("api/move", request);
        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync();
            viewModel.StatusMessage = errorMessage;
            return new BaseError(errorMessage);
        }
        return null;
    }
}
