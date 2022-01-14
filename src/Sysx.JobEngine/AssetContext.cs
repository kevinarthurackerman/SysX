namespace Sysx.JobEngine;

public class AssetContext
{
    private readonly Dictionary<Type, AssetSet> assetSetCache = new();
    private readonly IServiceProvider localServiceProvider;

    public AssetContext(
        IEnumerable<IAssetMapping> assetMappings,
        IEngineServiceProvider? engineServiceProvider = null,
        IQueueServiceProvider? queueServiceProvider = null)
    {
        foreach(var assetMapping in assetMappings)
            assetSetCache.Add(assetMapping.AssetType, new AssetSet(assetMapping));
        localServiceProvider = (IServiceProvider?)engineServiceProvider ?? queueServiceProvider!; // todo: either require to have a service provider or handle not having one
    }

    public TAsset GetAsset<TKey, TAsset>(TKey key)
        where TAsset : class, IAsset<TKey>?
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }
            else if (!assetSet.Assets.TryGetValue(key, out var asset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
            }
            else
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, (TAsset)asset, true);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public bool TryGetAsset<TKey, TAsset>(TKey key, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || !assetSet!.Assets.TryGetValue(key, out var asset))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, (TAsset)asset, true);
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

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }
            else if (assetSet.Assets.TryGetValue(asset.Key!, out var _))
            {
                throw new ArgumentException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' already exists.");
            }
            else
            {
                assetSet.Assets[asset.Key!] = asset;

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public bool TryAddAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || assetSet.Assets.TryGetValue(asset.Key!, out var _))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets[asset.Key!] = asset;

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

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }
            else
            {
                assetSet.Assets.TryGetValue(asset.Key!, out var prevAsset);

                assetSet.Assets[asset.Key!] = asset;

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, (TAsset?)prevAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public bool TryUpsertAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
                assetSet.Assets.TryGetValue(asset.Key!, out var prevAsset);

                assetSet.Assets[asset.Key!] = asset;

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, (TAsset?)prevAsset, true);
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

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }
            else if (!assetSet.Assets.TryGetValue(asset.Key!, out var prevAsset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{asset.Key}' was not found.");
            }
            else
            {
                assetSet.Assets[asset.Key!] = asset;

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, (TAsset)prevAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public bool TryUpdateAsset<TKey, TAsset>(TAsset asset, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.IsNotNull(asset, nameof(asset));
        EnsureArg.HasValue(asset.Key, nameof(asset.Key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), asset.Key, asset);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || !assetSet.Assets.TryGetValue(asset.Key!, out var prevAsset))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets[asset.Key!] = asset;

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, (TAsset)prevAsset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), asset.Key, asset, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset DeleteAsset<TKey, TAsset>(TKey key)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }
            else if (!assetSet.Assets.TryGetValue(key, out var asset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
            }
            else
            {
                assetSet.Assets.Remove(key);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, (TAsset)asset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    public bool TryDeleteAsset<TKey, TAsset>(TKey key, out TAsset? result)
        where TAsset : class, IAsset<TKey>
    {
        EnsureArg.HasValue(key, nameof(key));

        var initialRequestData = new OnAssetEventRequestData<TKey, TAsset>(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData<TKey, TAsset>? previousResultData = null;
        OnAssetEventResultData<TKey, TAsset>? currentResultData = null;

        var eventHandlers = localServiceProvider
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
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || !assetSet.Assets.TryGetValue(key, out var asset))
            {
                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets.Remove(key);

                previousResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, (TAsset)asset, true);
                var currentResultData = new OnAssetEventResultData<TKey, TAsset>(typeof(TAsset), key, null, true);
                return new OnAssetEventResult<TKey, TAsset>(previousResultData.Value, currentResultData);
            }
        }
    }

    private class AssetSet
    {
        internal IAssetMapping AssetMapping { get; }
        internal IDictionary<object, object> Assets { get; }

        internal AssetSet(IAssetMapping assetMapping)
        {
            AssetMapping = assetMapping;
            Assets = new Dictionary<object, object>();
        }
    }

    public interface IAssetMapping
    {
        public Type AssetType { get; }
    }

    public class AssetMapping<TKey, TAsset> : IAssetMapping
        where TAsset : class, IAsset<TKey>
    {
        public AssetMapping()
        {
            AssetType = typeof(TAsset);
        }

        public Type AssetType { get; }
    }
}