namespace Test_SysX.Enums.BaseEnumeration;

public class Color : BaseEnumeration<Color, int>
{
	public static readonly Color Red = new (1, "Red");
	public static readonly Color Blue = new (2, "Blue");

	private Color(int value, string displayName) : base(value, displayName)
	{
	}
}