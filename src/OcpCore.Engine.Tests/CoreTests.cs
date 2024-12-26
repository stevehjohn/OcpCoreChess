using OcpCore.Engine.Exceptions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using Xunit;

namespace OcpCore.Engine.Tests;

public class CoreTests
{
    [Theory]
    [InlineData(1, 20, 0, 0, 0, 0, 0, 0)]
    [InlineData(2, 400, 0, 0, 0, 0, 0, 0)]
    [InlineData(3, 8_902, 34, 0, 0, 0, 12, 0)]
    [InlineData(4, 197_281, 1_576, 0, 0, 0, 469, 8)]
    [InlineData(5, 4_865_609, 82_719, 258, 0, 0, 27_351, 347)]
    public void ReturnsExpectedCountAtPly(int ply, int count, int capture, int enPassant, int castle, int promotion, int check, int mate)
    {
        using var core = new Core(Colour.White);
    
        core.GetMove(ply);
        
        Assert.Equal(count, core.GetDepthCount(ply));
        
        Assert.Equal(capture, core.GetOutcomeCount(ply, MoveOutcome.Capture));
        
        Assert.Equal(enPassant, core.GetOutcomeCount(ply, MoveOutcome.EnPassant));
        
        Assert.Equal(castle, core.GetOutcomeCount(ply, MoveOutcome.Castle));
        
        Assert.Equal(promotion, core.GetOutcomeCount(ply, MoveOutcome.Promotion));
        
        Assert.Equal(check, core.GetOutcomeCount(ply, MoveOutcome.Check));
        
        Assert.Equal(mate, core.GetOutcomeCount(ply, MoveOutcome.CheckMate));
    }

    [Theory]
    [InlineData(Constants.InitialBoardFen, "a2a4", true, Kind.Pawn)]
    [InlineData(Constants.InitialBoardFen, "a2b4", false, Kind.Pawn)]
    [InlineData(Constants.InitialBoardFen, "b1a3", true, Kind.Knight)]
    [InlineData(Constants.InitialBoardFen, "b1b3", false, Kind.Knight)]
    public void ChecksMoveValidity(string fen, string move, bool isValid, Kind kind)
    {
        using var core = new Core(Colour.White, fen);

        if (isValid)
        {
            core.MakeMove(move);
        }
        else
        {
            var exception = Assert.Throws<InvalidMoveException>(() => core.MakeMove(move));
            
            Assert.Equal($"{move} is not a valid move for a {kind}.", exception.Message);
        }
    }

    [Theory]
    [InlineData("2B1k2r/p1ppqpb1/1n2pnp1/3PN3/1p2P3/2N2Q1p/PPPB1PPP/R3K2R b KQk - 0 2", "e8g8")]
    public void DoesNotMakeMistakesIdentifiedInPerfTesting(string fen, string expectedMove)
    {
        using var core = new Core(Colour.White, fen);

        var moves = core.GetAllowedMoves();

        Assert.Equal(1, moves.Count(m => m == expectedMove));
    }

