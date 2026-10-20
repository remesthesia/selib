namespace SeLib.API.Assets;

public abstract class AssetNode : INode<AssetProvider>
{
    public BaseRoR2Plugin Owner => Parent.Owner;
    public AssetProvider Parent { get; internal set; }
}