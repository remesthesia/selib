using SeLib.API.Assets;
using UnityEngine;

namespace SeLib.Core.Assets.Impl;

internal sealed class CacheAssetHolderRequest<T>(T asset) : IAssetHolderRequest<T> where T : Object
{
    public T Asset { get; } = asset;
    public T[] Assets { get; } = [asset];

    public bool IsDone => true;
}