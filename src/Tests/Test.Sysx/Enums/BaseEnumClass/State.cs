namespace Sysx.Test.Enums.BaseEnumClass;

public class State : Sysx.Enums.BaseEnumClass<State, int>
{
    public static readonly State Off = new State(1, "Off");
    public static readonly State Starting = new State(2, "Starting");
    public static readonly State Waiting = new State(3, "Waiting");
    public static readonly State Busy = new State(4, "Busy");
    public static readonly State Stopping = new State(5, "Stopping");

    private State(int value, string displayName) : base(value, displayName)
    {
    }
}