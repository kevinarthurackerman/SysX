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

    public TAsset GetAsset<TAsset>(object key)
    {
        EnsureArg.HasValue(key);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnGetAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return (TAsset)handlerResult.Current.Asset!;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                throw new ArgumentException($"Key type {key.GetType()} does not match key type {assetSet.AssetMapping.KeyType} of asset type {typeof(TAsset)}.");
            }

            if (!assetSet.Assets.TryGetValue(key, out var asset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
            }

            previousResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
            var currentResultData = previousResultData.Value;
            return new OnAssetEventResult(previousResultData.Value, currentResultData);
        }
    }

    public bool TryGetAsset<TAsset>(object key, out TAsset? result)
    {
        EnsureArg.HasValue(key);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnGetAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = (TAsset?)handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || key.GetType() != assetSet.AssetMapping.KeyType
                || !assetSet!.Assets.TryGetValue(key, out var asset))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
            else
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset AddAsset<TAsset>(TAsset asset)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnAddAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return (TAsset)handlerResult.Current.Asset!;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                throw new ArgumentException($"Key type {key.GetType()} does not match key type {assetSet.AssetMapping.KeyType} of asset type {typeof(TAsset)}.");
            }

            if (assetSet.Assets.TryGetValue(key, out var _))
            {
                throw new ArgumentException($"An asset with type {typeof(TAsset)} and key '{key}' already exists.");
            }

            assetSet.Assets[key] = asset;

            previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, true);
            var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
            return new OnAssetEventResult(previousResultData.Value, currentResultData);
        }
    }

    public bool TryAddAsset<TAsset>(TAsset asset, out TAsset? result)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnAddAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = (TAsset?)handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), null, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType
                || assetSet.Assets.TryGetValue(key, out var _))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets[key] = asset;

                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, true);
                var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset UpsertAsset<TAsset>(TAsset asset)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnUpsertAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return (TAsset)handlerResult.Current.Asset!;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                throw new ArgumentException($"Key type {key.GetType()} does not match key type {assetSet.AssetMapping.KeyType} of asset type {typeof(TAsset)}.");
            }

            assetSet.Assets.TryGetValue(key, out var prevAsset);

            assetSet.Assets[key] = asset;

            previousResultData = new OnAssetEventResultData(typeof(TAsset), key, prevAsset, true);
            var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
            return new OnAssetEventResult(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpsertAsset<TAsset>(TAsset asset, out TAsset? result)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnUpsertAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = (TAsset?)handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), null, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets.TryGetValue(key, out var prevAsset);

                assetSet.Assets[key] = asset;

                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, prevAsset, true);
                var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset UpdateAsset<TAsset>(TAsset asset)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnUpdateAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return (TAsset)handlerResult.Current.Asset!;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                throw new ArgumentException($"Key type {key.GetType()} does not match key type {assetSet.AssetMapping.KeyType} of asset type {typeof(TAsset)}.");
            }

            if (!assetSet.Assets.TryGetValue(key, out var prevAsset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
            }

            assetSet.Assets[key] = asset;

            previousResultData = new OnAssetEventResultData(typeof(TAsset), key, prevAsset, true);
            var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
            return new OnAssetEventResult(previousResultData.Value, currentResultData);
        }
    }

    public bool TryUpdateAsset<TAsset>(TAsset asset, out TAsset? result)
    {
        EnsureArg.HasValue(asset);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), null, asset);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnUpdateAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = (TAsset?)handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), null, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }

            var key = assetSet.AssetMapping.KeySelector(requestData.Asset!);

            if (key.GetType() != assetSet.AssetMapping.KeyType
                || !assetSet.Assets.TryGetValue(key, out var prevAsset))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets[key] = asset;

                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, prevAsset, true);
                var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
        }
    }

    public TAsset DeleteAsset<TAsset>(object key)
    {
        EnsureArg.HasValue(key);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnDeleteAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        return (TAsset)handlerResult.Current.Asset!;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet))
            {
                throw new KeyNotFoundException($"A set of assets was not found matching the type {typeof(TAsset)}.");
            }

            if (key.GetType() != assetSet.AssetMapping.KeyType)
            {
                throw new ArgumentException($"Key type {key.GetType()} does not match key type {assetSet.AssetMapping.KeyType} of asset type {typeof(TAsset)}.");
            }

            if (!assetSet.Assets.TryGetValue(key, out var asset))
            {
                throw new KeyNotFoundException($"An asset with type {typeof(TAsset)} and key '{key}' was not found.");
            }

            assetSet.Assets.Remove(key);

            previousResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
            var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, null, true);
            return new OnAssetEventResult(previousResultData.Value, currentResultData);
        }
    }

    public bool TryDeleteAsset<TAsset>(object key, out TAsset? result)
    {
        EnsureArg.HasValue(key);

        var initialRequestData = new OnAssetEventRequestData(typeof(TAsset), key, null);

        var handler = RootHandler;

        OnAssetEventResultData? previousResultData = null;
        OnAssetEventResultData? currentResultData = null;

        var eventHandlers = localServiceProvider
            .GetServices<IOnDeleteAssetEvent<TAsset>>()
            .Reverse()
            .ToArray();

        foreach (var eventHandler in eventHandlers)
        {
            var innerHandler = handler;
            handler = (in OnAssetEventRequestData currentRequestData) =>
            {
                var request = new OnAssetEventRequest(in initialRequestData, in currentRequestData);
                currentResultData = eventHandler.Execute(in request, (in OnAssetEventRequestData requestData) => innerHandler(requestData));
                return new OnAssetEventResult(previousResultData!.Value, currentResultData.Value);
            };
        }

        var handlerResult = handler(initialRequestData);

        result = (TAsset?)handlerResult.Current.Asset;

        return handlerResult.Current.Success;

        OnAssetEventResult RootHandler(in OnAssetEventRequestData requestData)
        {
            if (!assetSetCache.TryGetValue(typeof(TAsset), out var assetSet)
                || key.GetType() != assetSet.AssetMapping.KeyType
                || !assetSet.Assets.TryGetValue(key, out var asset))
            {
                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, null, false);
                var currentResultData = previousResultData.Value;
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
            }
            else
            {
                assetSet.Assets.Remove(key);

                previousResultData = new OnAssetEventResultData(typeof(TAsset), key, asset, true);
                var currentResultData = new OnAssetEventResultData(typeof(TAsset), key, null, true);
                return new OnAssetEventResult(previousResultData.Value, currentResultData);
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
        public Type KeyType { get; }
        public Func<object, object> KeySelector { get; }
    }

    public class AssetMapping<TAsset, TAssetKey> : IAssetMapping
    {
        public AssetMapping(Func<TAsset, TAssetKey> keySelector)
        {
            AssetType = typeof(TAsset);
            KeyType = typeof(TAssetKey);
            KeySelector = asset => keySelector((TAsset)asset)!;
        }

        public Type AssetType { get; }
        public Type KeyType { get; }
        public Func<object, object> KeySelector { get; }
    }
}