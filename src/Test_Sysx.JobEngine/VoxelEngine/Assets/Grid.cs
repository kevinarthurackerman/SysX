namespace Test_Sysx.JobEngine.VoxelEngine.Assets;

public class Grid
{
    public Guid Id { get; }
    public Guid VoxelPalletId { get; }
    public Guid VoxelShapeId { get; }
    public IEnumerable<Guid> VoxelChunkIds { get; }

    public Grid(Guid id, Guid voxelPalletId, Guid voxelShapeId, IEnumerable<Guid> voxelChunkIds)
    {
        Id = id;
        VoxelPalletId = voxelPalletId;
        VoxelShapeId = voxelShapeId;
        VoxelChunkIds = voxelChunkIds;
    }
}