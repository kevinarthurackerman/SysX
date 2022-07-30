using System;
using SysX_Test__Child;

namespace SysX_Test__Parent;

public static class TestParentAssemblyMarker
{
	public static readonly Type Child = typeof(TestChildAssemblyMarker);
}