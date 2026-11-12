using System;
using System.Collections.Generic;
using System.Linq;
using SeLib.API.Assets;
using SeLib.Core.Assets.Impl;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SeLib.Core.Assets;

internal sealed class AssetHolder(AssetBundle src)
{
    private readonly Dictionary<Type, Dictionary<string, Object>> _cache = [];

    public void SaveAsset<T>(T asset) where T : Object
    {
        if (!_cache.TryGetValue(typeof(T), out var dictionary)) _cache[typeof(T)] = dictionary = new Dictionary<string, Object>();
        
        Debug.Log($"Saving asset {asset.name} of type {typeof(T)}");
        dictionary[asset.name] = asset;
    }

    public T LoadAsset<T>(string name) where T : Object
    {
        if (_cache.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var obj)) return obj as T;
        return src.LoadAsset<T>(name);
    }
    
    public T[] LoadAllAssets<T>() where T : Object
    {
        if (!_cache.TryGetValue(typeof(T), out var dictionary)) return src.LoadAllAssets<T>();
        
        var list = dictionary.Values.Select(obj => obj as T).ToList();
        list.AddRange(src.LoadAllAssets<T>());
        
        return [..list];
    }

    public IAssetHolderRequest<T> LoadAssetAsync<T>(string name) where T : Object
    {
        if (_cache.TryGetValue(typeof(T), out var dictionary) && dictionary.TryGetValue(name, out var obj)) return new CacheAssetHolderRequest<T>(obj as T);
        return new BundleAssetHolderRequest<T>(src.LoadAssetAsync<T>(name));
    }
    
    public IAssetHolderRequest<T> LoadAllAssetsAsync<T>() where T : Object
    {
        Debug.logger.Log("Loadiang all assets 2");
        if (!_cache.TryGetValue(typeof(T), out var dictionary))
        {
            Debug.logger.Log("Cache");
            return new BundleAssetHolderRequest<T>(src.LoadAllAssetsAsync());
        }

        
        Debug.logger.Log("Both");
        var list = dictionary.Values.Select(obj => obj as T).ToList();
        return new CompositeAssetHolderRequest<T>([..list], src.LoadAllAssetsAsync<T>());
    }
}