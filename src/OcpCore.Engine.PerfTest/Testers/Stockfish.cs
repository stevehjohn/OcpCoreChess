using System.Diagnostics;

namespace OcpCore.Engine.PerfTest.Testers;

public static class Stockfish
{
    public static void Test(string fen)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "stockfish",
            RedirectStandardOutput = true,
            RedirectStandardInput = true
        };

        process.Start();
        
        Interact(process);
        
        process.WaitForExit();
    }

    private static void Interact(Process stockfish)
    {
        var stage = 0;

        var sw = new Stopwatch();
        
        while (true)
        {
            sw.Start();
            
            var output = stockfish.StandardOutput.ReadLine();
            
            Console.WriteLine(output);

            switch (stage)
            {
                case 0:
                    stockfish.StandardInput.WriteLine("uci");
                    stage++;
                    break;
                
                case 1:
                    stockfish.StandardInput.WriteLine("isready");
                    stage++;
                    break;
            }
        }
    }
}