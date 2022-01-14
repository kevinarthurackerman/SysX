namespace Sysx.JobEngine;

public interface IOnAssetEvent<TAsset> : IOnModifyAssetEvent<TAsset>, IOnGetAssetEvent<TAsset> { }

public interface IOnModifyAssetEvent<TAsset> : IOnAddAssetEvent<TAsset>, IOnUpsertAssetEvent<TAsset>, IOnUpdateAssetEvent<TAsset>, IOnDeleteAssetEvent<TAsset> { }

public interface IOnGetAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next);
}

public interface IOnAddAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next);
}

public interface IOnUpsertAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next);
}

public interface IOnUpdateAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next);
}

public interface IOnDeleteAssetEvent<TAsset>
{
    public OnAssetEventResultData Execute(in OnAssetEventRequest request, OnAssetEventNext next);
}

public readonly record struct OnAssetEventRequestData(Type AssetType, object? AssetKey, object? Asset);

public readonly record struct OnAssetEventRequest(in OnAssetEventRequestData Previous, in OnAssetEventRequestData Current);

public readonly record struct OnAssetEventResultData(Type AssetType, object? AssetKey, object? Asset, bool Success);

public readonly record struct OnAssetEventResult(in OnAssetEventResultData Previous, in OnAssetEventResultData Current);

public delegate OnAssetEventResult OnAssetEventNext(in OnAssetEventRequestData requestData);