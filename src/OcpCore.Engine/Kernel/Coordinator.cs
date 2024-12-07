using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class Coordinator
{
    public static readonly int Threads = Environment.ProcessorCount - 2;

    private readonly PriorityQueue<(Game game, int depth), int> _queue = new();
    
    private readonly long[] _depthCounts;
    
    private readonly long[][] _outcomes;

    private Task[] _processors;

    public Coordinator(int maxDepth)
    {
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }
    }
}