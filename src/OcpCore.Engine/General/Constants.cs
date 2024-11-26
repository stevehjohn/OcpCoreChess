namespace OcpCore.Engine.General;

public static class Constants
{
    public const int Cells = 64;
    public const int Ranks = 8;
    public const int Files = 8;
    public const int MaxRank = 7;
    public const int MaxFile = 7;

    public const int BlackHomeRank = 7;
    public const int BlackPawnRank = 6;
    public const int WhitePawnRank = 1;
    public const int WhiteHomeRank = 0;
    
    public const int BlackRankCellStart = 56;

    public const int RankOffset = 3;

    public const int CastleStatusMask = 0b0000_1111;
    public const int PlayerTurnBit = 0b0001_0000;
    public const int EnPassantOffset = 5;
    
    public const int MaxMoveDistance = 7;

    public static readonly (int RankDelta, int FileDelta)[] OrthogonalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1)];
    public static readonly (int RankDelta, int FileDelta)[] DiagonalMoves = [(-1, -1), (-1, 1), (1, -1), (1, 1)];
    public static readonly (int RankDelta, int FileDelta)[] DirectionalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)];
    public static readonly (int RankDelta, int FileDelta)[] KnightMoves = [(2, -1), (2, 1), (-2, -1), (-2, 1), (1, -2), (-1, -2), (1, 2), (-1, 2)];
}