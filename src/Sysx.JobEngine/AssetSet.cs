namespace Sysx.JobEngine;

public interface IAssetSet<TKey, TAsset>
    where TAsset : class, IAsset<TKey>
{
    public TAsset Get(TKey key);
    public bool TryGet(TKey key, out TAsset? result);
    public TAsset Add(TAsset asset);
    public bool TryAdd(TAsset asset, out TAsset? result);
    public TAsset Upsert(TAsset asset);
    public bool TryUpsert(TAsset asset, out TAsset? result);
    public TAsset Update(TAsset asset);
    public bool TryUpdate(TAsset asset, out TAsset? result);
    public TAsset Delete(TKey key);
    public bool TryDelete(TKey key, out TAsset? result);
    public IEnumerable<UncommittedAsset<TKey, TAsset>> GetUncommittedAssets();
}

public class AssetSet<TKey, TAsset> : IAssetSet<TKey, TAsset>, ISinglePhaseNotification
    where TAsset : class, IAsset<TKey>
{
    private readonly IAssetMapping assetMapping;
    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly IDictionary<TKey, TAsset> assets;
    private readonly IDictionary<TKey, TAsset?> uncommittedAssets;
    private Transaction? transaction;

    internal AssetSet(IAssetMapping assetMapping, IQueueServiceProvider queueServiceProvider)
    {
        this.assetMapping = assetMapping;
        this.queueServiceProvider = queueServiceProvider;
#pragma warning disable CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
        assets = new Dictionary<TKey, TAsset>();
        uncommittedAssets = new Dictionary<TKey, TAsset?>();
#pragma warning restore CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
    }

    public TAsset Get(TKey key)
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnGetAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return handlerResult.Current.Asset!;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var asset = Find(key);

            if (asset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
            var currentResultData = previousResultData.Value;
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryGet(TKey key, out TAsset? result)
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnGetAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var asset = Find(key);

            if (asset == null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset Add(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnAddAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return handlerResult.Current.Asset!;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            if (existingAsset != null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' already exists.");

            Set(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryAdd(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnAddAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            if (existingAsset != null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                Set(asset.Key, asset);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset Upsert(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnUpsertAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return handlerResult.Current.Asset!;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            Set(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpsert(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnUpsertAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            Set(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public TAsset Update(TAsset asset)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnUpdateAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return handlerResult.Current.Asset!;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            if (existingAsset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' was not found.");

            Set(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpdate(TAsset asset, out TAsset? result)
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnUpdateAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var existingAsset = Find(asset.Key);

            if (existingAsset == null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                Set(asset.Key, asset);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset Delete(TKey key)
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnDeleteAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return handlerResult.Current.Asset!;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var asset = Find(key);

            if (asset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");

            Set(asset.Key, null);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryDelete(TKey key, out TAsset? result)
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = queueServiceProvider
            .GetServices<IOnDeleteAssetEvent<TKey, TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData<TKey, TAsset> currentRequestData) =>
            {
                var request = new OnAssetEventRequest<TKey, TAsset>(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData<TKey, TAsset> requestData) => innerHandler(requestData));
                return new OnAssetEventResult<TKey, TAsset>(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult<TKey, TAsset> RootHandler(in OnAssetEventRequestData<TKey, TAsset> requestData)
        {
            var asset = Find(key);

            if (asset == null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                Set(asset.Key, null);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public IEnumerable<UncommittedAsset<TKey, TAsset>> GetUncommittedAssets()
    {
        foreach (var uncommitted in uncommittedAssets)
        {
            assets.TryGetValue(uncommitted.Key, out var current);

            yield return new UncommittedAsset<TKey, TAsset>(current, uncommitted.Value);
        }
    }

    private TAsset? Find(TKey key)
    {
        if (Transaction.Current != null)
        {
            if (uncommittedAssets.TryGetValue(key, out var uncommittedAsset))
                return uncommittedAsset;
        }

        assets.TryGetValue(key, out var asset);

        return asset;
    }

    private void Set(TKey key, TAsset? asset)
    {
        if (Transaction.Current != null)
        {
            uncommittedAssets[key] = asset;
        }
        else
        {
            if (asset == null)
            {
                assets.Remove(key);
            }
            else
            {
                assets[key] = asset;
            }
        }
    }

    void ISinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
    {
        Commit();
        singlePhaseEnlistment.Committed();
    }

    void IEnlistmentNotification.Commit(Enlistment enlistment)
    {
        Commit();
        enlistment.Done();
    }

    void IEnlistmentNotification.InDoubt(Enlistment enlistment)
    {
        enlistment.Done();
    }

    void IEnlistmentNotification.Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    void IEnlistmentNotification.Rollback(Enlistment enlistment)
    {
        transaction = null;

        uncommittedAssets.Clear();

        enlistment.Done();
    }

    private void EnlistTransaction()
    {
        if (Transaction.Current != null && Transaction.Current != transaction)
        {
            transaction = Transaction.Current;
            transaction.EnlistVolatile(this, EnlistmentOptions.None);
        }
    }

    private void Commit()
    {
        transaction = null;

        foreach (var asset in uncommittedAssets)
        {
            if (asset.Value == null)
            {
                assets.Remove(asset.Key);
            }
            else
            {
                assets[asset.Key] = asset.Value;
            }
        }

        uncommittedAssets.Clear();
    }
}

public readonly record struct UncommittedAsset<TKey, TAsset>(TAsset? Current, TAsset? Uncommitted)
    where TAsset : class, IAsset<TKey>;