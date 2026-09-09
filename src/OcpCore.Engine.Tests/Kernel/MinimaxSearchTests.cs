using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class MinimaxSearchTests
{
    [Theory]
    [InlineData(Constants.InitialBoardFen)]
    [InlineData("4k3/8/8/3q4/8/8/3R4/4K3 w - - 0 1")]
    [InlineData("4k3/3r4/8/8/3Q4/8/8/4K3 b - - 0 1")]
    public void PruningPreservesMinimaxResult(string fen)
    {
        var game = new Game();

        game.ParseFen(fen);

        var search = new MinimaxSearch();

        var expected = search.FindBestMove(game, 3, false);

        var exhaustiveNodes = search.NodesVisited;

        var actual = search.FindBestMove(game, 3);

        Assert.Equal(expected, actual);

        Assert.True(search.NodesVisited < exhaustiveNodes);

        Assert.True(search.Cutoffs > 0);
    }

    [Theory]
    [InlineData("4k3/8/8/3q4/8/8/3R4/4K3 w - - 0 1", "d2d5")]
    [InlineData("4k3/3r4/8/8/3Q4/8/8/4K3 b - - 0 1", "d7d4")]
    public void WinsMaterialForEitherColour(string fen, string expected)
    {
        using var core = new Core(Colour.White, fen);

        var before = core.ToString();

        Assert.Equal(expected, core.GetMove(2).Move);

        Assert.Equal(before, core.ToString());
    }

    [Fact]
    public void AvoidsCaptureWhichLosesQueenToRecapture()
    {
        using var core = new Core(Colour.White, "3rk3/8/8/3p4/8/8/3Q4/4K3 w - - 0 1");

        Assert.Equal("d2d5", core.GetMove(1).Move);

        Assert.NotEqual("d2d5", core.GetMove(2).Move);
    }

    [Theory]
    [InlineData("7k/6Q1/5K2/8/8/8/8/8 b - - 0 1", MoveOutcome.OpponentInCheckmate)]
    [InlineData("7k/5Q2/6K1/8/8/8/8/8 b - - 0 1", MoveOutcome.Stalemate)]
    [InlineData("8/8/8/8/8/5k2/6q1/7K w - - 0 1", MoveOutcome.EngineInCheckmate)]
    public void ReportsTerminalPositions(string fen, MoveOutcome expected)
    {
        using var core = new Core(Colour.White, fen);

        Assert.Equal((expected, string.Empty), core.GetMove(2));
    }

    [Fact]
    public void FindsMateAtDepthHorizon()
    {
        using var core = new Core(Colour.White, "7k/8/5KQ1/8/8/8/8/8 w - - 0 1");

        var result = core.GetMove(1);

        Assert.Equal(MoveOutcome.Move, result.Outcome);

        core.MakeMove(result.Move);

        Assert.Equal(MoveOutcome.OpponentInCheckmate, core.GetMove(1).Outcome);
    }

    [Fact]
    public void SearchesAndAppliesPromotion()
    {
        using var core = new Core(Colour.White, "7k/P7/8/8/8/8/8/4K3 w - - 0 1");

        var result = core.GetMove(1);

        Assert.Equal("a7a8q", result.Move);

        core.MakeMove(result.Move);

        Assert.Equal((Colour.White, Kind.Queen), core["a8"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RejectsInvalidDepth(int depth)
    {
        using var core = new Core(Colour.White);

        Assert.Throws<ArgumentOutOfRangeException>(() => core.GetMove(depth));
    }
}
