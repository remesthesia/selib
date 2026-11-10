using SeLib.Core;

namespace SeLib.API.Assets;

public abstract class BaseAssetProvider<T> : BaseAssetProvider where T : BaseAssetProvider<T>
{
    public static T Instance { get; private set; }

    public override void Init() => Instance = this as T;
}

public abstract class BaseAssetProvider : BaseProvider<BaseAssetProvider, BaseAssetNode>
{
    
}