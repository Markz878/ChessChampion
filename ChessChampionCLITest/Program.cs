using System;
using System.Diagnostics;
using System.IO;
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

            Process process = new()
            {
                StartInfo = startInfo
            };
            process.Start();

            StreamWriter streamWriter = process.StandardInput;
            StreamReader streamReader = process.StandardOutput;

            string moves = "position startpos moves e2e4";
            await WriteMessage(streamWriter, moves);

            await WriteMessage(streamWriter, "go");

            await Task.Delay(2000);

            await WriteMessage(streamWriter, "stop");

            await ReadResponse(streamReader);

            Console.ReadLine();
        }

        private static async Task WriteMessage(StreamWriter streamReader, string message)
        {
            await streamReader.WriteLineAsync(message);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        private static async Task ReadResponse(StreamReader streamReader)
        {
            Console.ForegroundColor = ConsoleColor.White;
            char[] buffer = new char[4096];
            int x = await streamReader.ReadAsync(buffer);
            string response = new string(buffer);
            Console.WriteLine(response);
        }
    }
}
