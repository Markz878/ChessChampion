using ChessChampionWebUI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Data
{
    public class ChessAIEngine : IDisposable
    {
        private readonly Process process;

        public ChessAIEngine()
        {
            ProcessStartInfo startInfo = new("stockfish_13_win_x64.exe")
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
        }

        public async Task SetParameters(int difficultyLevel)
        {
            await WriteMessage(process.StandardInput, $"setoption name Skill Level value {difficultyLevel}");
        }

        public async Task<string> GetNextMove(GameStateModel gameState, ushort calculationTimeMS, ILogger logger)
        {
            string compMove = "";
            int retries = 0;
            while (string.IsNullOrEmpty(compMove))
            {
                logger.LogInformation("Given state to AI is {0}", gameState.Moves);
                await WriteMessage(process.StandardInput, "position startpos moves" + gameState.Moves);
                await WriteMessage(process.StandardInput, "go");
                await Task.Delay(calculationTimeMS);
                await WriteMessage(process.StandardInput, "stop");
                await Task.Delay(500);
                string response = await ReadResponse(process.StandardOutput);
                compMove = ParseBestMove(response);
                logger.LogInformation("AI returned move {0} when state was {1}", compMove, gameState.Moves);
                if (!string.IsNullOrEmpty(compMove) && gameState[compMove[..2]].IsEmpty)
                {
                    logger.LogError("AI tried to move empty square {0}, moves were {1}", compMove, gameState.Moves);
                    compMove = null;
                }
                retries++;
                if (retries > 5)
                {
                    throw new ArgumentException("Could not find move in the given response: " + response);
                }
            }
            return compMove;
        }

        private static async Task WriteMessage(StreamWriter streamReader, string message)
        {
            await streamReader.WriteLineAsync(message);
        }

        private static string ParseBestMove(string response)
        {
            Match m = Regex.Match(response, @"bestmove (?<move>[a-h]\d[a-h]\d\w?)", RegexOptions.RightToLeft);
            if (m.Success)
            {
                return m.Groups["move"].Value;
            }
            return string.Empty;
        }

        private static async Task<string> ReadResponse(StreamReader streamReader)
        {
            char[] buffer = new char[4096 * 2];
            int x = await streamReader.ReadAsync(buffer, 0, buffer.Length);
            string result = new(buffer);
            return result;
        }

        public void Dispose()
        {
            process.Dispose();
        }
    }
}
