﻿namespace Sysx.Test.Reflection.AssemblyX;
using Assert = Xunit.Assert;
using AssemblyX = Sysx.Reflection.AssemblyX;

public class AssemblyXTests
{
#if NET5_0 || NETCOREAPP3_1
    [Fact]
    public void Should_Load()
    {
        var testAssemblyPath = typeof(TestParentAssemblyMarker).Assembly.Location;
        var options = AssemblyX.LoadOptions.Default with 
        {
            RootAssemblyName = AssemblyName.GetAssemblyName(testAssemblyPath)
        };
        options.AssemblyDependencyResolvers!.Insert(0, new AssemblyDependencyResolver(testAssemblyPath));

        var assemblies = AssemblyX.Load(in options).ToArray();

        Assert.NotNull(assemblies);
        Assert.Contains(typeof(TestParentAssemblyMarker).Assembly, assemblies);
        Assert.DoesNotContain(typeof(TestChildAssemblyMarker).Assembly, assemblies);
        Assert.DoesNotContain(typeof(TestGrandchildAssemblyMarker).Assembly, assemblies);
    }

    [Fact]
    public void Should_Load_Child()
    {
        var testAssemblyPath = typeof(TestParentAssemblyMarker).Assembly.Location;
        var options = AssemblyX.LoadOptions.Default with
        {
            RootAssemblyName = AssemblyName.GetAssemblyName(testAssemblyPath),
            IncludeRootAssembly = false,
            LoadDepth = AssemblyX.LoadDepth.LoadChildren
        };
        options.AssemblyDependencyResolvers!.Insert(0, new AssemblyDependencyResolver(testAssemblyPath));

        var assemblies = AssemblyX.Load(in options).ToArray();

        Assert.NotNull(assemblies);
        Assert.DoesNotContain(typeof(TestParentAssemblyMarker).Assembly, assemblies);
        Assert.Contains(typeof(TestChildAssemblyMarker).Assembly, assemblies);
        Assert.DoesNotContain(typeof(TestGrandchildAssemblyMarker).Assembly, assemblies);
    }

    [Fact]
    public void Should_Load_Descendants()
    {
        var testAssemblyPath = typeof(TestParentAssemblyMarker).Assembly.Location;
        var options = AssemblyX.LoadOptions.Default with
        {
            RootAssemblyName = AssemblyName.GetAssemblyName(testAssemblyPath),
            IncludeRootAssembly = false,
            LoadDepth = AssemblyX.LoadDepth.RecursivelyLoadDescendants
        };
        options.AssemblyDependencyResolvers!.Insert(0, new AssemblyDependencyResolver(testAssemblyPath));

        var assemblies = AssemblyX.Load(in options).ToArray();

        Assert.NotNull(assemblies);
        Assert.DoesNotContain(typeof(TestParentAssemblyMarker).Assembly, assemblies);
        Assert.Contains(typeof(TestChildAssemblyMarker).Assembly, assemblies);
        Assert.Contains(typeof(TestGrandchildAssemblyMarker).Assembly, assemblies);
    }

    [Fact]
    public void Should_Load_All()
    {
        var testAssemblyPath = typeof(TestParentAssemblyMarker).Assembly.Location;
        var options = AssemblyX.LoadOptions.Default with
        {
            RootAssemblyName = AssemblyName.GetAssemblyName(testAssemblyPath),
            LoadDepth = AssemblyX.LoadDepth.RecursivelyLoadDescendants
        };
        options.AssemblyDependencyResolvers!.Insert(0, new AssemblyDependencyResolver(testAssemblyPath));

        var assemblies = AssemblyX.Load(in options).ToArray();

        Assert.NotNull(assemblies);
        Assert.Contains(typeof(TestParentAssemblyMarker).Assembly, assemblies);
        Assert.Contains(typeof(TestChildAssemblyMarker).Assembly, assemblies);
        Assert.Contains(typeof(TestGrandchildAssemblyMarker).Assembly, assemblies);
    }
#endif
}