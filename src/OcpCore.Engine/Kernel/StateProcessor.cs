using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private readonly int _maxDepth;

    private readonly PriorityQueue<(Game game, int depth), int> _centralQueue;
    
    private readonly PriorityQueue<(Game game, int depth), int> _localQueue = new();

    private readonly long[] _depthCounts;
    
    private readonly long[][] _outcomes;

    public StateProcessor(int maxDepth, PriorityQueue<(Game game, int depth), int> centralQueue)
    {
        _maxDepth = maxDepth;

        _centralQueue = centralQueue;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }
    }
}