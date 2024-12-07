using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class StateProcessor
{
    private static readonly int Threads = Environment.ProcessorCount - 2;
    
    private readonly int _maxDepth;

    private readonly PriorityQueue<(Game game, int depth), int> _centralQueue;

    private readonly CancellationToken _cancellationToken;
    
    private readonly PriorityQueue<(Game game, int depth), int> _localQueue = new();

    private readonly long[] _depthCounts;
    
    private readonly long[][] _outcomes;

    public StateProcessor(int maxDepth, PriorityQueue<(Game game, int depth), int> centralQueue, CancellationToken cancellationToken)
    {
        _maxDepth = maxDepth;

        _centralQueue = centralQueue;

        _cancellationToken = cancellationToken;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }
    }

    public void StartProcessing()
    {
        while (_centralQueue.Count > 0)
        {
            lock (_centralQueue)
            {
                for (var i = 0; i < Math.Max(1, _centralQueue.Count / Threads); i++)
                {
                    if (_centralQueue.TryDequeue(out var workItem, out var priority))
                    {
                        _localQueue.Enqueue(workItem, priority);
                        
                        continue;
                    }
                    
                    break;
                }
            }

            while (_localQueue.Count > 0)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var (game, depth) = _localQueue.Dequeue();
                
                ProcessWorkItem(game, depth);
            }
        }
    }

    private void ProcessWorkItem(Game game, int depth)
    {
    }
}