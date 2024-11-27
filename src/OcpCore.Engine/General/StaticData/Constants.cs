namespace OcpCore.Engine.General.StaticData;

public static class Constants
{
    public const int Cells = 64;
    
    public const int Ranks = 8;
    public const int Files = 8;
    
    public const int MaxRank = 7;
    public const int MaxFile = 7;
    
    public const int MaxMoveDistance = 7;
    
    public const int BlackRankCellStart = 56;
    
    public const int NoEnPassant = 0b0111_1111;

    public static readonly (int RankDelta, int FileDelta)[] OrthogonalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1)];
    public static readonly (int RankDelta, int FileDelta)[] DiagonalMoves = [(-1, -1), (-1, 1), (1, -1), (1, 1)];
    public static readonly (int RankDelta, int FileDelta)[] DirectionalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)];
    public static readonly (int RankDelta, int FileDelta)[] KnightMoves = [(2, -1), (2, 1), (-2, -1), (-2, 1), (1, -2), (-1, -2), (1, 2), (-1, 2)];
}