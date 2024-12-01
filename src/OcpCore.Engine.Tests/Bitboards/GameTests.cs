using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.Bitboards;

public class GameTests
{
    private readonly Game _game = new();

    [Theory]
    [InlineData(Constants.InitialBoardFen, 
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000,
        0b00000000_11111111_00000000_00000000_00000000_00000000_11111111_00000000,
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000000_10000001,
        0b01000010_00000000_00000000_00000000_00000000_00000000_00000000_01000010,
        0b00100100_00000000_00000000_00000000_00000000_00000000_00000000_00100100,
        0b00001000_00000000_00000000_00000000_00000000_00000000_00000000_00001000,
        0b00010000_00000000_00000000_00000000_00000000_00000000_00000000_00010000)]
    public void ParsesFenCorrectly(string fen, ulong whitePlane, ulong blackPlane, ulong pawnPlane, ulong rookPlane, ulong knightPlane, ulong bishopPlane, ulong queenPlane, ulong kingPlane)
    {
        _game.ParseFen(fen);
        
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
    [InlineData(Constants.InitialBoardFen, 
        0b00000000_00000000_00000000_00000000_00000000_00000001_11111110_11111111,
        0b11111111_11111111_00000000_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnWhiteMoveCorrectly(string fen, ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(Kind.Pawn, Colour.White, 8, 16);
        
        Assert.Equal(whitePlane, _game[Plane.White]);
        
        Assert.Equal(blackPlane, _game[Plane.Black]);
    }
    
    [Theory]
    [InlineData(Constants.InitialBoardFen, 
        0b00000000_00000000_00000000_00000000_00000000_00000000_11111111_11111111,
        0b11111111_11111110_00000001_00000000_00000000_00000000_00000000_00000000)]
    public void UpdatesOnBlackMoveCorrectly(string fen, ulong whitePlane, ulong blackPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(Kind.Pawn, Colour.Black, 48, 40);
        
        Assert.Equal(whitePlane, _game[Plane.White]);
        
        Assert.Equal(blackPlane, _game[Plane.Black]);
    }

    [Theory]
    [InlineData(Constants.InitialBoardFen, 
        0b00000000_11111111_00000000_00000000_00000000_00000001_11111110_00000000)]
    public void UpdatesOnPawnMoveCorrectly(string fen, ulong pawnPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(Kind.Pawn, Colour.White, 8, 16);
        
        Assert.Equal(pawnPlane, _game[Plane.Pawn]);
    }
    
    [Theory]
    [InlineData(Constants.InitialBoardFen, 
        0b10000001_00000000_00000000_00000000_00000000_00000000_00000001_10000000)]
    public void UpdatesOnRookMoveCorrectly(string fen, ulong pawnPlane)
    {
        _game.ParseFen(fen);
        
        _game.MakeMove(Kind.Rook, Colour.White, 0, 8);
        
        Assert.Equal(pawnPlane, _game[Plane.Rook]);
    }
}