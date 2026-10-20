using SeLib.API.Assets;

namespace SeLib.API.Content;


public abstract class ContentProvider : INode
{
    public BaseRoR2Plugin Owner { get; internal set; }
}