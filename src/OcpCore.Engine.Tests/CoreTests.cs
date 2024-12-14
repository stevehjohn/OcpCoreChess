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

    [Fact]
    public void ReportsPerftResultsWhenEnabled()
    {
        var core = new Core(Colour.White, true);

        core.GetMove(2);

        foreach (var item in core.PerftData)
        {
            Assert.Equal(20, item.Value);
        }
    }
    [Fact]
    
    public void ReportsPerftResultsFromFenWhenEnabled()
    {
        var core = new Core(Colour.White, Constants.InitialBoardFen, true);

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