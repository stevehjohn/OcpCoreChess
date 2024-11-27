using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.Pieces;

namespace OcpCore.Engine.PerfTest;

[ExcludeFromCodeCoverage]
public static class EntryPoint
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
        2_439_530_234_167
    ];

    public static void Main(string[] arguments)
    {
        var depth = 6;
        
        if (arguments.Length > 0)
        {
            if (! int.TryParse(arguments[0], out depth))
            {
                depth = 6;
            }
        }

        Console.WriteLine();

        for (var maxDepth = 1; maxDepth <= depth; maxDepth++)
        {
            using var core = new Core(Colour.White);

            Console.WriteLine($"  {DateTime.Now:HH:mm:ss} Starting depth {maxDepth}");

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

                    var timeRemaining = TimeSpan.FromSeconds(remaining * averagePerSecond);

                    var etr = $"{(timeRemaining.Days > 0 ? $"{timeRemaining.Days:N0}d " : string.Empty)}{timeRemaining.Hours,2:00}:{timeRemaining.Minutes,2:00}.{timeRemaining.Seconds % 60,2:00}";
                    
                    Console.Write($"  {DateTime.Now:HH:mm:ss} {percent:N2}% {depthCount:N0} / {ExpectedCombinations[maxDepth - 1]:N0} ETR: {etr}");

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

            Console.Write($"  {(pass ? "✓ PASS" : "  FAIL")}  Depth: {depth,2}  Combinations: {count,15:N0}  Expected: {expected,15:N0}");

            if (! pass)
            {
                var delta = count - expected;

                Console.Write($"  Delta: {(delta > 0 ? ">" : "<")}{delta,13:N0}");
            }
            
            Console.WriteLine();
        }

        Console.WriteLine();

        Console.WriteLine($"  {maxDepth} depth{(maxDepth > 1 ? "s" : string.Empty)} explored in {(stopwatch.Elapsed.Hours > 0 ? $"{stopwatch.Elapsed.Hours}h " : string.Empty)}{stopwatch.Elapsed.Minutes}m {stopwatch.Elapsed.Seconds:N0}s {stopwatch.Elapsed.Milliseconds}ms");

        Console.WriteLine();
    }
}