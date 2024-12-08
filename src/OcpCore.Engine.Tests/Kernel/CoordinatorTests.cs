using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General.StaticData;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class CoordinatorTests
{
    [Fact]
    public void ParallelisesAtGivenLevel()
    {
        var coordinator = new Coordinator(1);

        var game = new Game();
        
        game.ParseFen(Constants.InitialBoardFen);
        
        coordinator.StartProcessing(game, 2);
    }
}