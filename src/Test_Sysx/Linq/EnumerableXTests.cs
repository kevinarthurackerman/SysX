namespace Test_Sysx.Linq;
using Assert = Xunit.Assert;

public class EnumerableXTests
{
    private readonly Node NodeA;
    private readonly Node NodeB;
    private readonly Node NodeC;
    private readonly Node NodeD;
    private readonly Node NodeE;
    private readonly Node NodeF;
    private readonly Node NodeG;
    private readonly Node NodeH;
    private readonly Node NodeI;
    private readonly Node NodeJ;
    private readonly Node NodeK;

    public EnumerableXTests()
    {
        NodeA = new Node("A");
        NodeB = new Node("B");
        NodeC = new Node("C");
        NodeD = new Node("D");
        NodeE = new Node("E");
        NodeF = new Node("F");
        NodeG = new Node("G");
        NodeH = new Node("H");
        NodeI = new Node("I");
        NodeJ = new Node("J");
        NodeK = new Node("K");

        NodeB.AddToParent(NodeA);
        NodeC.AddToParent(NodeA);

        NodeD.AddToParent(NodeB);
        NodeE.AddToParent(NodeB);

        NodeF.AddToParent(NodeC);
        NodeG.AddToParent(NodeC);

        NodeH.AddToParent(NodeD);
        NodeI.AddToParent(NodeD);

        NodeJ.AddToParent(NodeH);

        NodeK.AddToParent(NodeI);
    }

    [Fact]
    public void Should_Get_Ancestors()
    {
        var actual = EnumerableX.Ancestors(NodeD, x => x.Parent).ToArray();

        var expected = new[] { NodeB, NodeA };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    [Fact]
    public void Should_Get_Ancestors_With_Root()
    {
        var actual = EnumerableX.Ancestors(NodeD, x => x.Parent, includeRoot: true).ToArray();

        var expected = new[] { NodeD, NodeB, NodeA };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    [Fact]
    public void Should_Restrict_Ancestors_Depth()
    {
        var actual = EnumerableX.Ancestors(NodeD, x => x.Parent, maxDepth: 1).ToArray();

        var expected = new[] { NodeB };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    [Fact]
    public void Should_Get_Descendants()
    {
        var actual = EnumerableX.Descendants(NodeD, x => x.Children).ToArray();

        var expected = new[] { NodeH, NodeI, NodeJ, NodeK };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    [Fact]
    public void Should_Get_Descendants_With_Root()
    {
        var actual = EnumerableX.Descendants(NodeD, x => x.Children, includeRoot: true).ToArray();

        var expected = new[] { NodeD, NodeH, NodeI, NodeJ, NodeK };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    [Fact]
    public void Should_Restrict_Descendants_Depth()
    {
        var actual = EnumerableX.Descendants(NodeD, x => x.Children, maxDepth: 1).ToArray();

        var expected = new[] { NodeH, NodeI };

        Assert.Equal(expected.Length, actual.Length);

        for (var i = 0; i < actual.Length; i++)
            Assert.Equal(expected[0], actual[0]);
    }

    public class Node
    {
        private readonly IList<Node> children = new List<Node>();

        public string Name { get; }
        public Node? Parent { get; private set; }
        public IEnumerable<Node> Children => children;

        public Node(string name)
        {
            Name = name;
        }

        public void AddToParent(Node parent)
        {
            Parent?.children.Remove(this);
            Parent = parent;
            Parent.children.Add(this);
        }
    }
}