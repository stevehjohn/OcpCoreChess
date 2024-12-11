using System.Runtime.CompilerServices;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

[InlineArray(Constants.MoveSets)]
public struct MoveArray
{
    private ulong _moveArray;
}