    [Theory]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", "e2a6,a8c8,a6c8", "e8g8")]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", "a1b1,a6e2,b1a1,e2d1", "e1g1")]
    [InlineData("r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1", "a1a2,a8a3,a2h2,h8h2", "e1g1")]
    [InlineData("r3k2r/8/8/8/8/8/8/2R1K2R w Kkq - 0 1", "c1c8,e8d7,c8h8,a8h8", "e1g1")]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", "e5f7,e7f8,f7h8", "e8c8")]
    public void DoesNotMakeMistakesIdentifiedInPerfTestingAfterMoving(string fen, string movesToMake, string expectedMove)
    {
        using var core = new Core(Colour.White, fen);

        var moves = movesToMake.Split(',');

        foreach (var move in moves)
        {
            core.MakeMove(move);
        }

        var allowedMoves = core.GetAllowedMoves();

        Assert.Equal(1, allowedMoves.Count(m => m == expectedMove));
    }

    [Theory]
    [InlineData("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1")]
    public void ReturnsValidAllowedMoves(string fen)
    {
        using var core = new Core(Colour.White, fen);

        var moves = core.GetAllowedMoves();

        foreach (var move in moves)
        {
            var testCore = new Core(Colour.White, fen);
            
            testCore.MakeMove(move);
        }
    }

    [Theory]
    [InlineData("8/8/8/8/1k1PpN1R/8/8/4K3 b - d3 0 1", 9, 193)]
    [InlineData("8/8/8/8/1k1Ppn1R/8/8/4K3 b - d3 0 1", 17, 220)]
    [InlineData("4k3/8/8/2PpP3/8/8/8/4K3 w - d6 0 1", 9, 47, 376)]
    [InlineData("4k3/8/8/8/2pPp3/8/8/4K3 b - d3 0 1", 9, 47, 376)]
    [InlineData("4k3/b7/8/2Pp4/8/8/8/6K1 w - d6 0 1", 5, 45)]
    [InlineData("4k3/7b/8/4pP2/8/8/8/1K6 w - e6 0 1", 5, 45)]
    [InlineData("6k1/8/8/8/2pP4/8/B7/3K4 b - d3 0 1", 5, 45)]
    [InlineData("1k6/8/8/8/4Pp2/8/7B/4K3 b - e3 0 1", 5, 45)]
    [InlineData("4k3/b7/8/1pP5/8/8/8/6K1 w - b6 0 1", 6, 52)]
    [InlineData("4k3/7b/8/5Pp1/8/8/8/1K6 w - g6 0 1", 6, 51)]
    [InlineData("6k1/8/8/8/1Pp5/8/B7/4K3 b - b3 0 1", 6, 52)]
    [InlineData("1k6/8/8/8/5pP1/8/7B/4K3 b - g3 0 1", 6, 51)]
    [InlineData("4k3/K7/8/1pP5/8/8/8/6b1 w - b6 0 1", 6, 66)]
    [InlineData("4k3/7K/8/5Pp1/8/8/8/1b6 w - g6 0 1", 6, 60)]
    [InlineData("6B1/8/8/8/1Pp5/8/k7/4K3 b - b3 0 1", 6, 66)]
    [InlineData("1B6/8/8/8/5pP1/8/7k/4K3 b - g3 0 1", 6, 60)]
    [InlineData("4k3/b7/8/2Pp4/3K4/8/8/8 w - d6 0 1", 5, 44)]
    [InlineData("4k3/8/1b6/2Pp4/3K4/8/8/8 w - d6 0 1", 6, 59)]
    [InlineData("4k3/8/b7/1Pp5/2K5/8/8/8 w - c6 0 1", 6, 49)]
    [InlineData("4k3/8/7b/5pP1/5K2/8/8/8 w - f6 0 1", 6, 49)]
    [InlineData("4k3/7b/8/4pP2/4K3/8/8/8 w - e6 0 1", 5, 44)]
    [InlineData("4k3/8/6b1/4pP2/4K3/8/8/8 w - e6 0 1", 6, 53)]
    [InlineData("4k3/8/3K4/1pP5/8/q7/8/8 w - b6 0 1", 5, 114)]
    [InlineData("7k/4K3/8/1pP5/8/q7/8/8 w - b6 0 1", 8, 171)]
    [InlineData("4k3/2rn4/8/2K1pP2/8/8/8/8 w - e6 0 1", 4, 75)]
    [InlineData("4k3/8/8/K2pP2r/8/8/8/8 w - d6 0 1", 6, 94)]
    [InlineData("4k3/8/8/K2pP2q/8/8/8/8 w - d6 0 1", 6, 130)]
    [InlineData("4k3/8/8/r2pP2K/8/8/8/8 w - d6 0 1", 6, 87)]
    [InlineData("4k3/8/8/q2pP2K/8/8/8/8 w - d6 0 1", 6, 129)]
    [InlineData("8/8/8/8/1k1Pp2R/8/8/4K3 b - d3 0 1", 8, 125)]
    [InlineData("8/8/8/8/1R1Pp2k/8/8/4K3 b - d3 0 1", 6, 87)]
    [InlineData("k7/8/4r3/3pP3/8/8/8/4K3 w - d6 0 1", 5, 70)]
    [InlineData("k3K3/8/8/3pP3/8/8/8/4r3 w - d6 0 1", 6, 91)]
    [InlineData("4k3/8/8/4pP2/3K4/8/8/8 w - e6 0 1", 9, 49)]
    [InlineData("8/8/8/4k3/5Pp1/8/8/3K4 b - f3 0 1", 9, 50)]
    [InlineData("4k3/8/K6r/3pP3/8/8/8/8 w - d6 0 1", 6, 109)]
    [InlineData("4k3/8/K6q/3pP3/8/8/8/8 w - d6 0 1", 6, 151)]
    [InlineData("4k3/8/4r3/8/8/8/3p4/4K3 w - - 0 1", 4, 80, 320)]
    [InlineData("4k3/8/4q3/8/8/8/3b4/4K3 w - - 0 1", 4, 143, 496)]
    [InlineData("4k3/8/8/8/1b5b/8/3Q4/4K3 w - - 0 1", 3, 54, 1256)]
    [InlineData("4k3/8/8/8/1b5b/8/3R4/4K3 w - - 0 1", 3, 54, 836)]
    [InlineData("4k3/8/8/8/1b5b/2Q5/5P2/4K3 w - - 0 1", 6, 98, 2274)]
    [InlineData("4k3/8/8/8/1b5b/2R5/5P2/4K3 w - - 0 1", 4, 72, 1300)]
    [InlineData("4k3/8/8/8/1b2r3/8/3Q4/4K3 w - - 0 1", 3, 66, 1390)]
    [InlineData("4k3/8/8/8/1b2r3/8/3QP3/4K3 w - - 0 1", 6, 119, 2074)]
    public void EdgeCaseTests(string fen, params int[] depths)
    {
        using var core = new Core(Colour.White, fen);

        for (var d = 0; d < depths.Length; d++)
        {
            core.GetMove(d + 1);

            Assert.Equal(depths[d], core.GetDepthCount(d + 1));
        }
    }

    [Fact]
    public void CanHandleStaleMates()
    {
        var core = new Core(Colour.White, "7k/5Q2/6KP/8/8/8/8/8 b - - 0 1");

        core.GetMove(2);
        
        Assert.Equal(0, core.GetDepthCount(1));
    }

    [Fact]
    public void ReportsPerftResultsWhenEnabled()
    {
        using var core = new Core(Colour.White, true);

        core.GetMove(2);

        foreach (var item in core.PerftData)
        {
            Assert.Equal(20, item.Value);
        }
    }
    [Fact]
    
    public void ReportsPerftResultsFromFenWhenEnabled()
    {
        using var core = new Core(Colour.White, Constants.InitialBoardFen, true);

        core.GetMove(2);

        foreach (var item in core.PerftData)
        {
            Assert.Equal(20, item.Value);
        }
    }

    [Fact]
    public void ReportsWhenNotBusy()
    {
        using var core = new Core(Colour.White);
        
        Assert.False(core.IsBusy);
    }

    [Fact]
    public void ReportsQueueSize()
    {
        using var core = new Core(Colour.White);
        
        Assert.Equal(0, core.QueueSize);
    }

    [Fact]
    public void GetMoveFiresCallbackIfActionProvided()
    {
        using var core = new Core(Colour.White, Constants.InitialBoardFen);

        var called = false;

        core.GetMove(2, () => called = true);
        
        Thread.Sleep(100);
        
        Assert.True(called);
    }
}