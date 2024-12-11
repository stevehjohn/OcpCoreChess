namespace OcpCore.Engine.General.StaticData;

public static class Constants
{
    public const int Cells = 64;

    public const int Ranks = 8;
    public const int Files = 8;
    
    public const int MaxRank = 7;
    public const int MaxFile = 7;

    public const int MoveOutcomes = 7;

    public const int Planes = 8;

    public const int WhiteEnPassantTargetRankStart = 40;
    public const int BlackEnPassantTargetRankStart = 16;

    public static readonly (int RankDelta, int FileDelta)[] DirectionalMoves = [(-1, 0), (0, -1), (1, 0), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)];
    public static readonly (int RankDelta, int FileDelta)[] KnightMoves = [(2, -1), (2, 1), (-2, -1), (-2, 1), (1, -2), (-1, -2), (1, 2), (-1, 2)];

    public const string InitialBoardFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
}