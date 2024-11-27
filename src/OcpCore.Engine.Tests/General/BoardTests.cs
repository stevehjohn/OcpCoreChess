using OcpCore.Engine.Exceptions;
using OcpCore.Engine.General;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class BoardTests
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/9 b - - 0 1", "Too many files in rank 1: 9.")]
    [InlineData("ppppppppp/8/8/8/8/8/8/8 b - - 0 1", "Too many files in rank 8: ppppppppp.")]
    [InlineData("8/8/p7p/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: p7p.")]
    [InlineData("8/8/pp7/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: pp7.")]
    [InlineData("pppppppp/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 7.")]
    [InlineData("pppppppp/8/8/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 9.")]
    [InlineData("pppppppp/8/8/8/8/8/8/ppgppppp w - - 0 1", "Invalid piece token in rank 1: g.")]
    [InlineData("pppppppp/8/8/8/8/8/8/pppppppp - - 0 1", "Invalid number of parts to FEN string: 5.")]
    public void BoardDetectsInvalidFenSituations(string fen, string expectedMessage)
    {
        var exception =Assert.Throws<FenParseException>(() => new Board(fen));
        
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, Castle.All, Castle.All)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQkq - 0 1", 0, 16, Castle.All, Castle.All ^ Castle.WhiteQueenSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNBQKBNR w KQkq - 0 1", 7, 23, Castle.All, Castle.All ^ Castle.WhiteKingSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1", 4, 12, Castle.All, Castle.BlackQueenSide | Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/1ppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 56, 40, Castle.All, Castle.All ^ Castle.BlackQueenSide)]
    [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 63, 55, Castle.All, Castle.All ^ Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 60, 52, Castle.All, Castle.WhiteQueenSide | Castle.WhiteKingSide)]
    public void BoardUpdatesCastlingRightsOnMove(string fen, int position, int target, Castle rightsBeforeMove, Castle rightsAfterMove)
    {
        var board = new Board(fen);
        
        Assert.Equal(rightsBeforeMove, board.State.CastleStatus);
        
        board.MakeMove(position, target);
        
        Assert.Equal(rightsAfterMove, board.State.CastleStatus);
    }
    
    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 24, 16)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 10, 26, 18)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 48, 40, null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 48, 32, 40)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 50, 34, 42)]
    public void BoardUpdatesEnPassantCellOnMove(string fen, int position, int target, int? enPassantCell)
    {
        var board = new Board(fen);
        
        Assert.Null(board.State.EnPassantTarget);
        
        board.MakeMove(position, target);
        
        Assert.Equal(enPassantCell, board.State.EnPassantTarget);
    }
}