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
        private readonly StringBuilder moves = new("position startpos moves");
        private readonly Process process;
        public ChessAIEngine()
        {
            ProcessStartInfo startInfo = new("stockfish_13_win_x64.exe")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            string[][] startingSquare = new string[][]
            {
                new[] { "♜", "♞", "♝", "♛", "♚", "♝", "♞", "♜" },
                new[] {"♟︎", "♟︎", "♟︎", "♟︎", "♟︎", "♟︎", "♟︎", "♟︎" },
                new[] {"", "", "", "", "", "", "", "" },
                new[] {"", "", "", "", "", "", "", "" },
                new[] {"", "", "", "", "", "", "", "" },
                new[] {"", "", "", "", "", "", "", "" },
                new[] {"♙", "♙", "♙", "♙", "♙", "♙", "♙", "♙" },
                new[] {"♖", "♘", "♗", "♕", "♔", "♗", "♘", "♖" }
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

        public async Task SetDifficulty(int level)
        {
            await WriteMessage(process.StandardInput, $"setoption name Skill Level value {level}");
        }

        public async Task<string> GetNextMove(string playerMove, ushort calculationTimeMS)
        {
            if (!string.IsNullOrEmpty(playerMove))
            {
                moves.Append(' ').Append(playerMove);
            }
            string compMove = "";
            int retries = 0;
            while (string.IsNullOrEmpty(compMove))
            {
                await WriteMessage(process.StandardInput, moves.ToString());
                await WriteMessage(process.StandardInput, "go");
                await Task.Delay(calculationTimeMS);
                await WriteMessage(process.StandardInput, "stop");
                string response = await ReadResponse(process.StandardOutput);
                compMove = ParseBestMove(response);
                retries++;
                if (retries > 5)
                {
                    throw new ArgumentException("Could not find move in the given response: " + response);
                }
            }
            moves.Append(' ').Append(compMove);
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
            char[] buffer = new char[4096 * 4];
            int x = await streamReader.ReadAsync(buffer, 0, buffer.Length);
            string result = new(buffer);
            if (!result.Contains("bestmove"))
            {
                await streamReader.ReadAsync(buffer, 0, buffer.Length);
            }
            return result;
        }

        public void Dispose()
        {
            process.Dispose();
        }
    }
}
