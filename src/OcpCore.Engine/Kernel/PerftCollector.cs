using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.Kernel;

public class PerftCollector
{
    private readonly Dictionary<string, long> _counts = [];
    
    public void AddCount(int ply, int root, int count = 1)
    {
        var node = $"{(root >> 8).ToStandardNotation()}{(root & 0xFF).ToStandardNotation()}";
        
        if (ply == 1)
        {
            _counts.Add(node, count);
        }
        else
        {
            _counts[node] += count;
        }
    }
}