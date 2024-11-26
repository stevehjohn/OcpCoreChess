using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class PawnTests : PieceTestBase<Pawn>
{
    [Theory]
    [InlineData(8, Colour.White, "16,24")]
    [InlineData(48, Colour.Black, "40,32")]
    [InlineData(10, Colour.White, "18,26")]
    [InlineData(50, Colour.Black, "42,34")]
    [InlineData(16, Colour.White, "24")]
    [InlineData(40, Colour.Black, "32")]
    [InlineData(18, Colour.White, "26")]
    [InlineData(42, Colour.Black, "34")]
    public void MovesAccordingToPawnRulesOnEmptyBoard(int position, Colour colour, string expectedMoves)
    {
        var board = new Board();

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, colour, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/P7/8/P7/8 w - - 0 1", 8, Colour.White, "16")]
    [InlineData("8/p7/8/p7/8/8/8/8 b - - 0 1", 48, Colour.Black, "40")]
    [InlineData("8/8/8/8/2P5/8/2P5/8 w - - 0 1", 10, Colour.White, "18")]
    [InlineData("8/2p5/8/2p5/8/8/8/8 b - - 0 1", 50, Colour.Black, "42")]
    [InlineData("8/8/8/8/8/P7/P7/8 w - - 0 1", 8, Colour.White, null)]
    [InlineData("8/p7/p7/8/8/8/8/8 b - - 0 1", 48, Colour.Black, null)]
    [InlineData("8/8/8/8/8/2P5/2P5/8 w - - 0 1", 10, Colour.White, null)]
    [InlineData("8/2p5/2p5/8/8/8/8/8 b - - 0 1", 50, Colour.Black, null)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, Colour colour, string expectedMoves)
    {
        var board = new Board(fen);

        var piece = Piece;

        var moves = new List<Move>();
        
        piece.GetMoves(board, position, colour, moves);
        
        AssertExpectedMoves(expectedMoves, moves);
    }

}