namespace Test_SysX.JobEngine.VoxelEngine.Assets;

public class Grid : IAsset<Guid>
{
	public Guid Key { get; }
	public Guid VoxelPalletId { get; }
	public Guid VoxelShapeId { get; }
	public IEnumerable<Guid> VoxelChunkIds { get; }

	public Grid(Guid key, Guid voxelPalletId, Guid voxelShapeId, IEnumerable<Guid> voxelChunkIds)
	{
		Key = key;
		VoxelPalletId = voxelPalletId;
		VoxelShapeId = voxelShapeId;
		VoxelChunkIds = voxelChunkIds;
	}
}