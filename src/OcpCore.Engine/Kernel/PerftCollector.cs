using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.Kernel;

public class PerftCollector
{
    private readonly Dictionary<string, long> _counts = [];

    public IReadOnlyDictionary<string, long> Counts => _counts;
    
    public void AddCount(int ply, int maxDepth, int root, int count = 1)
    {
        var node = $"{(root >> 8).ToStandardNotation()}{(root & 0xFF).ToStandardNotation()}";
        
        if (ply == 1)
        {
            _counts.Add(node, maxDepth == 1 ? 1 : 0);
        }
        else if (ply == maxDepth)
        {
            _counts[node] += count;
        }
    }
}