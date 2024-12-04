using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class PawnTests
{
    private readonly Pawn _pawn = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/P7/8 w - - 0 1", 8,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0001_0000_0000_0000_0000)]
    [InlineData("8/8/8/8/8/P7/8/8 w - - 0 1", 16,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0000_0000_0000_0000_0000)]
    [InlineData("8/P7/8/8/8/8/8/8 w - - 0 1", 48,
        0b0000_0001_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    public void MovesAccordingToPawnRulesOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Pawn, position));

        var moves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    // [InlineData("8/8/8/8/8/8/P7/8 w - - 0 1", 8, Colour.White, "16,24")]
    // [InlineData("8/p7/8/8/8/8/p7/8 b - - 0 1", 48, Colour.Black, "40,32")]
    // [InlineData("8/8/8/8/8/8/2P5/8 w - - 0 1", 10, Colour.White, "18,26")]
    // [InlineData("8/2p5/8/8/8/8/8/8 b - - 0 1", 50, Colour.Black, "42,34")]
    // [InlineData("8/8/8/8/8/P7/8/8 b - - 0 1", 16, Colour.White, "24")]
    // [InlineData("8/8/p7/8/8/8/8/8 b - - 0 1", 40, Colour.Black, "32")]
    // [InlineData("8/8/8/8/8/2P5/8/8 b - - 0 1", 18, Colour.White, "26")]
    // [InlineData("8/8/2p5/8/8/8/8/8 b - - 0 1", 42, Colour.Black, "34")]
    // public void MovesAccordingToPawnRulesOnEmptyBoard(string fen, int position, Colour colour, string expectedMoves)

    // [InlineData("8/8/8/8/P7/8/P7/8 w - - 0 1", 8, Colour.White, "16")]
    // [InlineData("8/p7/8/p7/8/8/8/8 b - - 0 1", 48, Colour.Black, "40")]
    // [InlineData("8/8/8/8/2P5/8/2P5/8 w - - 0 1", 10, Colour.White, "18")]
    // [InlineData("8/2p5/8/2p5/8/8/8/8 b - - 0 1", 50, Colour.Black, "42")]
    // [InlineData("8/8/8/8/8/P7/P7/8 w - - 0 1", 8, Colour.White, null)]
    // [InlineData("8/p7/p7/8/8/8/8/8 b - - 0 1", 48, Colour.Black, null)]
    // [InlineData("8/8/8/8/8/2P5/2P5/8 w - - 0 1", 10, Colour.White, null)]
    // [InlineData("8/2p5/2p5/8/8/8/8/8 b - - 0 1", 50, Colour.Black, null)]
    // public void IsBlockedByPieceOfOwnColour(string fen, int position, Colour colour, string expectedMoves)

    // [InlineData("8/8/8/8/p7/8/P7/8 w - - 0 1", 8, Colour.White, "16")]
    // [InlineData("8/p7/8/P7/8/8/8/8 b - - 0 1", 48, Colour.Black, "40")]
    // [InlineData("8/8/8/8/2p5/8/2P5/8 w - - 0 1", 10, Colour.White, "18")]
    // [InlineData("8/2p5/8/2P5/8/8/8/8 b - - 0 1", 50, Colour.Black, "42")]
    // [InlineData("8/8/8/8/8/p7/P7/8 w - - 0 1", 8, Colour.White, null)]
    // [InlineData("8/p7/P7/8/8/8/8/8 b - - 0 1", 48, Colour.Black, null)]
    // [InlineData("8/8/8/8/8/2p5/2P5/8 w - - 0 1", 10, Colour.White, null)]
    // [InlineData("8/2p5/2P5/8/8/8/8/8 b - - 0 1", 50, Colour.Black, null)]
    // public void IsBlockedByPieceOfOOpposingColour(string fen, int position, Colour colour, string expectedMoves)

    // [InlineData("8/8/8/8/8/p7/1P6/8 w - - 0 1", 9, Colour.White, "16,17,25")]
    // [InlineData("8/8/8/8/8/2p5/1P6/8 w - - 0 1", 9, Colour.White, "18,17,25")]
    // [InlineData("8/1p6/2P5/8/8/8/8/8 b - - 0 1", 49, Colour.Black, "42,41,33")]
    // [InlineData("8/1p6/P7/8/8/8/8/8 b - - 0 1", 49, Colour.Black, "40,41,33")]
    // public void TakesPieceOfOpposingColour(string fen, int position, Colour colour, string expectedMoves)

    // [InlineData("8/8/8/1pP5/8/8/8/8 w KQkq b6 0 1", 34, Colour.White, "41,42")]
    // [InlineData("8/8/8/2Pp4/8/8/8/8 w KQkq d6 0 1", 34, Colour.White, "42,43")]
    // [InlineData("8/8/8/8/5pP1/8/8/8 b KQkq g3 0 1", 29, Colour.Black, "21,22")]
    // [InlineData("8/8/8/8/4Pp2/8/8/8 b KQkq e3 0 1", 29, Colour.Black, "20,21")]
    // [InlineData("8/8/8/1pP5/8/8/8/8 w KQkq - 0 1", 34, Colour.White, "42")]
    // [InlineData("8/8/8/2Pp4/8/8/8/8 w KQkq - 0 1", 34, Colour.White, "42")]
    // [InlineData("8/8/8/8/5pP1/8/8/8 b KQkq - 0 1", 29, Colour.Black, "21")]
    // [InlineData("8/8/8/8/4Pp2/8/8/8 b KQkq - 0 1", 29, Colour.Black, "21")]
    // public void DetectsEnPassantOpportunity(string fen, int position, Colour colour, string expectedMoves)

    // [InlineData("8/8/8/8/8/p7/1P6/8 w - - 0 1", 9, true, 16)]
    // [InlineData("8/8/8/8/8/2p5/1P6/8 w - - 0 1", 9, true, 18)]
    // [InlineData("8/8/8/8/8/1p6/1P6/8 w - - 0 1", 9, false, 0)]
    // public void ReportsCapturesCorrectly(string fen, int position, bool captureExpected, int captureCell)
    // {
}