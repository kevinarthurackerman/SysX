namespace Test_SysX.JobEngine.VoxelEngine.Assets;

public class Chunk : IAsset<Guid>
{
	public Guid Key { get; }
	public int X { get; }
	public int Y { get; }
	public int Z { get; }
	public int[,,] VoxelData { get; }

	public Chunk(Guid key, int x, int y, int z, int[,,] voxelData)
	{
		Key = key;
		X = x;
		Y = y;
		Z = z;
		VoxelData = voxelData;
	}
}