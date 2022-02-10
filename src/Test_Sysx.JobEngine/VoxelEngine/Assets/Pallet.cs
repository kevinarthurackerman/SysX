namespace Test_SysX.JobEngine.VoxelEngine.Assets;

public class Pallet : IAsset<Guid>
{
    public Guid Key { get; }
    public ImmutableDictionary<int, VoxelType> VoxelCodeMappings { get; }

    public Pallet(Guid key, ImmutableDictionary<int, VoxelType> voxelCodeMappings)
    {
        Key = key;
        VoxelCodeMappings = voxelCodeMappings;
    }

    public class VoxelType { }
}