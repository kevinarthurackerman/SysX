namespace Test_Sysx.JobEngine.VoxelEngine.OnActionHandlers;

public class OnAssetEvent_Log<TKey, TAsset> : IOnAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next)
    {
        // some logic to log here

        return next(request.Current).Current;
    }
}
