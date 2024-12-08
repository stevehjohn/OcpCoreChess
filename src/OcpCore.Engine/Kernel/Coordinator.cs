using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Kernel;

public class Coordinator
{
    public static readonly int Threads = Environment.ProcessorCount - 2;

    private readonly PriorityQueue<(Game game, int depth), int> _queue = new();

    private readonly int _maxDepth;
    
    private readonly long[] _depthCounts;
    
    private readonly long[][] _outcomes;

    private readonly StateProcessor[] _processors;

    private CancellationTokenSource _cancellationTokenSource;

    private CancellationToken _cancellationToken;

    private Task[] _tasks;

    public Coordinator(int maxDepth)
    {
        _maxDepth = maxDepth;
        
        _depthCounts = new long[maxDepth + 1];

        _outcomes = new long[maxDepth + 1][];

        for (var i = 1; i <= maxDepth; i++)
        {
            _depthCounts[i] = 0;

            _outcomes[i] = new long[Constants.MoveOutcomes + 1];
        }

        _tasks = new Task[Threads];

        _processors = new StateProcessor[Threads];

        for (var i = 0; i < Threads; i++)
        {
            var processorIndex = i;

            _tasks[i] = new Task(() =>
            {
                _processors[processorIndex] = new StateProcessor(maxDepth, _queue, _depthCounts);
            });
        }
    }

    public void StartProcessing()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _cancellationToken = _cancellationTokenSource.Token;

        for (var i = 0; i < Threads; i++)
        {
            _processors[i].StartProcessing(CoalesceResults, _cancellationToken);
        }
    }

    private void CoalesceResults(StateProcessor processor)
    {
        for (var depth = 1; depth <= _maxDepth; depth++)
        {
            Interlocked.Add(ref _depthCounts[depth], processor.DepthCounts[depth]);

            for (var outcome = 1; outcome <= Constants.MoveOutcomes; outcome++)
            {
                Interlocked.Add(ref _outcomes[depth][outcome], processor.Outcomes[depth][outcome]);
            }
        }
    }
}