using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class RookTests
{
    private readonly Rook _rook = new(new Moves());
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/R7 w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_1111_1110)]
    [InlineData("8/8/8/3r4/8/8/8/8 b - - 0 1", 35,
        0b0000_1000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_1000_0000_1000)]
    public void MovesOrthogonallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.Is(Kind.Rook, position));

        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }

    [Theory]
    [InlineData("8/8/8/8/8/8/8/R4P1P w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0001_1110)]
    [InlineData("8/8/8/8/8/8/8/P1P4R w - - 0 1", 7,
        0b1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_0111_1000)]
    [InlineData("8/8/8/8/8/8/8/R2P4 w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0110)]
    [InlineData("8/8/8/1P1R3P/8/8/8/8 w - - 0 1", 35,
        0b0000_1000_0000_1000_0000_1000_0111_0100_0000_1000_0000_1000_0000_1000_0000_1000)]
    [InlineData("3P4/8/8/3R4/8/8/3P4/8 w - - 0 1", 35,
        0b0000_0000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.Is(Kind.Rook, position));

        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/R4p1P w - - 0 1", 0,
        0b0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0000_0001_0011_1110)]
    [InlineData("8/8/8/8/8/8/8/P2p3R w - - 0 1", 0,
        0b1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_1000_0000_0111_1000)]
    [InlineData("3P4/8/8/3R4/8/8/3p4/8 w - - 0 1", 35,
        0b0000_0000_0000_1000_0000_1000_1111_0111_0000_1000_0000_1000_0000_1000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);
    
        Assert.True(game.Is(Kind.Rook, position));
    
        var moves = _rook.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    //
    // [Theory]
    // [InlineData("8/8/8/P7/8/8/8/r7 b - - 0 1", 0, "1,2,3,4,5,6,7,8,16,24,32")]
    // [InlineData("8/8/8/8/8/8/8/r2P4 b - - 0 1", 0, "1,2,3,8,16,24,32,40,48,56")]
    // [InlineData("8/8/8/1P1r3P/8/8/8/8 b - - 0 1", 35, "33,34,36,37,38,39,59,51,43,27,19,11,3")]
    // [InlineData("3P4/8/8/3r4/8/8/3P4/8 b - - 0 1", 35, "32,33,34,36,37,38,39,59,51,43,27,19,11")]
    // public void TakesPieceOfOpposingColour(string fen, int position, string expectedMoves)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, Colour.Black);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, Colour.Black, moves);
    //     
    //     AssertExpectedMoves(expectedMoves, moves);
    // }
    //
    // [Theory]
    // [InlineData("8/8/8/P7/8/8/8/r7 b - - 0 1", 0, true, 32)]
    // [InlineData("8/8/8/p7/8/8/8/r7 b - - 0 1", 0, false, 0)]
    // public void ReportsCapturesCorrectly(string fen, int position, bool captureExpected, int captureCell)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, Colour.Black);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, Colour.Black, moves);
    //
    //     if (captureExpected)
    //     {
    //         Assert.Single(moves, m => m.Outcome == MoveOutcome.Capture && m.Target == captureCell);
    //     }
    //     else
    //     {
    //         Assert.DoesNotContain(moves, m => m.Outcome == MoveOutcome.Capture);
    //     }
    // }
}