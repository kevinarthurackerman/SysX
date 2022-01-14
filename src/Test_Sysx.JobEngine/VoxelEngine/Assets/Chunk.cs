namespace Test_Sysx.JobEngine.VoxelEngine.Assets;

public class Chunk
{
    public Guid Id { get; }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    public int[,,] VoxelData { get; }

    public Chunk(Guid id, int x, int y, int z, int[,,] voxelData)
    {
        Id = id;
        X = x;
        Y = y;
        Z = z;
        VoxelData = voxelData;
    }
}