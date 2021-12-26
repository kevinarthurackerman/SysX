using System;
using Sysx_Test__Grandchild;

namespace Sysx_Test__Child;

public static class TestChildAssemblyMarker
{
    public static readonly Type Child = typeof(TestGrandchildAssemblyMarker);
}