namespace SeLib.API;

public abstract class AssetModule<T> : AssetModule where T : AssetModule<T>
{
    
}

public abstract class AssetModule : INode
{
    public ExtendedUnityPlugin Owner { get; }
    
    public virtual string AssetBundleDirectoryPath => string.Empty;
    public virtual string LanguageDirectoryPath => string.Empty;
    public virtual string SoundBankDirectoryPath => string.Empty;
}