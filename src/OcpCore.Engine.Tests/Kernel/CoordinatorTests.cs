using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class CoordinatorTests
{
    [Theory]
    [InlineData(2, 3, false)]
    [InlineData(5, 3, true)]
    public void ParallelisesAtGivenLevel(int requestedDepth, int parallelisationDepth, bool expectParallelisation)
    {
        using var coordinator = new Coordinator(null, parallelisationDepth);

        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);

        coordinator.StartProcessing(game, requestedDepth);
        
        Assert.Equal(expectParallelisation, coordinator.IsParallel);
    }

    [Fact]
    public void CatchesExceptions()
    {
        using var coordinator = new Coordinator(null, 0);

        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);

        var threw = false;
        
        try
        {
            coordinator.StartProcessing(game, 0);
        }
        catch
        {
            threw = true;
        }
        
        Assert.True(threw);
    }
    
    [Fact]
    public void ReportsEmptyQueueSize()
    {
        using var coordinator = new Coordinator();
        
        Assert.Equal(0, coordinator.QueueSize);
    }
}