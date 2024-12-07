namespace OcpCore.Engine.Kernel;

public class Coordinator
{
    public static readonly int Threads = Environment.ProcessorCount - 2;

    private Task[] _processors;

    public Coordinator()
    {
    }
}