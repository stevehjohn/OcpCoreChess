using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.General;

namespace OcpCore.Engine.PerfTest.Testers;

[ExcludeFromCodeCoverage]
public static class Basic
{
        private static readonly List<long> ExpectedCombinations =
    [
        20,
        400,
        8_902,
        197_281,
        4_865_609,
        119_060_324,
        3_195_901_860,
        84_998_978_956,
        2_439_530_234_167,
        69_352_859_712_417
    ];
    
    private static readonly Dictionary<(int Ply, MoveOutcome Outcome), long> ExpectedOutcomes = new()
    {
        { (1, MoveOutcome.Capture), 0 },
        { (1, MoveOutcome.EnPassant), 0 },
        { (1, MoveOutcome.Castle), 0 },
        { (1, MoveOutcome.Promotion), 0 },
        { (1, MoveOutcome.Check), 0 },
        { (1, MoveOutcome.CheckMate), 0 },
        
        { (2, MoveOutcome.Capture), 0 },
        { (2, MoveOutcome.EnPassant), 0 },
        { (2, MoveOutcome.Castle), 0 },
        { (2, MoveOutcome.Promotion), 0 },
        { (2, MoveOutcome.Check), 0 },
        { (2, MoveOutcome.CheckMate), 0 },
        
        { (3, MoveOutcome.Capture), 34 },
        { (3, MoveOutcome.EnPassant), 0 },
        { (3, MoveOutcome.Castle), 0 },
        { (3, MoveOutcome.Promotion), 0 },
        { (3, MoveOutcome.Check), 12 },
        { (3, MoveOutcome.CheckMate), 0 },
        
        { (4, MoveOutcome.Capture), 1_576 },
        { (4, MoveOutcome.EnPassant), 0 },
        { (4, MoveOutcome.Castle), 0 },
        { (4, MoveOutcome.Promotion), 0 },
        { (4, MoveOutcome.Check), 469 },
        { (4, MoveOutcome.CheckMate), 8 },
        
        { (5, MoveOutcome.Capture), 82_719 },
        { (5, MoveOutcome.EnPassant), 258 },
        { (5, MoveOutcome.Castle), 0 },
        { (5, MoveOutcome.Promotion), 0 },
        { (5, MoveOutcome.Check), 27_351 },
        { (5, MoveOutcome.CheckMate), 347 },
        
        { (6, MoveOutcome.Capture), 2_812_008 },
        { (6, MoveOutcome.EnPassant), 5_248 },
        { (6, MoveOutcome.Castle), 0 },
        { (6, MoveOutcome.Promotion), 0 },
        { (6, MoveOutcome.Check), 809_099 },
        { (6, MoveOutcome.CheckMate), 10_828 },
        
        { (7, MoveOutcome.Capture), 108_329_926 },
        { (7, MoveOutcome.EnPassant), 319_617 },
        { (7, MoveOutcome.Castle), 883_453 },
        { (7, MoveOutcome.Promotion), 0 },
        { (7, MoveOutcome.Check), 33_103_848 },
        { (7, MoveOutcome.CheckMate), 0435_767 },
        
        { (8, MoveOutcome.Capture), 3_523_740_106 },
        { (8, MoveOutcome.EnPassant), 7_187_977 },
        { (8, MoveOutcome.Castle), 23_605_205 },
        { (8, MoveOutcome.Promotion), 0 },
        { (8, MoveOutcome.Check), 968_981_593 },
        { (8, MoveOutcome.CheckMate), 9_852_036 },
        
        { (9, MoveOutcome.Capture), 125_208_536_153 },
        { (9, MoveOutcome.EnPassant), 319_496_827 },
        { (9, MoveOutcome.Castle), 1_784_356_000 },
        { (9, MoveOutcome.Promotion), 17_334_376 },
        { (9, MoveOutcome.Check), 36_095_901_903 },
        { (9, MoveOutcome.CheckMate), 400_191_963 }
    };

