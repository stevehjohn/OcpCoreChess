using System.Diagnostics.CodeAnalysis;
using OcpCore.Engine.PerfTest.Testers;

namespace OcpCore.Engine.PerfTest;

[ExcludeFromCodeCoverage]
public static class EntryPoint
{
    public static void Main(string[] arguments)
    {
        var depth = 6;
        
        if (arguments.Length > 0)
        {
            if (arguments[0].StartsWith("st", StringComparison.InvariantCultureIgnoreCase))
            {
                Etherial.Test();
                
                return;
            }
            
            if (arguments[0].StartsWith("di", StringComparison.InvariantCultureIgnoreCase))
            {
                Stockfish.Test(int.Parse(arguments[1]), arguments[2]);
                
                return;
            }
            
            if (! int.TryParse(arguments[0], out depth))
            {
                depth = 6;
            }
        }
                
        Basic.Test(depth);
    }
}