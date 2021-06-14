using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessChampionWebUI.Data
{
    public class ChessAIEngine
    {
        private readonly int difficultyLevel;

        public ChessAIEngine(int difficultyLevel)
        {
            this.difficultyLevel = difficultyLevel;
        }
        
        public async Task<string> GetNextMove(string moves, ushort calculationTimeMS)
        {
            ProcessStartInfo startInfo = new("stockfish_13_win_x64.exe")
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
            else
            {
                WriteMessage(process.StandardInput, $"setoption name Skill Level value {difficultyLevel}");
                string moveCommand = "position startpos moves" + moves;
                WriteMessage(process.StandardInput, moveCommand);
                WriteMessage(process.StandardInput, "go");
                await Task.Delay(calculationTimeMS);
                WriteMessage(process.StandardInput, "stop");
                string response = await ReadResponse(process.StandardOutput);
                string compMove = ParseBestMove(response);
                process.Kill();
                return compMove;
            }
        }

        private static void WriteMessage(StreamWriter streamReader, string message)
        {
            streamReader.WriteLine(message);
            streamReader.Flush();
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
    }
}
