using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.Kernel;

public class PerfTestCollector
{
    // Root moves use from << 8 | to. Format notation only when reporting.
    private readonly long[] _counts = new long[64 << 8];

    private readonly bool[] _roots = new bool[64 << 8];

    public IReadOnlyDictionary<string, long> Counts
    {
        get
        {
            var counts = new Dictionary<string, long>();

            for (var root = 0; root < _counts.Length; root++)
            {
                if (_roots[root])
                {
                    var move = $"{(root >> 8).ToStandardNotation()}{(root & 0xFF).ToStandardNotation()}";

                    counts.Add(move, _counts[root]);
                }
            }

            return counts;
        }
    }

    public void Clear()
    {
        Array.Clear(_counts);

        Array.Clear(_roots);
    }

    public void AddCount(int ply, int maxDepth, int root, int count)
    {
        if (ply == 1)
        {
            _roots[root] = true;
        }

        if (ply == maxDepth)
        {
            _counts[root] += count;
        }
    }

    internal void Merge(PerfTestCollector collector)
    {
        for (var root = 0; root < _counts.Length; root++)
        {
            _roots[root] |= collector._roots[root];

            _counts[root] += collector._counts[root];
        }
    }
}
