using OcpCore.Engine.Exceptions;
using OcpCore.Engine.Extensions;
using OcpCore.Engine.General;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Pieces;
using Xunit;

namespace OcpCore.Engine.Tests.General;

public class BoardTests
{
    [Theory]
    [InlineData("8/8/8/8/8/8/8/9 b - - 0 1", "Too many files in rank 1: 9.")]
    [InlineData("ppppppppp/8/8/8/8/8/8/8 b - - 0 1", "Too many files in rank 8: ppppppppp.")]
    [InlineData("8/8/p7p/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: p7p.")]
    [InlineData("8/8/pp7/8/8/8/8/8 w - - 0 1", "Too many files in rank 6: pp7.")]
    [InlineData("8/8/pp5/8/8/8/8/8 w - - 0 1", "Not enough files in rank 6: pp5.")]
    [InlineData("pppppppp/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 7.")]
    [InlineData("pppppppp/8/8/8/8/8/8/8/8 w - - 0 1", "Incorrect number of ranks in FEN string: 9.")]
    [InlineData("pppppppp/8/8/8/8/8/8/ppgppppp w - - 0 1", "Invalid piece token in rank 1: g.")]
    [InlineData("pppppppp/8/8/8/8/8/8/pppppppp - - 0 1", "Invalid number of parts to FEN string: 5.")]
    [InlineData("pppppppp/8/8/8/8/8/8/pppppppp x - - 0 1", "Invalid turn indicator: x.")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w XQkq - 0 1", "Invalid castling status indicator: X.")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - Y 1", "Invalid value for halfmove counter: Y.")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 Z", "Invalid value for fullmove counter: Z.")]
    public void BoardDetectsInvalidFenSituations(string fen, string expectedMessage)
    {
        var exception = Assert.Throws<FenParseException>(() => new Board(fen));

        Assert.Equal(expectedMessage, exception.Message);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, Castle.All, Castle.All)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/1PPPPPPP/RNBQKBNR w KQkq - 0 1", 0, 16, Castle.All, Castle.All ^ Castle.WhiteQueenSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPP1/RNBQKBNR w KQkq - 0 1", 7, 23, Castle.All, Castle.All ^ Castle.WhiteKingSide)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPP1PPP/RNBQKBNR w KQkq - 0 1", 4, 12, Castle.All, Castle.BlackQueenSide | Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/1ppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 56, 40, Castle.All, Castle.All ^ Castle.BlackQueenSide)]
    [InlineData("rnbqkbnr/ppppppp1/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 63, 55, Castle.All, Castle.All ^ Castle.BlackKingSide)]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, 52, Castle.All, Castle.WhiteQueenSide | Castle.WhiteKingSide)]
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
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 48, 40, null)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 48, 32, 40)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 50, 34, 42)]
    public void BoardUpdatesEnPassantCellOnMove(string fen, int position, int target, int? enPassantCell)
    {
        var board = new Board(fen);

        Assert.Null(board.State.EnPassantTarget);

        board.MakeMove(position, target);

        Assert.Equal(enPassantCell, board.State.EnPassantTarget);
    }

    [Theory]
    [InlineData("r3kbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, 58, "2kr1bnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 1 2")]
    [InlineData("r3k2r/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 60, 62, "r4rk1/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQ - 1 2")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3KBNR w KQkq - 0 1", 4, 2, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/2KR1BNR b kq - 1 1")]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R3K2R w KQkq - 0 1", 4, 6, "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/R4RK1 b kq - 1 1")]
    public void BoardPerformsCastle(string fen, int position, int target, string expectedFem)
    {
        var board = new Board(fen);

        board.MakeMove(position, target);

        Assert.Equal(expectedFem, board.Fen());
    }

    [Theory]
    [InlineData("rnbqkbnr/pppp1ppp/8/4pP2/8/8/PPPPP1PP/RNBQKBNR w KQkq e6 0 1", 37, 44, "rnbqkbnr/pppp1ppp/4P3/8/8/8/PPPPP1PP/RNBQKBNR b KQkq - 0 1")]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b KQkq f3 0 1", 28, 21, "rnbqkbnr/pppp1ppp/8/8/8/5p2/PPPPP1PP/RNBQKBNR w KQkq - 0 2")]
    public void BoardPerformsEnPassant(string fen, int position, int target, string expectedFem)
    {
        var board = new Board(fen);

        board.MakeMove(position, target);

        Assert.Equal(expectedFem, board.Fen());
    }

    [Theory]
    [InlineData("rnbqkbnr/ppp1pppp/8/8/8/p7/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 16, Scores.Initial, Scores.Initial - Scores.Pawn)]
    [InlineData("1nbqkbnr/pppppppp/8/8/8/r7/PPPPPPPP/RNBQKBNR w KQk - 0 1", 9, 16, Scores.Initial, Scores.Initial - Scores.Rook)]
    [InlineData("r1bqkbnr/pppppppp/8/8/8/n7/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 16, Scores.Initial, Scores.Initial - Scores.Knight)]
    [InlineData("rn1qkbnr/pppppppp/8/8/8/b7/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 16, Scores.Initial, Scores.Initial - Scores.Bishop)]
    [InlineData("rnb1kbnr/pppppppp/8/8/8/q7/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 9, 16, Scores.Initial, Scores.Initial - Scores.Queen)]
    [InlineData("rnbqkbnr/pppppppp/P7/8/8/8/1PPPPPPP/RNBQKBNR b KQkq - 0 1", 49, 40, Scores.Initial - Scores.Pawn, Scores.Initial)]
    [InlineData("rnbqkbnr/pppppppp/R7/8/8/8/PPPPPPPP/1NBQKBNR b Kkq - 0 1", 49, 40, Scores.Initial - Scores.Rook, Scores.Initial)]
    [InlineData("rnbqkbnr/pppppppp/N7/8/8/8/PPPPPPPP/R1BQKBNR b KQkq - 0 1", 49, 40, Scores.Initial - Scores.Knight, Scores.Initial)]
    [InlineData("rnbqkbnr/pppppppp/B7/8/8/8/PPPPPPPP/RN1QKBNR b KQkq - 0 1", 49, 40, Scores.Initial - Scores.Bishop, Scores.Initial)]
    [InlineData("rnbqkbnr/pppppppp/Q7/8/8/8/PPPPPPPP/RNB1KBNR b KQkq - 0 1", 49, 40, Scores.Initial - Scores.Queen, Scores.Initial)]
    public void BoardUpdatesScoresOnCapture(string fen, int position, int target, int expectedWhiteScore, int expectedBlackScore)
    {
        var board = new Board(fen);

        Assert.Equal(Scores.Initial, board.State.WhiteScore);

        Assert.Equal(Scores.Initial, board.State.BlackScore);

        board.MakeMove(position, target);

        Assert.Equal(expectedWhiteScore, board.State.WhiteScore);

        Assert.Equal(expectedBlackScore, board.State.BlackScore);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppp1ppp/8/4pP2/8/8/PPPPP1PP/RNBQKBNR w KQkq e6 0 1", 37, 44, Scores.Initial - Scores.Pawn, Scores.Initial)]
    [InlineData("rnbqkbnr/pppp1ppp/8/8/4pP2/8/PPPPP1PP/RNBQKBNR b KQkq f3 0 1", 28, 21, Scores.Initial, Scores.Initial - Scores.Pawn)]
    public void BoardUpdatesScoresOnEnPassant(string fen, int position, int target, int expectedWhiteScore, int expectedBlackScore)
    {
        var board = new Board(fen);

        Assert.Equal(Scores.Initial, board.State.WhiteScore);

        Assert.Equal(Scores.Initial, board.State.BlackScore);

        board.MakeMove(position, target);

        Assert.Equal(expectedWhiteScore, board.State.WhiteScore);

        Assert.Equal(expectedBlackScore, board.State.BlackScore);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", 8, 16, Colour.Black)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", 48, 40, Colour.White)]
    public void BoardChangesPlayerAfterMove(string fen, int position, int target, Colour expectedPlayer)
    {
        var board = new Board(fen);

        board.MakeMove(position, target);

        Assert.Equal(expectedPlayer, board.State.Player);
    }

    [Theory]
    [InlineData("rnbqk1nr/1pp2ppp/8/3p4/2Pb2P1/3K1N2/1q2PPBP/7R b kq - 3 16", "rnbqk1nr/1pp2ppp/8/3p4/2Pb2P1/3K1N2/1q2PPBP/7R b kq - 3 16", 16, 46, Castle.BlackKingSide | Castle.BlackQueenSide, Colour.Black)]
    [InlineData("3k4/p1p2p1p/p2p2p1/3P4/2K5/7r/2bb4/8 b - - 1 26", "3k4/p1p2p1p/p2p2p1/3P4/2K5/7r/2bb4/8 b - - 1 26", 1, 18, Castle.NotAvailable, Colour.Black)]
    public void BoardCopiesCorrectly(string fen, string expectedBoard, int expectedWhiteScore, int expectedBlackScore, Castle expectedCastleStatus, Colour expectedPlayer)
    {
        var board = new Board(fen);

        Assert.Equal(expectedWhiteScore, board.State.WhiteScore);

        Assert.Equal(expectedBlackScore, board.State.BlackScore);

        Assert.Equal(expectedCastleStatus, board.State.CastleStatus);

        Assert.Equal(expectedPlayer, board.State.Player);

        var copy = new Board(board);

        Assert.Equal(expectedBoard, copy.Fen());

        Assert.Equal(expectedWhiteScore, copy.State.WhiteScore);

        Assert.Equal(expectedBlackScore, copy.State.BlackScore);

        Assert.Equal(expectedCastleStatus, copy.State.CastleStatus);

        Assert.Equal(expectedPlayer, board.State.Player);
    }

    [Theory]
    [InlineData("rnbqkbnr/p1pppppp/1p6/8/P7/4P3/1PPP1PPP/RNBQKBNR b KQkq - 1 3", "c8a6", "e1e2")]
    // [InlineData("rn1qkbnr/2pppppp/bp6/p7/8/4PN2/PPPP1PPP/RNBQK2R w KQkq - 0 4", "e1e2")]
    // [InlineData("k7/8/4N3/8/8/8/8/8 w - - 0 1", "e6c7")]
    public void BoardDetectsCheckAfterMove(string fen, string move, string kingMove)
    {
        var board = new Board(fen);
        
        Assert.False(board.IsKingInCheck(Colour.Black));
    
        board.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
    
        board.MakeMove(kingMove[..2].FromStandardNotation(), kingMove[2..].FromStandardNotation());
    
        Assert.True(board.IsKingInCheck(Colour.Black));
    }
    
    [Theory]
    [InlineData("rnbqkbnr/ppp1pppp/3p4/8/8/5P1N/PPPPP1PP/RNBQKB1R b KQkq - 0 1", "c8h3")]
    public void BoardDoesNotDetectsFalseCheckAfterMove(string fen, string move)
    {
        var board = new Board(fen);
        
        Assert.False(board.IsKingInCheck(Colour.White));

        board.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());

        Assert.False(board.IsKingInCheck(Colour.White));
    }

    
    [Theory]
    [InlineData("8/8/8/3K4/8/4n3/8/8 w - - 0 1", true)]
    [InlineData("8/8/8/3K4/5n2/8/8/8 w - - 0 1", true)]
    [InlineData("8/8/5n2/3K4/8/8/8/8 w - - 0 1", true)]
    [InlineData("8/4n3/8/3K4/8/8/8/8 w - - 0 1", true)]
    [InlineData("8/8/8/3K4/8/2n5/8/8 w - - 0 1", true)]
    [InlineData("8/8/8/3K4/1n6/8/8/8 w - - 0 1", true)]
    [InlineData("8/8/1n6/3K4/8/8/8/8 w - - 0 1", true)]
    [InlineData("8/2n5/8/3K4/8/8/8/8 w - - 0 1", true)]
    [InlineData("K7/2n5/8/8/8/8/8/8 w - - 0 1", true)]
    [InlineData("K7/8/1n6/8/8/8/8/8 w - - 0 1", true)]
    [InlineData("K7/8/2n5/8/8/8/8/8 w - - 0 1", false)]
    [InlineData("K7/2N5/8/8/8/8/8/8 w - - 0 1", false)]
    public void BoardDetectsCheck(string fen, bool isCheck)
    {
        var board = new Board(fen);

        Assert.Equal(isCheck, board.IsKingInCheck(Colour.White));
    }

    [Theory]
    [InlineData("8/4P3/8/8/8/8/8/8 w - - 0 1", 52, 60, 1, 0, 9, 0)]
    [InlineData("5p2/4P3/8/8/8/8/8/8 w - - 0 1", 52, 61, 1, 1, 9, 0)]
    [InlineData("8/8/8/8/8/8/4p3/8 b - - 0 1", 12, 4, 0, 1, 0, 9)]
    [InlineData("8/8/8/8/8/8/4p3/5P2 b - - 0 1", 12, 5, 1, 1, 0, 9)]
    public void BoardPromotesPawns(string fen, int position, int target, int whiteScore, int blackScore, int newWhiteScore, int newBlackScore)
    {
        var board = new Board(fen);

        var piece = board[position];

        Assert.True(Cell.Is(piece, Kind.Pawn));

        Assert.Equal(whiteScore, board.State.WhiteScore);

        Assert.Equal(blackScore, board.State.BlackScore);

        board.MakeMove(position, target);

        piece = board[target];

        Assert.True(Cell.Is(piece, Kind.Queen));

        Assert.Equal(newWhiteScore, board.State.WhiteScore);

        Assert.Equal(newBlackScore, board.State.BlackScore);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
    [InlineData("rnb2b1k/p1p1p1pp/5Rp1/PppqN2n/4rP1Q/B6B/PP1PP1PP/RN2K3 w Qq b6 5 12", "rnb2b1k/p1p1p1pp/5Rp1/PppqN2n/4rP1Q/B6B/PP1PP1PP/RN2K3 w Qq b6 5 12")]
    public void BoardGeneratesCorrectFen(string input, string expected)
    {
        var board = new Board(input);

        var fen = board.Fen();

        Assert.Equal(expected, fen);
    }

    [Theory]
    [InlineData("b1c3", 1, 1)]
    [InlineData("b1c3,g8f6", 2, 2)]
    [InlineData("b1c3,g8f6,g1f3", 3, 2)]
    [InlineData("b1c3,g8f6,g1f3,a7a6", 0, 3)]
    [InlineData("a6b4", 0, 6, "r2qkb1r/ppp1pppp/n2p1n2/5b2/1Q1P4/7N/PPPKPPPP/RNB2B1R b kq - 5 5")]
    public void BoardCountsMovesCorrectly(string moveString, int expectedHalfMoves, int expectedFullmoves, string fen = null)
    {
        var board = new Board(fen ?? Constants.InitialBoardFen);

        var moves = moveString.Split(',');

        foreach (var move in moves)
        {
            board.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        }

        Assert.Equal(expectedHalfMoves, board.State.Halfmoves);

        Assert.Equal(expectedFullmoves, board.State.Fullmoves);
    }

    [Theory]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "a7a6", true)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", "a2a3", false)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", "a7a6", false)]
    [InlineData("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1", "a2a3", true)]
    public void BoardPreventsMoveByWrongPlayer(string fen, string move, bool exceptionExpected)
    {
        var board = new Board(fen);

        if (exceptionExpected)
        {
            var exception = Assert.Throws<InvalidMoveException>(() => board.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation()));

            Assert.Contains("Not the turn", exception.Message);
        }
        else
        {
            board.MakeMove(move[..2].FromStandardNotation(), move[2..].FromStandardNotation());
        }
    }

    [Fact]
    public void BoardPreventsEmptyCellMove()
    {
        var board = new Board(Constants.InitialBoardFen);

        var exception = Assert.Throws<InvalidMoveException>(() => board.MakeMove(16, 24));
        
        Assert.Equal("No piece at position a3.", exception.Message);
    }

    [Fact]
    public void ParseFenSetsColourBitboardsCorrectly()
    {
        var board = new Board(Constants.InitialBoardFen);

        for (var file = 0; file < Constants.Files; file++)
        {
            Assert.True(board.IsColour(Ranks.BlackPawnRank * 8 + file, Colour.Black));
            
            Assert.True(board.IsColour(Ranks.BlackHomeRank * 8  + file, Colour.Black));
            
            Assert.True(board.IsColour(Ranks.WhiteHomeRank * 8  + file, Colour.White));
            
            Assert.True(board.IsColour(Ranks.WhitePawnRank * 8  + file, Colour.White));
        }
    }

    [Fact]
    public void ToStringGeneratesReadableOutput()
    {
        var board = new Board(Constants.InitialBoardFen);
        
        Assert.Equal("rnbqkbnr\npppppppp\n        \n        \n        \n        \nPPPPPPPP\nRNBQKBNR", board.ToString());
    }
}