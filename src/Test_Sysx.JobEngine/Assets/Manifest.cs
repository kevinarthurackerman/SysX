namespace Test_Sysx.JobEngine.Assets;

public class Manifest
{
    public string Id { get; }

    public IEnumerable<Guid> PalletIds { get; }

    public Manifest(string id, IEnumerable<Guid> palletIds)
    {
        Id = id;
        PalletIds = palletIds;
    }
}
