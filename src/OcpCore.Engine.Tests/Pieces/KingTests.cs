using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Pieces;

public class KingTests
{
    private readonly King _king = new();
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/K7 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_0000_0010)]
    // [InlineData("8/8/8/3K4/8/8/8/8 w - - 0 1", 35,
    //     0b0000_0000_0001_0100_0010_0010_0000_0000_0010_0010_0001_0100_0000_0000_0000_0000)]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.King, position));

        var moves = _king.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    // [Theory]
    // [InlineData("8/8/8/8/8/8/p7/k7 b - - 0 1", 0, "9,1")]
    // [InlineData("8/8/8/8/8/8/1p6/k7 b - - 0 1", 0, "8,1")]
    // [InlineData("8/8/8/8/8/8/8/kp6 b - - 0 1", 0, "8,9")]
    // [InlineData("8/8/2pp4/3k4/8/8/8/8 b - - 0 1", 35, "44,34,36,26,27,28")]
    // [InlineData("8/8/8/3kp3/4p3/8/8/8 b - - 0 1", 35, "42,43,44,34,26,27")]
    // public void IsBlockedByPieceOfOwnColour(string fen, int position, string expectedMoves)
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
    // [InlineData("8/8/8/8/8/8/P7/k7 b - - 0 1", 0, "8,9,1")]
    // [InlineData("8/8/8/8/8/8/1P6/k7 b - - 0 1", 0, "8,9,1")]
    // [InlineData("8/8/8/8/8/8/8/kP6 b - - 0 1", 0, "8,9,1")]
    // [InlineData("8/8/2PP4/3k4/8/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
    // [InlineData("8/8/8/3kP3/4P3/8/8/8 b - - 0 1", 35, "42,43,44,34,36,26,27,28")]
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
    // [InlineData("r3kbnr/ppp1pppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, Colour.Black, "58,59,51")]
    // [InlineData("r3k2r/ppp1pp1p/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, Colour.Black, "58,59,51,61,62")]
    // [InlineData("r3k2r/ppp1pp1p/8/8/8/8/PPPPPPPP/RNBQKBNR b KQq - 0 1", 60, Colour.Black, "58,59,51,61")]
    // [InlineData("r3k2r/ppp1pp1p/8/8/8/8/PPPPPPPP/RNBQKBNR b KQk - 0 1", 60, Colour.Black, "59,51,61,62")]
    // [InlineData("r3k2r/ppp1pp1p/8/8/8/8/PPPPPPPP/RNBQKBNR b KQ - 0 1", 60, Colour.Black, "59,51,61")]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/RNBQK2R w KQkq - 0 1", 4, Colour.White, "5,6,12")]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/R3K2R w KQkq - 0 1", 4, Colour.White, "5,6,12,3,2")]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/RNBQK2R w Qkq - 0 1", 4, Colour.White, "5,12")]
    // [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/R3K2R w Kkq - 0 1", 4, Colour.White, "3,5,6,12")]
    // public void DetectsCastlingOpportunity(string fen, int position, Colour colour, string expectedMoves)
    // {
    //     var board = new Board(fen);
    //
    //     AssertPieceIsWhereExpected(board, position, colour);
    //
    //     var moves = new List<Move>();
    //     
    //     Piece.GetMoves(board, position, colour, moves);
    //     
    //     AssertExpectedMoves(expectedMoves, moves);
    // }
    //
    // [Theory]
    // [InlineData("rnbqk2B/ppppnp1p/4p3/8/8/bP6/P1PPPPPP/RN1QKBNR b KQkq - 0 4", "e8g8")]
    // public void NoFalsePositiveCastling(string fen, string excludedMove)
    // {
    //     var core = new Core(Colour.White, fen);
    //
    //     var moves = core.GetAllowedMoves();
    //
    //     foreach (var move in moves)
    //     {
    //         Assert.False(move.Position == excludedMove[..2].FromStandardNotation() && move.Target == excludedMove[2..].FromStandardNotation());
    //     }
    // }
}