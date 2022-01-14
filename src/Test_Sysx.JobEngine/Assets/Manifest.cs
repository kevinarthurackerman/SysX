namespace Test_Sysx.JobEngine.Assets;

public class Manifest : IAsset<string>
{
    public string Key { get; }

    public IEnumerable<Guid> PalletIds { get; }

    public Manifest(string key, IEnumerable<Guid> palletIds)
    {
        Key = key;
        PalletIds = palletIds;
    }
}
