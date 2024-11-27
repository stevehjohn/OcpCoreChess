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
        var depth = 5;
        
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
            var core = new Core(Colour.White);

            Console.WriteLine($"  {DateTime.Now:HH:mm:ss} Starting depth {maxDepth}");

            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();
            
            core.GetMove(depth);
            
            stopwatch.Stop();
        }
    }
}