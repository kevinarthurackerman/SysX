namespace Test_SysX.Enums.BaseEnumeration;

public class State : BaseEnumeration<State, int>
{
	public static readonly State Off = new (1, "Off");
	public static readonly State Starting = new (2, "Starting");
	public static readonly State Waiting = new (3, "Waiting");
	public static readonly State Busy = new (4, "Busy");
	public static readonly State Stopping = new (5, "Stopping");

	private State(int value, string displayName) : base(value, displayName)
	{
	}
}