using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.PerfTest.Testers;

namespace OcpCore.Engine.PerfTest;

[ExcludeFromCodeCoverage]
public static class EntryPoint
{
    public static void Main(string[] arguments)
    {
        if (arguments.Length > 0)
        {
            if (! int.TryParse(arguments[0], out var depth))
            {
                depth = 6;
            }

            if (arguments[0].StartsWith("st", StringComparison.InvariantCultureIgnoreCase))
            {
                Etherial.Test();
                
                return;
            }
                
            Basic.Test(depth);
        }
    }
}