using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessChampionCLITest
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var startInfo = new ProcessStartInfo(@"C:\Users\Mark\Downloads\stockfish_13_win_x64\stockfish_13_win_x64\stockfish_13_win_x64.exe");
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
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
            Process process = new()
            {
                StartInfo = startInfo
            };
            process.Start();

            StreamWriter streamWriter = process.StandardInput;
            StreamReader streamReader = process.StandardOutput;

            StringBuilder moveBuilder = new("position startpos moves");

            while (true)
            {
                Console.WriteLine("Give next move:");
                string move = Console.ReadLine();
                moveBuilder.Append(' ').Append(move);

                await WriteMessage(streamWriter, moveBuilder.ToString());
                await WriteMessage(streamWriter, "go");

                await Task.Delay(5000);

                await WriteMessage(streamWriter, "stop");

                string response = await ReadResponse(streamReader);

                string compMove = ReadMove(response);

                moveBuilder.Append(' ').Append(compMove);
            }
        }

        private static async Task WriteMessage(StreamWriter streamReader, string message)
        {
            await streamReader.WriteLineAsync(message);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        private static string ReadMove(string response)
        {
            Match m = Regex.Match(response, @"bestmove (?<move>[a-g]\d[a-g]\d) ", RegexOptions.RightToLeft);
            if (m.Success)
            {
                Console.WriteLine("Next move is " + m.Groups["move"].Value);
                return m.Groups["move"].Value;
            }
            throw new ArgumentException("Could not find move in the given response");
        }

        private static async Task<string> ReadResponse(StreamReader streamReader)
        {
            Console.ForegroundColor = ConsoleColor.White;
            char[] buffer = new char[4096];
            int x = await streamReader.ReadAsync(buffer);
            string response = new(buffer);
            Console.WriteLine(response);
            return response;
        }
    }
}
