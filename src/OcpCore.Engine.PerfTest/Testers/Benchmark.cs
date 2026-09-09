using System.Diagnostics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;

namespace OcpCore.Engine.PerfTest.Testers;

public static class Benchmark
{
    public static void Test()
    {
        var game = new Game();

        game.ParseFen(Constants.InitialBoardFen);

        foreach (var parallel in new[] { false, true })
        {
            using var warmup = new Coordinator(Colour.White, new PerfTestCollector(), parallel ? 0 : int.MaxValue);

            warmup.StartProcessing(game, 3);

            for (var iteration = 1; iteration <= 3; iteration++)
            {
                using var coordinator = new Coordinator(Colour.White, new PerfTestCollector(), parallel ? 0 : int.MaxValue);

                var allocated = GC.GetTotalAllocatedBytes(true);

                var stopwatch = Stopwatch.StartNew();

                coordinator.StartProcessing(game, 5);

                stopwatch.Stop();

                allocated = GC.GetTotalAllocatedBytes(true) - allocated;

                var nodes = coordinator.GetDepthCount(5);

                if (nodes != 4_865_609)
                {
                    throw new InvalidOperationException($"Unexpected node count: {nodes}");
                }

                Console.WriteLine($"Parallel: {parallel}, run: {iteration}, nodes: {nodes}, milliseconds: {stopwatch.Elapsed.TotalMilliseconds:F1}, allocated bytes: {allocated}");
            }
        }
    }
}
