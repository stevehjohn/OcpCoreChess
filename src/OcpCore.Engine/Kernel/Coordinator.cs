namespace OcpCore.Engine.Kernel;

public class Coordinator
{
    public static readonly int Threads = Environment.ProcessorCount - 1;

    private Task[] _processors;

    public Coordinator()
    {
    }
}