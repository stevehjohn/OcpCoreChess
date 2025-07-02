namespace OcpCore.Engine.Extensions;

public static class CoreExtensions
{
    public static void OutputBoard(this Core core, bool invert = false)
    {
        var foreground = Console.ForegroundColor;

        var background = Console.BackgroundColor;
        
        var state = core.ToString();

        Console.ForegroundColor = ConsoleColor.White;

        Console.BackgroundColor = ConsoleColor.Black;

        Console.Write("  ");

        var i = invert ? state.Length - 1 : 0;

        var delta = invert ? -1 : 1;

        while (true)
        {
            if (state[i] == '|')
            {
                Console.WriteLine();
                
                Console.Write("  ");

                i += delta;
            }

            if (char.IsUpper(state[i]))
            {
                Console.ForegroundColor = ConsoleColor.Black;

                Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.BackgroundColor = ConsoleColor.Black;
            }

            Console.Write(state[i]);

            Console.ForegroundColor = ConsoleColor.White;

            Console.BackgroundColor = ConsoleColor.Black;

            i += delta;

            if (i < 0 || i >= state.Length)
            {
                break;
            }
        }

        Console.ForegroundColor = foreground;

        Console.BackgroundColor = background;
        
        Console.WriteLine();
    }
}