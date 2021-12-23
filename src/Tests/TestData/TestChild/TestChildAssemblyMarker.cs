using System;
using Sysx.Test.TestGrandchild;

namespace Sysx.Test.TestChild;

public static class TestChildAssemblyMarker
{
    public static Type Child = typeof(TestGrandchildAssemblyMarker);
}