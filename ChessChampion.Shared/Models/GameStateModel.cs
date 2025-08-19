using System.Text.Json.Serialization;

namespace ChessChampion.Shared.Models;

public class GameStateModel
{
    public GameSquare[][] State { get; set; }
    public string Moves { get; set; } = "";
    public bool CanWhiteKingCastleRight { get; set; } = true;
    public bool CanWhiteKingCastleLeft { get; set; } = true;
    public bool CanBlackKingCastleRight { get; set; } = true;
    public bool CanBlackKingCastleLeft { get; set; } = true;
    public bool IsWhitePlayerTurn { get; set; } = true;

    [JsonConstructor]
    private GameStateModel() 
    {
        State = [];
    }

    public GameStateModel(GameSquare[][] initialState)
    {
        State = initialState;
    }

    public IEnumerable<GameSquare> GetSquares()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                yield return State[i][j];
            }
        }
    }

    public GameSquare[] this[int row] => State[row];

    public GameSquare this[string coordinate] => GetSquareFromCoordinates(coordinate);

    public GameSquare GetSquareFromCoordinates(string coordinate)
    {
        try
        {
            int column = char.ConvertToUtf32(coordinate, 0) - 97;
            int row = int.Parse(coordinate[1].ToString()) - 1;
            return State[row][column];
        }
        catch (Exception)
        {
            throw;
        }
    }

    public GameSquare GetPieceSquare<T>()
    {
        foreach (GameSquare square in GetSquares())
        {
            if (square.Piece is not null and T)
            {
                return square;
            }
        }
        string x = typeof(T).Name;
        throw new InvalidOperationException(x + " piece not found.");
    }
}