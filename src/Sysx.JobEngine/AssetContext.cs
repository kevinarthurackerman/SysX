namespace Sysx.JobEngine;

public class AssetContext : ISinglePhaseNotification
{
    private readonly IQueueServiceProvider queueServiceProvider;
    private readonly Dictionary<Type, IAssetSet> assetSetCache;
    private Transaction? transaction;

    public AssetContext(
        IEnumerable<IAssetMapping> assetMappings,
        IQueueServiceProvider queueServiceProvider)
    {
        assetSetCache = assetMappings.ToDictionary(
            assetMapping => assetMapping.AssetType,
            assetMapping =>
            {
                return (IAssetSet)typeof(AssetSet<,>)
                    .MakeGenericType(assetMapping.AssetKeyType, assetMapping.AssetType)
                    .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single()
                    .Invoke(new[] { assetMapping });
            });

        this.queueServiceProvider = queueServiceProvider;
    }

    public TAsset GetAsset<TKey, TAsset>(TKey key)
        where TAsset : class, IAsset<TKey>?
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var asset = typedAssetSet.GetAsset(key);

            if (asset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
            var currentResultData = previousResultData.Value;
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryGetAsset<TKey, TAsset>(TKey key, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.HasValue(key, nameof(key));

        EnlistTransaction();

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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var asset = typedAssetSet.GetAsset(key);

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

    public TAsset AddAsset<TKey, TAsset>(TAsset asset)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var existingAsset = typedAssetSet.GetAsset(asset.Key);

            if (existingAsset != null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' already exists.");

            typedAssetSet.SetAsset(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryAddAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var existingAsset = typedAssetSet.GetAsset(asset.Key);

            if (existingAsset != null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                typedAssetSet.SetAsset(asset.Key, asset);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset UpsertAsset<TKey, TAsset>(TAsset asset)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var existingAsset = typedAssetSet.GetAsset(asset.Key);

            typedAssetSet.SetAsset(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpsertAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

                var existingAsset = typedAssetSet.GetAsset(asset.Key);

                typedAssetSet.SetAsset(asset.Key, asset);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset UpdateAsset<TKey, TAsset>(TAsset asset)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var existingAsset = typedAssetSet.GetAsset(asset.Key);

            if (existingAsset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' was not found.");

            typedAssetSet.SetAsset(asset.Key, asset);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpdateAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var existingAsset = typedAssetSet.GetAsset(asset.Key);

            if (existingAsset == null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                typedAssetSet.SetAsset(asset.Key, asset);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, existingAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset DeleteAsset<TKey, TAsset>(TKey key)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var asset = typedAssetSet.GetAsset(key);

            if (asset == null)
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");

            typedAssetSet.SetAsset(asset.Key, null);

            previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
            var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
            return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
        }
    }

    public bool TryDeleteAsset<TKey, TAsset>(TKey key, out TAsset? result)
        where TAsset : class, IAsset<TKey>
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }

            var typedAssetSet = (AssetSet<TKey, TAsset>)assetSet;

            var asset = typedAssetSet.GetAsset(key);

            if (asset == null)
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                typedAssetSet.SetAsset(asset.Key, null);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, asset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public IEnumerable<UncommittedAsset<TKey, TAsset>> GetUncommittedAssets<TKey, TAsset>()
        where TAsset : class, IAsset<TKey>
    {
        var assetSet = (AssetSet<TKey, TAsset>)assetSetCache[typeof(TAsset)];
        
        foreach (var uncommitted in assetSet.UncommittedAssets)
        {
            assetSet.Assets.TryGetValue(uncommitted.Key, out var current);

            yield return new UncommittedAsset<TKey, TAsset>(current, uncommitted.Value);
        }
    }

    public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
    {
        Commit();
        singlePhaseEnlistment.Committed();
    }

    public void Commit(Enlistment enlistment)
    {
        Commit();
        enlistment.Done();
    }

    public void InDoubt(Enlistment enlistment)
    {
        enlistment.Done();
    }

    public void Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    public void Rollback(Enlistment enlistment)
    {
        transaction = null;

        foreach (var assetSet in assetSetCache.Values)
            assetSet.Rollback();

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

        foreach (var assetSet in assetSetCache.Values)
            assetSet.Commit();
    }

    private interface IAssetSet
    {
        internal void Commit();
        internal void Rollback();
    }

    private class AssetSet<TKey, TAsset> : IAssetSet
        where TAsset : class, IAsset<TKey>
    {
        internal IAssetMapping AssetMapping { get; }
        internal IDictionary<TKey, TAsset> Assets { get; }
        internal IDictionary<TKey, TAsset?> UncommittedAssets { get; }

        internal AssetSet(IAssetMapping assetMapping)
        {
            AssetMapping = assetMapping;
#pragma warning disable CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
            Assets = new Dictionary<TKey, TAsset>();
            UncommittedAssets = new Dictionary<TKey, TAsset?>();
#pragma warning restore CS8714 // Key can be nullable in order to match asset key, but null will never be passed in as a key.
        }

        internal TAsset? GetAsset(TKey key)
        {
            if (Transaction.Current != null)
            {
                if (UncommittedAssets.TryGetValue(key, out var uncommittedAsset))
                    return uncommittedAsset;
            }

            Assets.TryGetValue(key, out var asset);

            return asset;
        }

        internal void SetAsset(TKey key, TAsset? asset)
        {
            if (Transaction.Current != null)
            {
                UncommittedAssets[key] = asset;
            }
            else
            {
                if (asset == null)
                {
                    Assets.Remove(key);
                }
                else
                {
                    Assets[key] = asset;
                }
            }
        }

        void IAssetSet.Commit()
        {
            foreach(var asset in UncommittedAssets)
            {
                if (asset.Value == null)
                {
                    Assets.Remove(asset.Key);
                }
                else
                {
                    Assets[asset.Key] = asset.Value;
                }
            }

            UncommittedAssets.Clear();
        }

        void IAssetSet.Rollback()
        {
            UncommittedAssets.Clear();
        }
    }

    public interface IAssetMapping
    {
        public Type AssetKeyType { get; }
        public Type AssetType { get; }
    }

    public class AssetMapping<TKey, TAsset> : IAssetMapping
        where TAsset : class, IAsset<TKey>
    {
        public AssetMapping()
        {
            AssetKeyType = typeof(TKey);
            AssetType = typeof(TAsset);
        }

        public Type AssetKeyType { get; }
        public Type AssetType { get; }
    }

    public readonly record struct UncommittedAsset<TKey, TAsset>(TAsset? Current, TAsset? Uncommitted)
        where TAsset : IAsset<TKey>;
}