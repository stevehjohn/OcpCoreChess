using System.Numerics;
using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public sealed class Coordinator : IDisposable
{
    public static readonly int Threads = Environment.ProcessorCount - 2;

    private readonly PriorityQueue<Node, int> _queue = new();

    private readonly StateProcessor[] _processors;

    private readonly int _parallelDepthThreshold;

    private int _maxDepth;
    
    private long[] _depthCounts;
    
    private long[][] _outcomes;

    private CancellationTokenSource _cancellationTokenSource;

    private CancellationToken _cancellationToken;

    private CountdownEvent _countdownEvent;

    private Node _rootNode;

    public long GetDepthCount(int ply) => _depthCounts[ply];

    public long GetOutcomeCount(int ply, MoveOutcome outcome) => _outcomes[ply][BitOperations.Log2((byte) outcome) + 1];
    
    public int QueueSize { get; private set; }

    public bool IsParallel => _countdownEvent != null;

    public string BestMove => _rootNode.Move;

    public Coordinator(PerfTestCollector perfTestCollector = null, int parallelDepthThreshold = 6)
    {
        _parallelDepthThreshold = parallelDepthThreshold;
        
        _processors = new StateProcessor[Threads];

        for (var i = 0; i < Threads; i++)
        {
            _processors[i] = new StateProcessor(_queue, perfTestCollector);
        }
    }

    public void StartProcessing(Game game, int maxDepth, bool isMaximising)
    {
        _maxDepth = maxDepth;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }

        _queue.Clear();

        _rootNode = new Node(game, _maxDepth, isMaximising);
        
        _queue.Enqueue(_rootNode, 0);

        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        if (maxDepth < _parallelDepthThreshold)
        {
            _processors[0].StartProcessing(maxDepth, CoalesceResults, _cancellationToken);
        }
        else
        {
            _countdownEvent = new CountdownEvent(Threads);

            Exception exception = null;

            for (var i = 0; i < Threads; i++)
            {
                var index = i;
            
                Task.Factory.StartNew(() => _processors[index].StartProcessing(maxDepth, CoalesceResults, _cancellationToken), _cancellationToken)
                    .ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            exception = t.Exception;
                        }
                    }, _cancellationToken);
            }

            while (! _countdownEvent.IsSet)
            {
                if (exception != null)
                {
                    throw exception;
                }

                QueueSize = _queue.Count;
            
                Thread.Sleep(500);
            }
        }
        
        _cancellationTokenSource.Dispose();

        _cancellationTokenSource = null;
    }

    private void CoalesceResults(StateProcessor processor, bool isComplete)
    {
        for (var depth = 1; depth <= _maxDepth; depth++)
        {
            Interlocked.Add(ref _depthCounts[depth], processor.GetDepthCount(depth));
        }

        if (isComplete)
        {
            for (var depth = 1; depth <= _maxDepth; depth++)
            {
                for (var outcome = 0; outcome <= Constants.MoveOutcomes; outcome++)
                {
                    Interlocked.Add(ref _outcomes[depth][outcome], processor.GetOutcomeCount(depth, (MoveOutcome) (1 << outcome) - 1));
                }
            }

            _countdownEvent?.Signal();
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Dispose();

        _cancellationTokenSource = null;
        
        _countdownEvent?.Dispose();

        _countdownEvent = null;
    }
}