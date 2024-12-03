using OcpCore.Engine.Bitboards;

namespace OcpCore.Engine.Extensions;

public static class PlaneExtensions
{
    public static Plane InvertColour(this Plane plane)
    {
        return 1 - plane;
    }
}