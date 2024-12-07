using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using Xunit;

namespace OcpCore.Engine.Tests.Bitboards;

public class GameTests
{
    private readonly Game _game = new();

    [Theory]
    [InlineData(
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b00000000_11111111_00000000_00000000_00000000_00000000_11111111_00000000,
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_10000001,
        0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_01000010,
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00100100,
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00001000,
        0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00010000)]
    public void ParsesInitialFenCorrectly(ulong whitePlane, ulong blackPlane, ulong pawnPlane, ulong rookPlane, ulong knightPlane, ulong bishopPlane, ulong queenPlane, ulong kingPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);

        Assert.Equal(whitePlane, _game[Plane.White]);

        Assert.Equal(blackPlane, _game[Plane.Black]);

        Assert.Equal(pawnPlane, _game[Plane.Pawn]);

        Assert.Equal(rookPlane, _game[Plane.Rook]);

        Assert.Equal(knightPlane, _game[Plane.Knight]);

        Assert.Equal(bishopPlane, _game[Plane.Bishop]);

        Assert.Equal(queenPlane, _game[Plane.Queen]);

        Assert.Equal(kingPlane, _game[Plane.King]);
    }

    [Theory]
    [InlineData(
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b00000000_11111111_00000000_00000000_00000000_00000000_11111111_00000000,
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_10000001,
        0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_01000010,
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00100100,
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00001000,
        0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00010000)]
    public void CopiesSelfCorrectly(ulong whitePlane, ulong blackPlane, ulong pawnPlane, ulong rookPlane, ulong knightPlane, ulong bishopPlane, ulong queenPlane, ulong kingPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var copy = new Game(_game);

        Assert.Equal(whitePlane, copy[Plane.White]);

        Assert.Equal(blackPlane, copy[Plane.Black]);

        Assert.Equal(pawnPlane, copy[Plane.Pawn]);

        Assert.Equal(rookPlane, copy[Plane.Rook]);

        Assert.Equal(knightPlane, copy[Plane.Knight]);

        Assert.Equal(bishopPlane, copy[Plane.Bishop]);

        Assert.Equal(queenPlane, copy[Plane.Queen]);

        Assert.Equal(kingPlane, copy[Plane.King]);
    }
    
    [Theory]
    [InlineData( 
        0b00000000_00000000_00000000_00000000_00000000_00000001_11111110_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnWhiteMoveCorrectly(ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(8, 16);
        
        Assert.Equal(whitePlane, _game[Plane.White]);
        
        Assert.Equal(blackPlane, _game[Plane.Black]);
    }
    
    [Theory]
    [InlineData( 
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111110_00000001_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnBlackMoveCorrectly(ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1");
        
        _game.MakeMove(48, 40);
        
        Assert.Equal(whitePlane, _game[Plane.White]);
        
        Assert.Equal(blackPlane, _game[Plane.Black]);
    }

    [Theory]
    [InlineData( 
        0b00000000_11111111_00000000_00000000_00000000_00000001_11111110_00000000)]
    public void UpdatesOnPawnMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(8, 16);
        
        Assert.Equal(expectedPlane, _game[Plane.Pawn]);
    }
    
    [Theory]
    [InlineData( 
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000001_10000000)]
    public void UpdatesOnRookMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(0, 8);
        
        Assert.Equal(expectedPlane, _game[Plane.Rook]);
    }
    
    [Theory]
    [InlineData( 
        0b01000010_00000000_00000000_00000000_00000000_00000000_00001000_01000000)]
    public void UpdatesOnKnightMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(1, 11);
        
        Assert.Equal(expectedPlane, _game[Plane.Knight]);
    }
    
    [Theory]
    [InlineData(
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000010_00100000)]
    public void UpdatesOnBishopMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(2, 9);
        
        Assert.Equal(expectedPlane, _game[Plane.Bishop]);
    }
    
    [Theory]
    [InlineData( 
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00000001)]
    public void UpdatesOnQueenMoveCorrectly(ulong expectedPlane)
    {
        _game.ParseFen(Constants.InitialBoardFen);
        
        _game.MakeMove(3, 0);
        
        Assert.Equal(expectedPlane, _game[Plane.Queen]);
    }    
    
    [Theory]
    [InlineData("rnbqkbnr/p1p3pp/1p1pp3/5p2/3P1B2/2NQ4/PPP1PPPP/R3KBNR w KQkq - 0 5", "e1c1", Colour.White,
        0b00000000_00000000_00000000_00000000_00101000_00001100_11110111_11101100)]
    [InlineData("rnbqkbnr/ppp3pp/3p1p2/4p3/8/5NPB/PPPPPP1P/RNBQK2R w KQkq - 0 4", "e1g1", Colour.White,
        0b00000000_00000000_00000000_00000000_00000000_11100000_10111111_01101111)]
    [InlineData("rnbqk2r/pppp1ppp/3bp2n/8/8/2PP1P1P/PP2P1P1/RNBQKBNR b KQkq - 0 4", "e8g8", Colour.Black,
        0b01101111_11101111_10011000_00000000_00000000_00000000_00000000_00000000)]
    [InlineData("r3kbnr/pppqpppp/2npb3/8/8/3PPPPP/PPP5/RNBQKBNR b KQkq - 0 5", "e8c8", Colour.Black,
        0b11101100_11111111_00011100_00000000_00000000_00000000_00000000_00000000)]
    public void PerformsCastleCorrectly(string fen, string move, Colour colour, ulong expectedPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        
        Assert.Equal(expectedPlane, _game[(Plane) colour]);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/8/p7/1K6 w - - 0 1", Colour.White)]
    [InlineData("8/8/8/8/8/8/2p5/1K6 w - - 0 1", Colour.White)]
    [InlineData("1k6/P7/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("1k6/2P5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("1k6/2K5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/7B b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/7Q b - - 0 1", Colour.Black)]
    [InlineData("k7/1p6/8/8/8/8/8/7B b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/2P5/8/8/8/8/7B b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/8/8/8/8/8/R7 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/8/8/8/Q7 b - - 0 1", Colour.Black)]
    [InlineData("k7/8/8/8/p7/8/8/R7 b - - 0 1", Colour.Black, false)]
    [InlineData("k7/8/8/8/P7/8/8/R7 b - - 0 1", Colour.Black, false)]
    [InlineData("k7/2N5/8/8/8/8/8/8 b - - 0 1", Colour.Black)]
    public void ReportsKingInCheckCorrectly(string fen, Colour colour, bool check = true)
    {
        _game.ParseFen(fen);

        if (check)
        {
            Assert.True(_game.IsKingInCheck((Plane) colour));
        }
        else
        {
            Assert.False(_game.IsKingInCheck((Plane) colour));
        }
    }

    [Fact]
    public void ThrowsExceptionOnMoveIfNoPieceInFromCell()
    {
        _game.ParseFen(Constants.InitialBoardFen);

        var exception = Assert.Throws<InvalidMoveException>(() => _game.MakeMove(16, 17));
        
        Assert.Equal("No piece at a3.", exception.Message);
    }
}