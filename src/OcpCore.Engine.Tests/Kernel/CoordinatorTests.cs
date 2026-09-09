using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
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
        using var coordinator = new Coordinator(Colour.White, null, parallelisationDepth);

        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);

        coordinator.StartProcessing(game, requestedDepth);
        
        Assert.Equal(expectParallelisation, coordinator.IsParallel);
    }

    [Fact]
    public void RejectsInvalidDepth()
    {
        using var coordinator = new Coordinator(Colour.White, null, 0);

        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);

        Assert.Throws<ArgumentOutOfRangeException>(() => coordinator.StartProcessing(game, 0));
    }
    
    [Fact]
    public void ReportsEmptyQueueSize()
    {
        using var coordinator = new Coordinator(Colour.White);
        
        Assert.Equal(0, coordinator.QueueSize);
    }

    [Theory]
    [InlineData(Constants.InitialBoardFen, 3)]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 3)]
    [InlineData("4k3/8/8/2PpP3/8/8/8/4K3 w - d6 0 1", 3)]
    [InlineData("7k/P7/8/8/8/8/8/4K3 w - - 0 1", 3)]
    [InlineData("7k/P7/8/8/8/8/8/4K3 w - - 0 1", 1)]
    [InlineData("7k/6Q1/5K2/8/8/8/8/8 b - - 0 1", 3)]
    public void ParallelResultsMatchSerialIncludingRepeatedRuns(string fen, int depth)
    {
        var game = new Game();

        game.ParseFen(fen);

        var serialCollector = new PerfTestCollector();

        var parallelCollector = new PerfTestCollector();

        using var serial = new Coordinator(Colour.White, serialCollector, int.MaxValue);

        using var parallel = new Coordinator(Colour.White, parallelCollector, 0);

        for (var run = 0; run < 2; run++)
        {
            serial.StartProcessing(game, depth);

            parallel.StartProcessing(game, depth);

            for (var ply = 1; ply <= depth; ply++)
            {
                Assert.Equal(serial.GetDepthCount(ply), parallel.GetDepthCount(ply));

                for (var outcome = PlyOutcome.Move; outcome < PlyOutcome.Null; outcome = (PlyOutcome) ((int) outcome << 1))
                {
                    Assert.Equal(serial.GetOutcomeCount(ply, outcome), parallel.GetOutcomeCount(ply, outcome));
                }
            }

            Assert.Equal(serialCollector.Counts.OrderBy(item => item.Key), parallelCollector.Counts.OrderBy(item => item.Key));

            Assert.Equal(parallel.GetDepthCount(depth), parallelCollector.Counts.Values.Sum());
        }
    }

    [Fact]
    public void ParallelDepthSixPreservesKnownCounts()
    {
        using var core = new Core(Colour.White, true);

        core.GetMove(6);

        Assert.Equal(119_060_324, core.GetDepthCount(6));

        Assert.Equal(2_812_008, core.GetOutcomeCount(6, PlyOutcome.Capture));

        Assert.Equal(5_248, core.GetOutcomeCount(6, PlyOutcome.EnPassant));

        Assert.Equal(809_099, core.GetOutcomeCount(6, PlyOutcome.Check));

        Assert.Equal(10_828, core.GetOutcomeCount(6, PlyOutcome.CheckMate));

        Assert.Equal(core.GetDepthCount(6), core.PerftData.Values.Sum());
    }
}