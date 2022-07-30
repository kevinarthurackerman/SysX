namespace Test_SysX.JobEngine.VoxelEngine.Assets;

public class Shape : IAsset<Guid>
{
	public Guid Key { get; }

	public Shape(Guid key)
	{
		Key = key;
	}
}
