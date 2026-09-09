using OcpCore.Engine.Bitboards;
using OcpCore.Engine.General;
using OcpCore.Engine.Kernel;
using Xunit;

namespace OcpCore.Engine.Tests.Kernel;

public class PerftCorpusTests
{
    public static IEnumerable<object[]> Positions()
    {
        var index = 0;

        foreach (var line in File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Data", "standard.epd")))
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries);

            var expected = parts.Skip(1).Where(part => int.Parse(part[1..2]) <= 3).DefaultIfEmpty(parts[1]).ToArray();

            yield return [index++, parts[0], expected.Select(part => int.Parse(part[1..2])).ToArray(), expected.Select(part => long.Parse(part[3..])).ToArray()];
        }
    }

    [Theory]
    [MemberData(nameof(Positions))]
    public void MatchesReferenceCounts(int index, string fen, int[] depths, long[] counts)
    {
        var game = new Game();

        game.ParseFen(fen);

        using var coordinator = new Coordinator(Colour.White);

        coordinator.StartProcessing(game, depths.Max());

        for (var i = 0; i < counts.Length; i++)
        {
            Assert.True(counts[i] == coordinator.GetDepthCount(depths[i]),
                $"Corpus entry {index + 1}, depth {depths[i]}: expected {counts[i]}, actual {coordinator.GetDepthCount(depths[i])}.");
        }
    }
}
