using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class StateProcessorTests
{
    [Fact]
    public void CancelsWhenRequested()
    {
        var queue = new PriorityQueue<(Game game, int depth), int>();
        
        var processor = new StateProcessor(queue);

        for (var i = 0; i < 1_000_000; i++)
        {
            queue.Enqueue((new Game(), 1), 0);
        }

        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        Task.Run(() => processor.StartProcessing(10, null, cancellationToken), cancellationToken);
        
        Thread.Sleep(100);
        
        cancellationTokenSource.Cancel();
    }
}