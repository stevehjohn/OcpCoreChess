using OcpCore.Engine.Bitboards;
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
        _game.ParseFen(Constants.InitialBoardFen);
        
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
}