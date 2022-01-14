namespace Test_Sysx.JobEngine.VoxelEngine.OnActionHandlers;

public class OnAssetEvent_Log<TAsset> : IOnAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next)
    {
        // some logic to log here

        return next(request.Current).Current;
    }
}
