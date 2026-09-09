using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;

namespace OcpCore.Engine.Kernel;

public sealed class Coordinator : IDisposable
{
    public static readonly int Threads = Math.Max(1, Environment.ProcessorCount - 2);

    // Only independent root subtrees enter this queue. Descendants stay local.
    private readonly Queue<Node> _queue = new();

    private readonly Colour _engineColour;

    private readonly PerfTestCollector _perfTestCollector;

    private readonly int _parallelDepthThreshold;

    private volatile StateProcessor[] _processors = [];

    private CancellationTokenSource _cancellationTokenSource;

    public long GetDepthCount(int ply) => _processors.Sum(processor => processor.GetDepthCount(ply));

    public long GetOutcomeCount(int ply, PlyOutcome outcome) => _processors.Sum(processor => processor.GetOutcomeCount(ply, outcome));

    public int QueueSize
    {
        get
        {
            lock (_queue)
            {
                return _queue.Count;
            }
        }
    }

    public bool IsParallel { get; private set; }

    public Coordinator(Colour engineColour, PerfTestCollector perfTestCollector = null, int parallelDepthThreshold = 6)
    {
        _engineColour = engineColour;

        _perfTestCollector = perfTestCollector;

        _parallelDepthThreshold = parallelDepthThreshold;
    }

    public void StartProcessing(Game game, int maxDepth)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxDepth, 1);

        _processors = [];

        lock (_queue)
        {
            _queue.Clear();
        }

        _perfTestCollector?.Clear();

        using var cancellationTokenSource = new CancellationTokenSource();

        _cancellationTokenSource = cancellationTokenSource;

        IsParallel = maxDepth >= _parallelDepthThreshold;

        try
        {
            if (! IsParallel)
            {
                lock (_queue)
                {
                    _queue.Enqueue(new Node(game, maxDepth, -1));
                }

                var processor = new StateProcessor(_engineColour, _queue, _perfTestCollector);

                _processors = [processor];

                processor.StartProcessing(maxDepth, null, cancellationTokenSource.Token);

                return;
            }

            var rootProcessor = new StateProcessor(_engineColour, _queue, _perfTestCollector);

            _processors = [rootProcessor];

            var seeds = rootProcessor.PrepareWork(game, maxDepth, cancellationTokenSource.Token);

            lock (_queue)
            {
                foreach (var seed in seeds)
                {
                    _queue.Enqueue(seed);
                }
            }

            var workers = Math.Min(Threads, seeds.Count);

            var processors = new StateProcessor[workers + 1];

            processors[0] = rootProcessor;

            var collectors = new PerfTestCollector[workers];

            var tasks = new Task[workers];

            for (var i = 0; i < workers; i++)
            {
                var collector = _perfTestCollector == null ? null : new PerfTestCollector();

                var processor = new StateProcessor(_engineColour, _queue, collector);

                collectors[i] = collector;

                processors[i + 1] = processor;

                tasks[i] = Task.Run(() => processor.StartProcessing(maxDepth, null, cancellationTokenSource.Token));
            }

            _processors = processors;

            Task.WaitAll(tasks);

            for (var i = 0; i < workers; i++)
            {
                _perfTestCollector?.Merge(collectors[i]);
            }
        }
        finally
        {
            _cancellationTokenSource = null;
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
}
