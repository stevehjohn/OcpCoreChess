using System.Buffers;

namespace OcpCore.Engine.General;

public class Board
{
    private readonly byte[] _cells;

    public byte this[int index] => _cells[index];

    public Board()
    {
        _cells = ArrayPool<byte>.Shared.Rent(Constants.BoardBufferSize);
    }
}