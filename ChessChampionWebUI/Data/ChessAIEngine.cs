using System.Diagnostics;

namespace ChessChampionWebUI.Data;

public sealed partial class ChessAIEngine : IDisposable
{
    private readonly Process process;
    public ChessAIEngine(int difficultyLevel, string engineFileName)
    {
        ProcessStartInfo startInfo = new(engineFileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        process = new()
        {
            StartInfo = startInfo
        };
        if (!process.Start())
        {
            throw new FileNotFoundException("Stockfish could not be started");
        }
        WriteMessage(process.StandardInput, $"setoption name Skill Level value {difficultyLevel}");
    }

    public async Task<string> GetNextMove(string moves, ushort calculationTimeMS)
    {
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
            WriteMessage(process.StandardInput, "go");
            await Task.Delay(calculationTimeMS);
            WriteMessage(process.StandardInput, "stop");
            string response = await ReadResponse(process.StandardOutput);
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

    private static async Task<string> ReadResponse(StreamReader streamReader)
    {
        char[] buffer = new char[4096 * 2];
        await streamReader.ReadAsync(buffer, 0, buffer.Length);
        string result = new(buffer);
        return result;
    }

    public void Dispose()
    {
        process.Dispose();
    }
}
