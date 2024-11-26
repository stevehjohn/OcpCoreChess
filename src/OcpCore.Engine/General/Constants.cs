namespace OcpCore.Engine.General;

public static class Constants
{
    public const int Cells = 64;

    public const int MaxRank = 7;

    public const int MaxFile = 7;

    public const int BoardBufferSize = Cells + 2;

    public const int RankOffset = 3;

    public const int FileMask = 7;
    
    public const int MaxMoveDistance = 7;

    public static readonly (int RankDelta, int FileDelta)[] OrthogonalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1)];

    public static readonly (int RankDelta, int FileDelta)[] DiagonalMoves = [(-1, -1), (-1, 1), (1, -1), (1, 1)];

    public static readonly (int RankDelta, int FileDelta)[] DirectionalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)];
}