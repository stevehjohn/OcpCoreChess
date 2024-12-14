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

        var keys = stockfishPerft.Select(i => i.Move).Union(ocpPerft.Select(i => i.Move)).Order();

        Console.WriteLine("");
        
        Console.WriteLine("          StockFish         OcpCore            Delta");

        var colour = Console.ForegroundColor;
        
        foreach (var key in keys)
        {
            Console.Write($"{key}: ");

            var stockfish = stockfishPerft.SingleOrDefault(i => i.Move == key);

            var ocp = ocpPerft.SingleOrDefault(i => i.Move == key);

            var delta = ocp.Count - stockfish.Count;

            if (delta != 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }

            if (stockfish != default)
            {
                Console.Write($"{stockfish.Count,13:N0}    ");
            }
            else
            {
                Console.Write("                 ");
            }

            if (ocp != default)
            {
                Console.Write($"{ocp.Count,13:N0}   ");
            }
            else
            {
                Console.Write("                 ");
            }
            
            Console.Write($"{delta,13:N0}");

            Console.ForegroundColor = colour;

            Console.WriteLine();
        }
        
        Console.WriteLine();
        
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