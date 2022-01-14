namespace Test_Sysx.JobEngine.VoxelEngine.Assets;

public class Pallet
{
    public Guid Id { get; }
    public ImmutableDictionary<int, VoxelType> VoxelCodeMappings { get; }

    public Pallet(Guid id, ImmutableDictionary<int, VoxelType> voxelCodeMappings)
    {
        Id = id;
        VoxelCodeMappings = voxelCodeMappings;
    }

    public class VoxelType { }
}