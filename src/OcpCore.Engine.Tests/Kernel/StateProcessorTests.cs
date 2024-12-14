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
        
        var queue = new PriorityQueue<(Game game, int depth, int root), int>();

        queue.Enqueue((game, 1, -1), 0);
        
        var processor = new StateProcessor(queue);
        
        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        cancellationTokenSource.Cancel();
        
        processor.StartProcessing(1, null, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(0, processor.GetDepthCount(1));
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
        
        var queue = new PriorityQueue<(Game game, int depth, int root), int>();

        queue.Enqueue((game, 2, -1), 0);
        
        var processor = new StateProcessor(queue);
        
        var cancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = cancellationTokenSource.Token;
        
        processor.StartProcessing(2, (_, _) => { }, cancellationToken);
        
        Thread.Sleep(100);
        
        Assert.Equal(expectedMoves, processor.GetDepthCount(1));

        Assert.Equal(promotions, processor.GetOutcomeCount(1, (MoveOutcome) (int) MoveOutcome.Promotion));
        
        Assert.Equal(captures, processor.GetOutcomeCount(1, (MoveOutcome) (int) MoveOutcome.Capture));
        
        Assert.Equal(checks, processor.GetOutcomeCount(1, (MoveOutcome) (int) MoveOutcome.Check));
        
        Assert.Equal(checkmates, processor.GetOutcomeCount(1, (MoveOutcome) (int) MoveOutcome.CheckMate));
    }
}