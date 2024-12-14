using System.Diagnostics;

namespace OcpCore.Engine.PerfTest.Testers;

public static class Stockfish
{
    public static void Test(int depth, string fen)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "stockfish",
            RedirectStandardOutput = true,
            RedirectStandardInput = true
        };

        process.Start();

        var perft = GetPerft(process, fen, depth);
        
        Console.WriteLine($"{perft[0].Move}: {perft[0].Count}");
        
        process.WaitForExit();
    }

    private static List<(string Move, long Count)> GetPerft(Process stockfish, string fen, int depth)
    {
        stockfish.StandardInput.WriteLine($"position fen {fen}");
        
        stockfish.StandardInput.WriteLine($"go perft {depth}");

        var perft = new List<(string Move, long Count)>();
        
        while (true)
        {
            var line = stockfish.StandardOutput.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(line) || line.StartsWith("Stockfish", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (line.StartsWith("Nodes", StringComparison.InvariantCultureIgnoreCase))
            {
                return perft;
            }
            
            var parts = line.Split(':', StringSplitOptions.TrimEntries);

            perft.Add((parts[0], long.Parse(parts[1])));
        }
    }
}