using System.Linq;
using SeLib.API.Assets;
using UnityEngine;

namespace SeLib.Core.Assets.Impl;

internal sealed class BundleAssetHolderRequest<T>(AssetBundleRequest assetBundleRequest) : IAssetHolderRequest<T> where T : Object
{
    public T Asset => assetBundleRequest.asset as T;
    public T[] Assets => assetBundleRequest.allAssets.OfType<T>().ToArray();

    public bool IsDone => assetBundleRequest.isDone;
}