using System.Linq;
using SeLib.API.Assets;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SeLib.Core.Assets.Impl;

internal sealed class CompositeAssetHolderRequest<T>(T[] assets, AssetBundleRequest assetBundleRequest) : IAssetHolderRequest<T> where T : Object
{
    public T Asset => Assets[0];

    public T[] Assets
    {
        get
        {
            foreach (var asset in assets)
            {
                Debug.logger.Log("Asset: " + asset.name);
            }
            
            foreach (var asset in assetBundleRequest.allAssets)
            {
                Debug.logger.Log("Asset: " + asset.name);
            }
            
            foreach (var asset in assetBundleRequest.allAssets.Cast<T>())
            {
                Debug.logger.Log("Asset: " + asset.name);
            }
            return [..assets, ..assetBundleRequest.allAssets.Cast<T>()];
        }
    }

    public bool IsDone => assetBundleRequest.isDone;
}