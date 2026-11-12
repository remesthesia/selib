using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SeLib.Core;
using SeLib.Core.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SeLib.API.Assets;

public abstract class BaseAssetProvider<T> : BaseAssetProvider where T : BaseAssetProvider<T>
{
    public static T Instance { get; private set; }

    public override void Init()
    {
        base.Init();
        Instance = this as T;
    }
}

public abstract class BaseAssetProvider : BaseProvider<BaseAssetProvider, BaseAssetNode>
{
    private readonly Dictionary<string, AssetHolder> _holders = [];
    private Transform _root;

    public override void Init()
    {
        _root = new GameObject($"{Owner.Name} Prefabs").transform;
        _root.gameObject.SetActive(false);
        _root.transform.parent = Owner.transform;
        
        On.RoR2.Util.IsPrefab += (orig, gameObject) =>
        {
            var flag = orig(gameObject);
            return flag || (_root && gameObject.transform.parent == _root);
        };
    }

    public GameObject ClonePrefab(GameObject prefab, string name)
    {
        var instance = Object.Instantiate(prefab,  _root);
        instance.name = name;

        return instance;
    }

    public void SaveAssetToHolder<T>(string holderName, T asset) where T : Object
    {
        _holders[holderName].SaveAsset(asset);
    }
    
    public T LoadAssetFromHolder<T>(string holderName, string assetName) where T : Object
    {
        return _holders[holderName].LoadAsset<T>(assetName);
    }
    
    public T[] LoadAllAssetsFromHolder<T>(string holderName) where T : Object
    {
        return _holders[holderName].LoadAllAssets<T>();
    }
    
    public IAssetHolderRequest<T> LoadAssetFromHolderAsync<T>(string holderName, string assetName) where T : Object
    {
        return _holders[holderName].LoadAssetAsync<T>(assetName);
    }
    
    public IAssetHolderRequest<T> LoadAllAssetsFromHolderAsync<T>(string holderName) where T : Object
    {
        Debug.logger.Log("Loadiang all assets");
        return _holders[holderName].LoadAllAssetsAsync<T>();
    }

    public string[] GetAllHolderNames() => _holders.Keys.ToArray();
    
    public IEnumerator LoadAssetBundles()
    {
        foreach (var assetBundlePath in Directory.EnumerateFiles(Path.Combine(Directory.GetParent(Owner.Info.Location).FullName, "Assets")))
        {
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
            while (!assetBundleCreateRequest.isDone) yield return null;
            
            var assetBundle = assetBundleCreateRequest.assetBundle;
            _holders[assetBundle.name] = new AssetHolder(assetBundle);
            
            foreach (var material in assetBundle.LoadAllAssets<Material>())
            {
                var shaderName = material.shader.name;
                if (!shaderName.StartsWith("Stubbed")) continue;
                
                
                var targetShaderName = shaderName.Substring("Stubbed".Length);
                
                var shader = Addressables.LoadAssetAsync<Shader>(targetShaderName + ".shader").WaitForCompletion();
                material.shader = shader;
                SaveAssetToHolder(assetBundle.name,  material);
            }
        }
        
        foreach (var node in Children)
        {
            yield return node.Init();
        }
    }
    
    
    /*
    private Dictionary<string, AssetBundle> _assetBundles = [];
    private Dictionary<string, Object> _assets = [];

    public Transform _root;

    public void SaveAsset(string bundle, Object asset)
    {
        
        _assets[bundle] = asset;
    }
    
    public GameObject ClonePrefab(GameObject src, string newName)
    {
        if (!_root)
        {
            _root = new GameObject("Root").transform;
            _root.transform.parent = Owner.transform;
            _root.gameObject.SetActive(false);
        }
        
        var instance = Object.Instantiate(src, _root);
        instance.name = newName;
        return instance;
    }

    public void AddPrefab(GameObject prefab)
    {
        if (!_root)
        {
            _root = new GameObject("Root").transform;
            _root.transform.parent = Owner.transform;
            _root.gameObject.SetActive(false);
        }
        
        prefab.transform.parent = _root;
    }
    
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
            foreach (var material in assetBundle.LoadAllAssets<Material>())
            {
                var shaderName = material.shader.name;
                if (!shaderName.StartsWith("Stubbed")) continue;
                
                
                var targetShaderName = shaderName.Substring("Stubbed".Length);
                
                var shader = Addressables.LoadAssetAsync<Shader>(targetShaderName + ".shader").WaitForCompletion();
                material.shader = shader;
                SaveAsset(material.name,  material);
            }
            _assetBundles.Add(assetBundle.name, assetBundle);
        }
        
        foreach (var node in Children)
        {
            yield return node.Init();
        }
    }
    */
}