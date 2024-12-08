using OcpCore.Engine.Bitboards;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
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
    
    [Theory]
    [InlineData("8/8/8/8/8/b7/P7/8 b - - 0 1", 8,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("rnbqkbnr/1ppppppp/B7/8/8/p3P3/PPPPNPPP/RNBQ1RK1 b kq - 1 4", 8,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Pawn, position));

        var moves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/b7/P7/8 b - - 0 1", 8,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    public void IsBlockedByPieceOfOwnColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Pawn, position));

        var moves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/8/8/1p6/P7/8 w - - 0 1", 8,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0001_0000_0011_0000_0000_0000_0000)]
    public void TakesPieceOfOpposingColour(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Pawn, position));

        var moves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("8/8/8/1pP5/8/8/8/8 w KQkq b6 0 1", 34,
        0b0000_0000_0000_0000_0000_0110_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000)]
    [InlineData("rnbqkbnr/p1pppppp/8/8/1pP5/8/PP1PPPPP/RNBQKBNR b KQkq c3 0 1", 25,
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0110_0000_0000_0000_0000)]
    public void DetectsEnPassantOpportunity(string fen, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(fen);

        Assert.True(game.IsKind(Kind.Pawn, position));

        var moves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, moves);
    }
    
    [Theory]
    [InlineData("e2e3,a7a5,f1f6,a5a4,g1e2,a4a3,e1g1", 16, 
        0b0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0000_0010_0000_0000)]
    public void DoesNotMakeMistakesIdentifiedInPerfTesting(string movesString, int position, ulong expectedMoves)
    {
        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);

        var moves = movesString.Split(',');

        foreach (var move in moves)
        {
            game.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        }
        
        var pawnMoves = _pawn.GetMoves(game, position);
        
        Assert.Equal(expectedMoves, pawnMoves);
    }
}