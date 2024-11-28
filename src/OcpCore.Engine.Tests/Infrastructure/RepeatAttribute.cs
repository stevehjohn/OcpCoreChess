using System.Reflection;
using Xunit.Sdk;

namespace OcpCore.Engine.Tests.Infrastructure;

public class RepeatAttribute : DataAttribute
{
    private readonly int _count;

    private readonly object[] _data;

    public RepeatAttribute(int count, params object[] data)
    {
        _count = count;

        _data = data;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return Enumerable.Repeat(_data, _count);
    }
}