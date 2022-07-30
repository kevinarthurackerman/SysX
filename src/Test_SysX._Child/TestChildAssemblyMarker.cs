using System;
using SysX_Test__Grandchild;

namespace SysX_Test__Child;

public static class TestChildAssemblyMarker
{
	public static readonly Type Child = typeof(TestGrandchildAssemblyMarker);
}