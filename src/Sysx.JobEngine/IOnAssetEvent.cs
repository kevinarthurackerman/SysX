namespace Sysx.JobEngine;

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnAssetEvent<TKey, TAsset>
    : IOnModifyAssetEvent<TKey, TAsset>, IOnGetAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
    { }

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnModifyAssetEvent<TKey, TAsset>
    : IOnAddAssetEvent<TKey, TAsset>, IOnUpsertAssetEvent<TKey, TAsset>, IOnUpdateAssetEvent<TKey, TAsset>, IOnDeleteAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
    { }

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnGetAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next);
}

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnAddAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next);
}

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnUpsertAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next);
}

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnUpdateAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next);
}

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public interface IOnDeleteAssetEvent<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public OnAssetEventResultData<TKey, TAsset> Execute(in OnAssetEventRequest<TKey, TAsset> request, OnAssetEventNext<TKey, TAsset> next);
}

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public readonly record struct OnAssetEventRequestData<TKey, TAsset>(Type AssetType, TKey AssetKey, TAsset? Asset)
    where TAsset : class, IAsset<TKey>;

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public readonly record struct OnAssetEventRequest<TKey, TAsset>(in OnAssetEventRequestData<TKey, TAsset> Original, in OnAssetEventRequestData<TKey, TAsset> Current)
    where TAsset : class, IAsset<TKey>;

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public readonly record struct OnAssetEventResultData<TKey, TAsset>(Type AssetType, TKey AssetKey, TAsset? Asset, bool Success)
    where TAsset : class, IAsset<TKey>;

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public readonly record struct OnAssetEventResult<TKey, TAsset>(in OnAssetEventResultData<TKey, TAsset> Original, in OnAssetEventResultData<TKey, TAsset> Current)
    where TAsset : class, IAsset<TKey>;

[Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
public delegate OnAssetEventResult<TKey, TAsset> OnAssetEventNext<TKey, TAsset>(in OnAssetEventRequestData<TKey, TAsset> requestData)
    where TAsset : class, IAsset<TKey>;