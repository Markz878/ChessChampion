using System.Diagnostics;

namespace ChessChampionWebUI.Data;

public sealed partial class ChessAIEngine(int difficultyLevel, string engineFileName)
{
    public async Task<string> GetNextMove(string moves, ushort calculationTimeMS)
    {
        ProcessStartInfo startInfo = new(engineFileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        using Process process = new()
        {
            StartInfo = startInfo
        };
        if (!process.Start())
        {
            throw new FileNotFoundException("Stockfish could not be started");
        }
        WriteMessage(process.StandardInput, $"setoption name Skill Level value {difficultyLevel}");
        int retries = 0;
        string compMove;
        string moveCommand = "position startpos moves" + moves;
        WriteMessage(process.StandardInput, moveCommand);
        do
        {
            retries++;
            if (retries > 5)
            {
                throw new ArgumentException("Could not find move in the given response:");
            }
            WriteMessage(process.StandardInput, "go movetime " + calculationTimeMS);
            await Task.Delay(calculationTimeMS);
            WriteMessage(process.StandardInput, "stop");
            string response = ReadResponse(process.StandardOutput);
            compMove = ParseBestMove(response);
        } while (string.IsNullOrEmpty(compMove));
        return compMove;
    }

    private static void WriteMessage(StreamWriter streamWriter, string message)
    {
        streamWriter.WriteLine(message);
        streamWriter.Flush();
    }

    private static string ParseBestMove(string response)
    {
        const string marker = "bestmove ";
        int indexOfBestMove = response.LastIndexOf(marker);
        if (indexOfBestMove < 0)
        {
            return string.Empty;
        }
        int indexOfMove = indexOfBestMove + marker.Length;
        string result = response[indexOfMove..(indexOfMove + 4)];
        return result;
    }

    private static string ReadResponse(StreamReader streamReader)
    {
        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            if (line.StartsWith("bestmove"))
            {
                return line;
            }
        }
        return string.Empty;
    }
}
