using System.Runtime.CompilerServices;
using OcpCore.Engine.General.StaticData;

namespace OcpCore.Engine.Bitboards;

[InlineArray(Constants.Planes)]
public struct Planes
{
    private ulong _plane;
}