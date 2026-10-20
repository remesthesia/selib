namespace SeLib.API.Content;

public abstract class ContentNode : INode<ContentProvider>
{
    public BaseRoR2Plugin Owner => Parent.Owner;
    public ContentProvider Parent { get; internal set; }
}