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
    [InlineData("8/8/8/3K4/8/8/8/8 w - - 0 1", 35,
        0b0000_0000_0000_0000_0001_1100_0001_0100_0001_1100_0000_0000_0000_0000_0000_0000)]
    public void MovesDirectionallyAsExpectedOnEmptyBoard(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.King, position));

        var moves = _king.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }    
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/KP6 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_0000_0000)]
    [InlineData("8/8/3K4/3K4/8/8/8/8 w - - 0 1", 35,
        0b0000_0000_0000_0000_0001_0100_0001_0100_0001_1100_0000_0000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.King, position));

        var moves = _king.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/8/KP6 w - - 0 1", 0,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0011_0000_0000)]
    [InlineData("8/8/3k4/3K4/8/8/8/8 w - - 0 1", 35,
        0b0000_0000_0000_0000_0001_1100_0001_0100_0001_1100_0000_0000_0000_0000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.King, position));

        var moves = _king.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("rnbqk2B/ppppnp1p/4p3/8/8/bP6/P1PPPPPP/RN1QKBNR b KQq - 0 4", "e8g8")]
    public void NoFalsePositiveCastling(string fen, string excludedMove)
    {
        var core = new Core(Colour.White, fen);
    
        var moves = core.GetAllowedMoves();
    
        foreach (var move in moves)
        {
            Assert.False(move[..2] == excludedMove[..2] && move[2..] == excludedMove[2..]);
        }
    }
    
    [Theory]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPPP/RNBQK2R w KQkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0110_0000)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPqP/RNBQK2R w KQkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPPP/RNBQK2R w Qkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1100)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPqPPPPP/R3KBNR w KQkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000)]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/8/PPPPPPPP/R3KBNR w Kkq - 0 1", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_1000)]
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60,
        0b0000_1100_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQk - 0 1", 60,
        0b0000_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("1N2k2r/p1ppqpb1/bn2pnp1/3P4/1p2P3/2N2Q1p/PPPBBPPP/R3K2R b KQk - 0 2", 60,
        0b0110_1000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("r3k2r/p1ppqpb1/1n2pnp1/3PN3/1p2P3/P1N2Q1p/1PPB1PPP/3bK2R w Kkq - 0 3", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0110_1000)]
    [InlineData("r3k2r/2ppqpb1/1n2pnp1/pb1PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w Kkq - 0 3", 4,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0110_1000)]
    public void DetectsCastlingOpportunity(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.King, position));

        var moves = _king.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }    
}