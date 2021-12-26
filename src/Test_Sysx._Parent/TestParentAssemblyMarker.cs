using System;
using Sysx_Test__Child;

namespace Sysx_Test__Parent;

public static class TestParentAssemblyMarker
{
    public static readonly Type Child = typeof(TestChildAssemblyMarker);
}