    public static void Test(int depth)
    {
        Console.WriteLine();

        for (var maxDepth = 1; maxDepth <= depth; maxDepth++)
        {
            using var core = new Core(Colour.White);
            
            Console.WriteLine($"  Created engine {Core.Name} by {Core.Author}");

            Console.WriteLine();

            Console.WriteLine($"  {DateTime.Now:HH:mm:ss}: Starting depth {maxDepth}");

            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();
            
            var exception = false;

            // ReSharper disable once AccessToModifiedClosure
            // ReSharper disable once AccessToDisposedClosure
            core.GetMove(maxDepth, () => PlyComplete(core, maxDepth, stopwatch))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        exception = true;

                        Console.WriteLine(task.Exception);
                    }
                });

            var y = Console.CursorLeft;
            
            while (core.IsBusy && ! exception)
            {
                Thread.Sleep(1000);

                if (core.IsBusy)
                {
                    var depthCount = core.GetDepthCount(maxDepth);
                    
                    var percent = (float) depthCount / ExpectedCombinations[maxDepth - 1] * 100;

                    var averagePerSecond = stopwatch.Elapsed.TotalSeconds / depthCount;

                    var remaining = ExpectedCombinations[maxDepth - 1] - depthCount;

                    try
                    {
                        var timeRemaining = TimeSpan.FromSeconds(remaining * averagePerSecond);

                        var etr = $"{(timeRemaining.Days > 0 ? $"{timeRemaining.Days:N0}d " : string.Empty)}{timeRemaining.Hours,2:00}:{timeRemaining.Minutes,2:00}.{timeRemaining.Seconds % 60,2:00}";

                        Console.Write($"  {DateTime.Now:HH:mm:ss}: {depthCount:N0} / {ExpectedCombinations[maxDepth - 1]:N0} ({percent:N2}%) Queue: {core.QueueSize:N0} ETR: {etr}          ");
                    }
                    catch
                    {
                        Console.Write($"  {DateTime.Now:HH:mm:ss}: {depthCount:N0} / {ExpectedCombinations[maxDepth - 1]:N0} ({percent:N2}%) Queue: {core.QueueSize:N0} ETR: ∞     ");
                    }

                    Console.CursorLeft = y;
                }
            }
            
            if (exception)
            {
                break;
            }
        }
    }

    private static void PlyComplete(Core core, int maxDepth, Stopwatch stopwatch)
    {
        stopwatch.Stop();

        for (var depth = 1; depth <= maxDepth; depth++)
        {
            var count = core.GetDepthCount(depth);

            var expected = ExpectedCombinations[depth - 1];

            var pass = count == expected;

            Console.Write($"  {(pass ? "✓ PASS" : "  FAIL")}  Depth: {depth,2}  Combinations: {count,18:N0}  Expected: {expected,18:N0}");

            if (! pass)
            {
                var delta = count - expected;

                Console.Write($"  Delta: {(delta > 0 ? ">" : "<")}{delta,13:N0}");
            }

            if (depth >= 10)
            {
                continue;
            }

            Console.WriteLine();

            Console.Write($"      Capture:  {core.GetOutcomeCount(depth, MoveOutcome.Capture),15:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.Capture)] == core.GetOutcomeCount(depth, MoveOutcome.Capture) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.Capture)] == core.GetOutcomeCount(depth, MoveOutcome.Capture))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.Capture) - ExpectedOutcomes[(depth, MoveOutcome.Capture)],13:N0}");
            }

            Console.Write($"      En Passant: {core.GetOutcomeCount(depth, MoveOutcome.EnPassant),13:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.EnPassant)] == core.GetOutcomeCount(depth, MoveOutcome.EnPassant) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.EnPassant)] == core.GetOutcomeCount(depth, MoveOutcome.EnPassant))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.EnPassant) - ExpectedOutcomes[(depth, MoveOutcome.EnPassant)],13:N0}");
            }

            Console.Write($"      Castle:     {core.GetOutcomeCount(depth, MoveOutcome.Castle),13:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.Castle)] == core.GetOutcomeCount(depth, MoveOutcome.Castle) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.Castle)] == core.GetOutcomeCount(depth, MoveOutcome.Castle))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.Castle) - ExpectedOutcomes[(depth, MoveOutcome.Castle)],13:N0}");
            }

            Console.Write($"      Promotion:  {core.GetOutcomeCount(depth, MoveOutcome.Promotion),13:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.Promotion)] == core.GetOutcomeCount(depth, MoveOutcome.Promotion) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.Promotion)] == core.GetOutcomeCount(depth, MoveOutcome.Promotion))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.Promotion) - ExpectedOutcomes[(depth, MoveOutcome.Promotion)],13:N0}");
            }

            Console.Write($"      Check:     {core.GetOutcomeCount(depth, MoveOutcome.Check),14:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.Check)] == core.GetOutcomeCount(depth, MoveOutcome.Check) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.Check)] == core.GetOutcomeCount(depth, MoveOutcome.Check))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.Check) - ExpectedOutcomes[(depth, MoveOutcome.Check)],13:N0}");
            }

            Console.Write($"      Check Mate: {core.GetOutcomeCount(depth, MoveOutcome.CheckMate),13:N0}");
            Console.Write($" {(ExpectedOutcomes[(depth, MoveOutcome.CheckMate)] == core.GetOutcomeCount(depth, MoveOutcome.CheckMate) ? "✓" : string.Empty)}");
            if (ExpectedOutcomes[(depth, MoveOutcome.CheckMate)] == core.GetOutcomeCount(depth, MoveOutcome.CheckMate))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine($"  Delta: {core.GetOutcomeCount(depth, MoveOutcome.CheckMate) - ExpectedOutcomes[(depth, MoveOutcome.CheckMate)],13:N0}");
            }

            var score = core.BestScore;

            Console.WriteLine(score is int.MinValue or int.MaxValue
                ? $"      Best Score:            {(score == int.MinValue ? "-" : " ")}∞"
                : $"      Best Score: {score,13:N0}");
        }

        Console.WriteLine();
        
        Console.WriteLine($"  {maxDepth} depth{(maxDepth > 1 ? "s" : string.Empty)} explored in {(stopwatch.Elapsed.Days > 0 ? $"{stopwatch.Elapsed.Days:N0}d " : string.Empty)}{(stopwatch.Elapsed.Hours > 0 ? $"{stopwatch.Elapsed.Hours}h " : string.Empty)}{stopwatch.Elapsed.Minutes}m {stopwatch.Elapsed.Seconds:N0}s {stopwatch.Elapsed.Milliseconds}ms");

        Console.WriteLine();
    }
}