namespace SeLib.API.Assets;

public abstract class AssetProvider : INode
{
    public BaseRoR2Plugin Owner { get; internal set; }
}