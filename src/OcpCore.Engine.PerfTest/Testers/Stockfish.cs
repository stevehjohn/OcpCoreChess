using System.Diagnostics;
using OcpCore.Engine.General;

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

        var stockfishPerft = GetStockfishPerft(process, fen, depth);

        var ocpPerft = GetOcpPerft(fen, depth);

        foreach (var item in ocpPerft)
        {
            Console.WriteLine($"{item.Move}: {item.Count}");
        }
        
        process.Kill();
    }

    private static List<(string Move, long Count)> GetOcpPerft(string fen, int depth)
    {
        var core = new Core(Colour.White, fen, true);

        core.GetMove(depth);

        return core.PerftData.Select(i => (i.Key, i.Value)).ToList();
    }

    private static List<(string Move, long Count)> GetStockfishPerft(Process stockfish, string fen, int depth)
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