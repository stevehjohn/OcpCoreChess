// using OcpCore.Engine.Exceptions;
// using OcpCore.Engine.General;
// using OcpCore.Engine.General.StaticData;
// using OcpCore.Engine.Pieces;
// using Xunit;
//
// namespace OcpCore.Engine.Tests;
//
// public class CoreTests
// {
//     [Theory]
//     [InlineData(1, 20, 0, 0, 0, 0, 0, 0)]
//     [InlineData(2, 400, 0, 0, 0, 0, 0, 0)]
//     [InlineData(3, 8_902, 34, 0, 0, 0, 12, 0)]
//     [InlineData(4, 197_281, 1_576, 0, 0, 0, 469, 8)]
//     [InlineData(5, 4_865_609, 82_719, 258, 0, 0, 27_351, 347)]
//     public void ReturnsExpectedCountAtPly(int ply, int count, int capture, int enPassant, int castle, int promotion, int check, int mate)
//     {
//         using var core = new Core(Colour.White);
//     
//         core.GetMove(ply);
//         
//         Assert.Equal(count, core.GetDepthCount(ply));
//         
//         Assert.Equal(capture, core.GetMoveOutcome(ply, MoveOutcome.Capture));
//         
//         Assert.Equal(enPassant, core.GetMoveOutcome(ply, MoveOutcome.EnPassant));
//         
//         Assert.Equal(castle, core.GetMoveOutcome(ply, MoveOutcome.Castle));
//         
//         Assert.Equal(promotion, core.GetMoveOutcome(ply, MoveOutcome.Promotion));
//         
//         Assert.Equal(check, core.GetMoveOutcome(ply, MoveOutcome.Check));
//         
//         Assert.Equal(mate, core.GetMoveOutcome(ply, MoveOutcome.CheckMate));
//     }
//
//     [Theory]
//     [InlineData(Constants.InitialBoardFen, "a2a4", true, Kind.Pawn)]
//     [InlineData(Constants.InitialBoardFen, "a2b4", false, Kind.Pawn)]
//     [InlineData(Constants.InitialBoardFen, "b1a3", true, Kind.Knight)]
//     [InlineData(Constants.InitialBoardFen, "b1b3", false, Kind.Knight)]
//     public void ChecksMoveValidity(string fen, string move, bool isValid, Kind kind)
//     {
//         var core = new Core(Colour.White, fen);
//
//         if (isValid)
//         {
//             core.MakeMove(move);
//         }
//         else
//         {
//             var exception = Assert.Throws<InvalidMoveException>(() => core.MakeMove(move));
//             
//             Assert.Equal($"{move} is not a valid move for a {kind}.", exception.Message);
//         }
//     }
//
//     [Fact]
//     private void GetMoveFiresCallbackIfActionProvided()
//     {
//         var core = new Core(Colour.White, Constants.InitialBoardFen);
//
//         var called = false;
//
//         core.GetMove(2, () => called = true);
//         
//         Thread.Sleep(100);
//         
//         Assert.True(called);
//     }
// }