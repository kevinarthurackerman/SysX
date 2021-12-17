using System;
using Sysx.Test.TestChild;

namespace Sysx.Test.TestParent;

public static class TestParentAssemblyMarker
{
    public static Type Child = typeof(TestChildAssemblyMarker);
}