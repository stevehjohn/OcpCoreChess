using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
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
        
        var queue = new PriorityQueue<Node, int>();

        queue.Enqueue(new Node(game, 1, -1), 0);
        
        var processor = new StateProcessor(Colour.White, queue);
        
        using var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        
        processor.StartProcessing(1, null, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(0, processor.GetDepthCount(1));
    }

    [Fact]
    public void DoesNotDequeueAllItems()
    {
        var queue = new PriorityQueue<Node, int>();

        var game = new Game();
        
        for (var i = 0; i < Coordinator.Threads * 10; i++)
        {
            queue.Enqueue(new Node(game, 1, -1), 0);
        }

        var processor = new StateProcessor(Colour.White, queue);
        
        using var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        
        processor.StartProcessing(2, null, cancellationToken);
        
        Assert.True(queue.Count > Coordinator.Threads * 5);
    }
    
    [Fact]
    public void CanHandleAnEmptyQueueGracefully()
    {
        var queue = new PriorityQueue<Node, int>();

        var processor = new StateProcessor(Colour.White, queue);
        
        using var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        var called = false;
        
        processor.StartProcessing(2, (_, _) => called = true, cancellationToken);
        
        Assert.True(called);    }
    
    [Fact]
    public void DoesNotFreezeIfQueueIsLocked()
    {
        var queue = new PriorityQueue<Node, int>();

        var processor = new StateProcessor(Colour.White, queue);
        
        using var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();

        var called = false;
        
        lock (queue)
        {
            processor.StartProcessing(2, (_, _) => called = true, cancellationToken);
        }
        
        Assert.True(called);
    }

    [Theory]
    [InlineData("8/8/2P5/8/8/8/8/8 w - - 0 1", 1, 0, 0, 0, 0)]
    [InlineData("6pk/2P5/8/8/8/8/8/8 w - - 0 1", 4, 4, 0, 0, 0)]
    [InlineData("4k3/2P5/8/8/8/8/8/8 w - - 0 1", 4, 4, 0, 2, 0)]
    [InlineData("5kp1/2P1ppp1/8/8/8/8/8/8 w - - 0 1", 4, 4, 0, 2, 2)]
    [InlineData("3p4/2P5/8/8/8/8/8/8 w - - 0 1", 8, 8, 4, 0, 0)]
    public void HandlesPromotionsCorrectly(string fen, int expectedMoves, int promotions, int captures, int checks, int checkmates)
    {
        var game = new Game();
        
        game.ParseFen(fen);
        
        var queue = new PriorityQueue<Node, int>();

        queue.Enqueue(new Node(game, 2, -1), 0);
        
        var processor = new StateProcessor(Colour.White, queue);
        
        using var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        processor.StartProcessing(2, (_, _) => { }, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(expectedMoves, processor.GetDepthCount(1));

        Assert.Equal(promotions, processor.GetOutcomeCount(1, (PlyOutcome) (int) PlyOutcome.Promotion));
        
        Assert.Equal(captures, processor.GetOutcomeCount(1, (PlyOutcome) (int) PlyOutcome.Capture));
        
        Assert.Equal(checks, processor.GetOutcomeCount(1, (PlyOutcome) (int) PlyOutcome.Check));
        
        Assert.Equal(checkmates, processor.GetOutcomeCount(1, (PlyOutcome) (int) PlyOutcome.CheckMate));
    }
}