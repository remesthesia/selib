namespace SeLib.API.Assets;

public abstract class AssetProvider<T> : AssetProvider where T : AssetProvider<T>
{
    
}

public abstract class AssetProvider : BaseModule<AssetProvider, AssetNode>, IAssetBundleProvider
{
    
}