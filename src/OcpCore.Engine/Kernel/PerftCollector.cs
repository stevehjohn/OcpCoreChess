using OcpCore.Engine.Extensions;

namespace OcpCore.Engine.Kernel;

public class PerftCollector
{
    private readonly Dictionary<string, long> _counts = [];

    public IReadOnlyDictionary<string, long> Count => _counts;
    
    public void AddCount(int ply, int from, int to, int count = 1)
    {
        if (ply == 1)
        {
            _counts.Add($"{from.ToStandardNotation()}{to.ToStandardNotation()}", count);
        }
        else
        {
            //_counts[move] += count;
        }
    }
}