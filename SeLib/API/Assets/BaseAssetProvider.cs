using System.Collections;
using System.Collections.Generic;
using System.IO;
using SeLib.Core;
using UnityEngine;

namespace SeLib.API.Assets;

public abstract class BaseAssetProvider<T> : BaseAssetProvider where T : BaseAssetProvider<T>
{
    public static T Instance { get; private set; }

    public override void Init() => Instance = this as T;
}

public abstract class BaseAssetProvider : BaseProvider<BaseAssetProvider, BaseAssetNode>
{
    private Dictionary<string, AssetBundle> _assetBundles = [];

    public T LoadAsset<T>(string bundle, string asset) where T : Object
    {
        return _assetBundles[bundle].LoadAsset<T>(asset);
    }
    
    public AssetBundleRequest LoadAssetAsync<T>(string bundle, string asset) where T : Object
    {
        return _assetBundles[bundle].LoadAssetAsync<T>(asset);
    }
    
    public IEnumerator LoadAssetBundles()
    {
        foreach (var assetBundlePath in Directory.EnumerateFiles(Path.Combine(Directory.GetParent(Owner.Info.Location).FullName, "Assets")))
        {
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            while (!assetBundleCreateRequest.isDone) yield return null;
            
            var assetBundle = assetBundleCreateRequest.assetBundle;
            _assetBundles.Add(assetBundle.name, assetBundle);
        }
        
        foreach (var node in Children)
        {
            yield return node.Init();
        }
    }
}