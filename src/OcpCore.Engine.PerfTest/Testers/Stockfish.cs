using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.General;

namespace OcpCore.Engine.PerfTest.Testers;

[ExcludeFromCodeCoverage]
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

        var moves = new List<string>();

        do
        {
            PerformRound(process, fen, depth, moves);

            Console.Write("  Move? ");

            var move = Console.ReadLine() ?? string.Empty;

            if (move.StartsWith("q", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine();
                
                break;
            }

            moves.Add(move);

            depth--;

        } while (depth > 0);

        process.Kill();
    }

    private static void PerformRound(Process process, string fen, int depth, List<string> moves)
    {
        Console.WriteLine();
        
        var stockfishPerft = GetStockfishPerft(process, fen, depth, moves);

        var ocpPerft = GetOcpPerft(fen, depth, moves);

        var keys = stockfishPerft.Select(i => i.Move).Union(ocpPerft.Select(i => i.Move)).Order();

        Console.WriteLine("");
        
        Console.WriteLine("            StockFish         OcpCore            Delta");

        var colour = Console.ForegroundColor;
        
        foreach (var key in keys)
        {
            Console.Write($"  {key}: ");

            var stockfish = stockfishPerft.SingleOrDefault(i => i.Move == key);

            var ocp = ocpPerft.SingleOrDefault(i => i.Move == key);

            var delta = ocp.Count - stockfish.Count;

            if (delta != 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }

            Console.Write(stockfish != default ? $"{stockfish.Count,13:N0}    " : "                 ");

            Console.Write(ocp != default ? $"{ocp.Count,13:N0}   " : "                 ");

            Console.Write($"{delta,13:N0}");

            Console.ForegroundColor = colour;

            Console.WriteLine();
        }
        
        Console.WriteLine();
    }

    private static List<(string Move, long Count)> GetOcpPerft(string fen, int depth, List<string> moves)
    {
        var core = new Core(Colour.White, fen, true);

        foreach (var move in moves)
        {
            core.MakeMove(move);
        }
        
        core.GetMove(depth, () =>
        {
        });

        while (core.IsBusy)
        {
            Thread.Sleep(100);
            
            Console.Write($"  OcpCore:   {core.GetDepthCount(depth),13:N0}");

            Console.CursorLeft = 0;
        }
        
        Console.WriteLine();
        
        Console.WriteLine($"  {core.BestMove}");

        return core.PerftData.Select(i => (i.Key, i.Value)).ToList();
    }

    private static List<(string Move, long Count)> GetStockfishPerft(Process stockfish, string fen, int depth, List<string> moves)
    {
        stockfish.StandardInput.Write($"position fen {fen}");

        if (moves.Count > 0)
        {
            stockfish.StandardInput.Write($" moves {string.Join(' ', moves)}");
        }
        
        stockfish.StandardInput.WriteLine();

        stockfish.StandardInput.WriteLine($"go perft {depth}");

        var perft = new List<(string Move, long Count)>();

        var sum = 0L;
        
        while (true)
        {
            Console.Write($"  Stockfish: {sum,13:N0}");

            Console.CursorLeft = 0;

            var line = stockfish.StandardOutput.ReadLine() ?? string.Empty;

            if (string.IsNullOrEmpty(line) || line.StartsWith("Stockfish", StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (line.StartsWith("Nodes", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine();
                
                return perft;
            }
            
            var parts = line.Split(':', StringSplitOptions.TrimEntries);

            perft.Add((parts[0], long.Parse(parts[1])));

            sum += long.Parse(parts[1]);
        }
    }
}