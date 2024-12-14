using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class StateProcessorTests
{
    [Fact]
    public void CancelsWhenRequested()
    {
        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);
        
        var queue = new PriorityQueue<(Game game, int depth, int root), int>();

        queue.Enqueue((game, 1, -1), 0);
        
        var processor = new StateProcessor(queue);
        
        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        
        processor.StartProcessing(1, (_, _) => { }, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(0, processor.GetDepthCount(1));
    }

    [Theory]
    [InlineData("8/8/2P5/8/8/8/8/8 w - - 0 1", 1)]
    [InlineData("8/2P5/8/8/8/8/8/8 w - - 0 1", 4)]
    public void HandlesPromotionsCorrectly(string fen, int expectedTotal)
    {
        var game = new Game();
        
        game.ParseFen(fen);
        
        var queue = new PriorityQueue<(Game game, int depth, int root), int>();

        queue.Enqueue((game, 1, -1), 0);
        
        var processor = new StateProcessor(queue);
        
        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        processor.StartProcessing(1, (_, _) => { }, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(expectedTotal, processor.GetDepthCount(1));
    }
}