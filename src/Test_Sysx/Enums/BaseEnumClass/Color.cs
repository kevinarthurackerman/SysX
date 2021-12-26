namespace Test_Sysx.Enums.BaseEnumClass;

public class Color : BaseEnumClass<Color, int>
{
    public static readonly Color Red = new Color(1, "Red");
    public static readonly Color Blue = new Color(2, "Blue");

    private Color(int value, string displayName) : base(value, displayName)
    {
    }
}