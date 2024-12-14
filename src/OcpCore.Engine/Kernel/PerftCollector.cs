namespace OcpCore.Engine.Kernel;

public class PerftCollector
{
    private Dictionary<string, long> _counts;

    public void AddCount(int ply, string move, int count = 1)
    {
        if (ply == 1)
        {
            _counts.Add(move, count);
        }
        else
        {
            _counts[move] += count;
        }
    }
}