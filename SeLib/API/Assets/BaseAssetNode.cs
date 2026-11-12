using System.Collections;
using SeLib.Core;
using UnityEngine;

namespace SeLib.API.Assets;

public abstract class BaseAssetNode : BaseNode<BaseAssetNode, BaseAssetProvider>
{
    public abstract IEnumerator Init();
    
    public T LoadAsset<T>(string assetName) where T : Object => Parent.LoadAssetFromHolder<T>(Identifier, assetName);
    public T[] LoadAllAssets<T>() where T : Object => Parent.LoadAllAssetsFromHolder<T>(Identifier);
    
    public IAssetHolderRequest<T> LoadAssetAsync<T>(string assetName) where T : Object => Parent.LoadAssetFromHolderAsync<T>(Identifier, assetName);
    public IAssetHolderRequest<T> LoadAllAssetsAsync<T>() where T : Object => Parent.LoadAllAssetsFromHolderAsync<T>(Identifier);
}