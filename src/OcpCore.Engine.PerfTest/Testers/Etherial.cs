using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.General;

namespace OcpCore.Engine.PerfTest.Testers;

[ExcludeFromCodeCoverage]
public static class Etherial
{
    public static void Test()
    {
        var tests = File.ReadAllLines("Data/standard.epd");

        var stopwatch = Stopwatch.StartNew();

        var fails = new List<(int Index, string Fen)>();

        for (var i = 0; i < tests.Length; i++)
        {
            var test = tests[i];
            
            var parts = test.Split(';', StringSplitOptions.TrimEntries);

            var fen = parts[0];
            
            using var core = new Core(Colour.White, fen);

            var depth = parts[^1][1] - '0';
        
            Console.WriteLine();
            
            Console.WriteLine($"  Test: {i + 1,3}/{tests.Length,3}  Depth: {depth}  FEN: {parts[0]}");
            
            Console.WriteLine();

            var exception = false;

            var pass = false;
            
            // ReSharper disable once AccessToDisposedClosure
            core.GetMove(depth, bestMove => pass = TestComplete(core, depth, parts[1..], bestMove))
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        exception = true;

                        Console.WriteLine(task.Exception);
                    }
                });

            var expected = long.Parse(parts[^1][3..]);

            var y = Console.CursorLeft;

            while (core.IsBusy && !exception)
            {
                Thread.Sleep(1000);
                
                if (core.IsBusy)
                {
                    var depthCount = core.GetDepthCount(depth);

                    var percent = (float) depthCount / expected * 100;

                    var averagePerSecond = stopwatch.Elapsed.TotalSeconds / depthCount;

                    var remaining = expected - depthCount;

                    try
                    {
                        var timeRemaining = TimeSpan.FromSeconds(remaining * averagePerSecond);

                        var etr = $"{(timeRemaining.Days > 0 ? $"{timeRemaining.Days:N0}d " : string.Empty)}{timeRemaining.Hours,2:00}:{timeRemaining.Minutes,2:00}.{timeRemaining.Seconds % 60,2:00}";

                        Console.Write($"  {DateTime.Now:HH:mm:ss}: {depthCount:N0} / {expected:N0} ({percent:N2}%) Queue: {core.QueueSize:N0} ETR: {etr}          ");
                    }
                    catch
                    {
                        Console.Write($"  {DateTime.Now:HH:mm:ss}: {depthCount:N0} / {expected:N0} ({percent:N2}%) Queue: {core.QueueSize:N0} ETR: ∞          ");
                    }

                    Console.CursorLeft = y;
                }
            }

            if (! pass)
            {
                fails.Add((i, fen));
            }
        }
        
        stopwatch.Stop();

        if (fails.Count > 0)
        {
            Console.WriteLine();

            foreach (var fail in fails)
            {
                Console.WriteLine($"  Failed test {fail.Index + 1}: {fail.Fen}");
            }
        }

        Console.WriteLine();
        
        Console.WriteLine($"  {tests.Length - fails.Count}/{tests.Length} test{(tests.Length > 1 ? "s" : string.Empty)} passed in {(stopwatch.Elapsed.Days > 0 ? $"{stopwatch.Elapsed.Days:N0}d " : string.Empty)}{(stopwatch.Elapsed.Hours > 0 ? $"{stopwatch.Elapsed.Hours}h " : string.Empty)}{stopwatch.Elapsed.Minutes}m {stopwatch.Elapsed.Seconds:N0}s {stopwatch.Elapsed.Milliseconds}ms");

        Console.WriteLine();
    }

    private static bool TestComplete(Core core, int depth, string[] test, string bestMove)
    {
        Console.Write("                                                                          ");

        Console.CursorLeft = 0;

        var pass = true;

        var part = 0;
        
        for (var i = 0; i < depth; i++)
        {
            if (test[part][1] - '0' != i + 1)
            {
                continue;
            }

            var expected = long.Parse(test[part][3..]);

            var result = core.GetDepthCount(i + 1);
            
            Console.Write($"    Ply: {i + 1} {(result == expected ? "✓" : " ")} {expected,14:N0}");

            if (result != expected)
            {
                Console.Write($"  Delta: {result - expected,14:N0}");

                pass = false;
            }

            Console.WriteLine();
            
            part++;
        }

        Console.WriteLine($"    Best move: {bestMove}.");

        return pass;
    }
}