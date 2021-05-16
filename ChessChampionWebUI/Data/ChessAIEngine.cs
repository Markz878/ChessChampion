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
            ProcessStartInfo startInfo = new(@"C:\Users\Mark\Downloads\stockfish_13_win_x64\stockfish_13_win_x64\stockfish_13_win_x64.exe")
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
            process.Start();
        }

        public async Task<string> GetNextMove(string playerMove, ushort calculationTimeMS)
        {
            if (!string.IsNullOrEmpty(playerMove))
            {
                moves.Append(' ').Append(playerMove);
            }
            await WriteMessage(process.StandardInput, moves.ToString());
            await WriteMessage(process.StandardInput, "go");
            await Task.Delay(calculationTimeMS);

            await WriteMessage(process.StandardInput, "stop");
            string response = await ReadResponse(process.StandardOutput);

            string compMove = ReadMove(response);

            moves.Append(' ').Append(compMove);
            return compMove;
        }

        private static async Task WriteMessage(StreamWriter streamReader, string message)
        {
            await streamReader.WriteLineAsync(message);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        private static string ReadMove(string response)
        {
            Match m = Regex.Match(response, @"bestmove (?<move>[a-h]\d[a-h]\d\w?) ", RegexOptions.RightToLeft);
            if (m.Success)
            {
                Console.WriteLine("Next move is " + m.Groups["move"].Value);
                return m.Groups["move"].Value;
            }
            throw new ArgumentException("Could not find move in the given response: " + response);
        }

        private static async Task<string> ReadResponse(StreamReader streamReader)
        {
            Console.ForegroundColor = ConsoleColor.White;
            char[] buffer = new char[4096 * 10];
            int x = await streamReader.ReadAsync(buffer);
            string response = new(buffer);
            Console.WriteLine(response);
            return response;
        }

        public void Dispose()
        {
            process.Dispose();
        }
    }
}
