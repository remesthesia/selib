using UnityEngine;

namespace SeLib.API.Assets;

public interface IAssetHolderRequest<T> where T : Object
{
    public T Asset { get; }
    public T[] Assets { get; }
    
    public bool IsDone { get; }
}